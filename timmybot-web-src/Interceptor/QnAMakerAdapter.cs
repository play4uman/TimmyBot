using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker;
using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QnABot.Interceptor
{
    public class QnAMakerAdapter : IQnAMakerAdapter
    {
        public QnAMakerAdapter()
        {
            _kbClient = new Lazy<Task<IQnAMakerClient>>(CreateKbClient);
            _runtimeClient = new Lazy<Task<IQnAMakerRuntimeClient>>(CreateRuntimeClient);
            _primaryEndpointKey = new Lazy<Task<string>>(GetQueryEndpointKey);
        }

        public const string KbId = "91ef0ca3-269e-4e4d-8e3d-856244a02bee";
        public const string AuthoringKey = "13d4e0f9a82b47ffa7990837d8233978";
        public const string AuthoringURL = "https://timmybot.cognitiveservices.azure.com";
        public const string QueryingURL = "https://timmybot.azurewebsites.net";

        private Lazy<Task<IQnAMakerClient>> _kbClient { get; set; }
        private Lazy<Task<IQnAMakerRuntimeClient>> _runtimeClient { get; }
        private Lazy<Task<string>> _primaryEndpointKey { get; set; }

        private async Task<IQnAMakerClient> CreateKbClient()
        {
            return new QnAMakerClient(new ApiKeyServiceClientCredentials(AuthoringKey))
            {
                Endpoint = AuthoringURL
            };
        }
        private async Task<IQnAMakerRuntimeClient> CreateRuntimeClient()
        {
            var endpointKey = await _primaryEndpointKey.Value;
            return new QnAMakerRuntimeClient(new EndpointKeyServiceClientCredentials(endpointKey))
            {
                RuntimeEndpoint = QueryingURL
            };
        }
        private async Task<string> GetQueryEndpointKey()
        {
            var kbClient = await _kbClient.Value;
            var endpointKeysObject = await kbClient.EndpointKeys.GetKeysAsync();

            return endpointKeysObject.PrimaryEndpointKey;
        }

        public async Task<(int? answerId, string answer)> GenerateAnswerAsync(string query)
        {
            var runtimeClient = await _runtimeClient.Value;
            var response = await runtimeClient.Runtime.GenerateAnswerAsync(KbId,
                new QueryDTO
                {
                    Question = query
                });
            return (response.Answers[0].Id, response.Answers[0].Answer);
        }

        public async Task PushQuestionAnswerPairAsync(string question, string subject, string answer)
        {
            var metadata = subject != null ? new List<MetadataDTO> { new MetadataDTO { Name = "Category", Value = subject } } : Enumerable.Empty<MetadataDTO>().ToList();

            var updateKbDTO = new UpdateKbOperationDTO
            {
                Add = new UpdateKbOperationDTOAdd
                {
                    QnaList = new List<QnADTO>
                    {
                        new QnADTO
                        {
                            Questions = new List<string>(new []{ question} ),
                            Answer = answer,
                            Metadata = metadata
                        }
                    }
                }
            };

            var kbClient = (await _kbClient.Value);
            var updateOp = await kbClient.Knowledgebase.UpdateAsync(KbId, updateKbDTO);

            // loop while sucess 
            // https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/quickstarts/quickstart-sdk?tabs=version-1%2Cv1&pivots=programming-language-csharp#get-status-of-an-operation
            updateOp = await MonitorOperation(updateOp, kbClient);
        }


        private async Task<Operation> MonitorOperation(Operation operation, IQnAMakerClient initializedClient)
        {
            // Loop while operation is success
            for (int i = 0;
                i < 20 && (operation.OperationState == OperationStateType.NotStarted || operation.OperationState == OperationStateType.Running);
                i++)
            {
                await Task.Delay(500);
                operation = await initializedClient.Operations.GetDetailsAsync(operation.OperationId);
            }

            if (operation.OperationState != OperationStateType.Succeeded)
            {
                throw new Exception($"Operation {operation.OperationId} failed to completed.");
            }
            return operation;
        }
    }
}

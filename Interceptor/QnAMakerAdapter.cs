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

        public const string KbId = "17fe2152-9322-432d-a0c5-9ce82db8550f";
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
    }
}

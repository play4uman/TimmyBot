using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker;
using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QnABot.Interceptor
{
    public class QuestionInterceptor : IQuestionInterceptor
    {
        public QuestionInterceptor(IQnAMakerAdapter qnAMakerAdapter, Client fileApiClient)
        {
            _qnAMakerAdapter = qnAMakerAdapter;
            _fileApiClient = fileApiClient;
        }

        private readonly IQnAMakerAdapter _qnAMakerAdapter;
        private readonly Client _fileApiClient;

        public async Task<ResponseStatus> InterceptQueryAsync(string question, string subject)
        {
            var answerPair = await _qnAMakerAdapter.GenerateAnswerAsync(question);
            ResponseStatus responseStatus;
            if (NoAnswer(answerPair))
            {
                var timmyAnswer = await _fileApiClient.QuestionAsync(question, subject);
                string answer = timmyAnswer.Answer;
                if (GoodTimmyAnswer(answer))
                {
                    await _qnAMakerAdapter.PushQuestionAnswerPairAsync(question, subject, answer);
                    responseStatus = ResponseStatus.Added;
                }
                else
                {
                    responseStatus = ResponseStatus.NotFoundAndNotAdded;
                }
            }
            else
            {
                responseStatus = ResponseStatus.Found;
            }
            return responseStatus;
        }

        private bool NoAnswer((int? id, string answer) answerPair)
        {
            return (!answerPair.id.HasValue) || (answerPair.id.HasValue && answerPair.id.Value < 0);
        }

        private bool GoodTimmyAnswer(string answer)
        {
            return true; // todo: && implement when an answer is not satisfactory
        }


    }
}

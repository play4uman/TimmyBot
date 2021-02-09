using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker;
using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace QnABot.Interceptor
{
    public class QuestionInterceptor : IQuestionInterceptor
    {
        public QuestionInterceptor(IQnAMakerAdapter qnAMakerAdapter)
        {
            _qnAMakerAdapter = qnAMakerAdapter;
        }

        private readonly IQnAMakerAdapter _qnAMakerAdapter;

        public async Task<ResponseStatus> InterceptQueryAsync(string query)
        {
            var answerPair = await _qnAMakerAdapter.GenerateAnswerAsync(query);
            if (NoAnswer(answerPair))
            {
                //...
            }

            return ResponseStatus.Found;
        }

        private bool NoAnswer((int? id, string answer) answerPair)
        {
            return (!answerPair.id.HasValue) || (answerPair.id.HasValue && answerPair.id.Value < 0);
        }
    }
}

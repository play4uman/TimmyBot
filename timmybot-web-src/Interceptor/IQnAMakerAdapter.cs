using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QnABot.Interceptor
{
    public interface IQnAMakerAdapter
    {
        Task<(int? answerId, string answer)> GenerateAnswerAsync(string query);
        Task PushQuestionAnswerPairAsync(string question, string subject, string answer);
    }
}

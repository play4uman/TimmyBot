using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QnABot.Interceptor
{
    public enum ResponseStatus
    {
        Found,
        Added,
        NotFound
    }
    public interface IQuestionInterceptor
    {
        Task<ResponseStatus> InterceptQueryAsync(string query);
    }
}

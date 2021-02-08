using Newtonsoft.Json;
using QnABot.Interceptor.DTO;
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
        public const string KbUri = "https://timmybot.azurewebsites.net/qnamaker/knowledgebases/17fe2152-9322-432d-a0c5-9ce82db8550f/generateAnswer";

        public async Task<ResponseStatus> InterceptQueryAsync(string query)
        {
            using var httpClient = new HttpClient();

            using var httpRequest = new HttpRequestMessage();
            var content = new StringContent(JsonConvert.SerializeObject(new { question = query}));
            httpRequest.RequestUri = new Uri(KbUri);
            httpRequest.Method = HttpMethod.Post;
            httpRequest.Content = content;
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("EndpointKey", " 6d11317c-2fa3-4f62-a9fb-50fdc8f7b8d2");
            httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            var response = await httpClient.SendAsync(httpRequest);
            var responseText = await response.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<QueryResponse>(responseText);

            if (responseObj.Answers[0].Id < 0)
            { 
            }


            return ResponseStatus.Found;
        }
    }
}

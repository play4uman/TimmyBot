using Flurl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.API
{
    public partial class Client
    {
        public async Task<string> LoadDocIntoMemoryAsync(string docRelativeUrl)
        {
            var docFullUrl = Url.Combine(BaseUrl, docRelativeUrl);
            using var httpReq = new HttpRequestMessage(new HttpMethod("GET"), docFullUrl);
            var response = await _httpClient.SendAsync(httpReq);
            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Newtonsoft.Json;
using QnABot.Interceptor;

namespace Microsoft.BotBuilderSamples
{
    // This ASP Controller is created to handle a request. Dependency Injection will provide the Adapter and IBot
    // implementation at runtime. Multiple different IBot implementations running at different endpoints can be
    // achieved by specifying a more specific type for the bot constructor argument.
    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly IBot _bot;
        private readonly IQuestionInterceptor _questionInterceptor;

        public BotController(IBotFrameworkHttpAdapter adapter, IBot bot, IQuestionInterceptor questionInterceptor)
        {
            _adapter = adapter;
            _bot = bot;
            _questionInterceptor = questionInterceptor;
        }

        [HttpGet, HttpPost]
        public async Task PostAsync()
        {
            using var sr = new StreamReader(Request.Body);
            var bodyText = sr.ReadToEnd();
            byte[] requestData = Encoding.UTF8.GetBytes(bodyText);

            dynamic bodyObj = JsonConvert.DeserializeObject(bodyText);
            if ((string)bodyObj.type == "message")
            {
                var result = await _questionInterceptor.InterceptQueryAsync((string)bodyObj.text);
            }

            Request.Body = new MemoryStream(requestData);

            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.
            await _adapter.ProcessAsync(Request, Response, _bot);
        }
    }
}

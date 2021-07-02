// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
            var bodyText = await sr.ReadToEndAsync();

            dynamic bodyObj = JsonConvert.DeserializeObject(bodyText);
            byte[] requestData = Encoding.UTF8.GetBytes(bodyText); // og request data

            if ((string)bodyObj.type == "message")
            {
                var fullQuery = (string)bodyObj.text;
                var (subject, question) = ParseQueryIntoQuestionAndSubject(fullQuery);
                var result = await _questionInterceptor.InterceptQueryAsync(question, subject);
                bodyObj.text = question;
                var newSerializedBody = JsonConvert.SerializeObject(bodyObj);
                requestData = Encoding.UTF8.GetBytes(newSerializedBody);
            }

            Request.Body = new MemoryStream(requestData);

            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.
            await _adapter.ProcessAsync(Request, Response, _bot);
        }

        private (string subject, string question) ParseQueryIntoQuestionAndSubject(string query)
        {
            // We can invoke a subject with "Subject: computer science; What is computer science?"
            var regexWithSubject = new Regex("subject:(.*);(.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var subjectMatch = regexWithSubject.Match(query);
            string subject = null;
            string question = query.Trim();
            if (subjectMatch.Success)
            {
                subject = subjectMatch.Groups[1].Value.Replace(" ", "").Trim();
                question = subjectMatch.Groups[2].Value.Trim();
            }

            return (subject, question);
        }
    }
}

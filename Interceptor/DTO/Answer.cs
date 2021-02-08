using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QnABot.Interceptor.DTO
{
    public class Answer
    {
        public string[] Questions { get; set; }
		[JsonProperty("answer")]
        public string AnswerString { get; set; }
        public double Score { get; set; }
        public int Id { get; set; }
        public bool IsDocumentText { get; set; }
        public string[] Metadata { get; set; }
    }
}

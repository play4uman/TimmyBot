using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QnABot.Interceptor.DTO
{
    public class QueryResponse
    {
        public Answer[] Answers { get; set; }
        public bool ActiveLearningDisabled { get; set; }
    }
}


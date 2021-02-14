using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.Algorithm.NLP
{
    public class QueryParsingResult
    {
        public string[] BM25Tokens { get; set; }
        public string[] BERTTokens { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using AnswerExtraction.ProcessExecution;
using Newtonsoft.Json;

namespace AnswerExtraction.Algorithm.NLP
{
    public class QueryParser : IQueryParser
    {
        public QueryParser(IExecutor executor)
        {
            _executor = executor;
        }
        public readonly string ParserFileName = "NLP-1.0-SNAPSHOT-jar-with-dependencies.jar";
        private readonly IExecutor _executor;
        public async Task<QueryParsingResult> ParseQueryAsync(string query)
        {
            string output = null;
            _executor.DataReceived += (o, ea) =>
            {
                if (ea.Data != null)
                    output = ea.Data;
            };
            _executor.ErrorReceived += (o, ea) => Console.WriteLine(ea.Data);
            var process = await _executor.RunProcessAsync("java", $"-jar ../NLP/target/{ParserFileName} \"{query}\"");

            if (output is null)
                throw new ApplicationException("Couldn't read data from query parser");

            var parsedResult = JsonConvert.DeserializeObject<QueryParsingResult>(output);
            return parsedResult;
        }
    }
}

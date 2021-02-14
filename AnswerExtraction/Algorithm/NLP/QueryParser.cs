using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AnswerExtraction.Algorithm.NLP
{
    public class QueryParser : IQueryParser
    {

        public QueryParser()
        {
            stopWords = LoadStopWords();
        }

        private HashSet<string> stopWords;

        public string[] ParseQuery(string query)
        {
            string cleanedQuery = RemoveStopWords(query);
            return cleanedQuery.Split(null);
        }

        private HashSet<string> LoadStopWords()
        {
           
            var assembly = typeof(QueryParser).Assembly;
            Stream StopWordsFile = assembly.GetManifestResourceStream("AnswerExtraction.Algorithm.NLP.common-english-words.txt");
            using var sr = new StreamReader(StopWordsFile);
            return sr.ReadToEnd().Split(null).ToHashSet();
        }

        private string RemoveStopWords(string query)
        {
            string[] queryTokens = query.Split(null);
            var cleanTokens = queryTokens.Where(qt => !stopWords.Contains(qt));
            var result = string.Join(' ', cleanTokens);

            return result;
        }
    }
}

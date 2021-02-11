using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.Algorhitm.NLP
{
    public class QueryParser : IQueryParser
    {
        // Insert NLP implementation here
        public string[] ParseQuery(string query)
        {
            return query.Split(null);
        }
    }
}

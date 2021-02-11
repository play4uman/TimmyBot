using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.Algorhitm.NLP
{
    public interface IQueryParser
    {
        string[] ParseQuery(string query);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.Algorhitm.DocumentRanking
{
    public interface IBM25
    {
        double Compute(string doc, int docLength, IEnumerable<string> keywords, int numberOfDocs, double avgDocLength,
            Dictionary<string, int> keywordsContainedInMap);
    }
}

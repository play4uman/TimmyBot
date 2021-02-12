using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.Algorithm.DocumentRanking
{
    public interface IBM25
    {
        double Compute(string doc, int docLength, IEnumerable<FlaggedKeyword> flaggedKeywords, int numberOfDocs, double avgDocLength,
            Dictionary<string, int> keywordsContainedInMap);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.Algorithm.BERT
{
    public interface IBertWrapper
    {
        Task<string> GetAnswerAsync(string question, string paragraph, bool debug = false);
    }
}

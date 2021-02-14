using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.Algorithm.DocumentParsing
{
    public interface IParagraphSplitter
    {
        string[] SplitIntoParagraphs(string doc);
    }
}

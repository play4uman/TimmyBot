using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.Algorithm.DocumentParsing
{
    public class ParagraphSplitter : IParagraphSplitter
    {
        public string[] SplitIntoParagraphs(string doc)
        {
            string[] paragraphs = doc.Split($"{Environment.NewLine}{Environment.NewLine}");
            return paragraphs;
        }
    }
}

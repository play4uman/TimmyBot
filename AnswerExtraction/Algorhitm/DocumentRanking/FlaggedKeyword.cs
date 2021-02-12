using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.Algorhitm.DocumentRanking
{
    public class FlaggedKeyword : IEquatable<FlaggedKeyword>
    {
        public FlaggedKeyword(Guid fileId, string keyword, bool matched, int? times = null)
        {
            FileId = fileId;
            Keyword = keyword;
            Matched = matched;
            Times = times;
        }
        public Guid FileId { get;  }
        public string Keyword { get;  }
        public bool Matched { get;  }
        public int? Times { get; set; }

        public override int GetHashCode()
        {
            return FileId.GetHashCode() * 17 + Keyword.GetHashCode();
        }
        public bool Equals(FlaggedKeyword other)
        {
            return this.FileId == other.FileId && this.Keyword == other.Keyword;
        }
    }
}

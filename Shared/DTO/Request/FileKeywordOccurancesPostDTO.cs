using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTO.Request
{
    public class FileKeywordOccurancesPostDTO
    {
        public Guid FileId { get; set; }
        public string Keyword { get; set; }
        public int Times { get; set; }
    }
}

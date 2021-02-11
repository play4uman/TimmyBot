using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTO.Response
{
    public class FileMetadataDTO
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string OriginalName { get; set; }
        public string Category { get; set; }
        public int WordCount { get; set; }
        public int? PreferredParagraphDelimiter { get; set; }
        public string[] Tags { get; set; }
        public Dictionary<string, int> Keywords { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTO.Request
{
    public class FileMetadataUploadDTO
    {
        public string Category { get; set; }
        public string[] Tags { get; set; }
        public int? PreferredParagraphDelimiter { get; set; }
    }
}

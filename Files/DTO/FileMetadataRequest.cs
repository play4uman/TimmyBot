using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Files.DTO
{
    public class FileMetadataRequest
    {
        public string Category { get; set; }
        public string[] Tags { get; set; }
    }
}

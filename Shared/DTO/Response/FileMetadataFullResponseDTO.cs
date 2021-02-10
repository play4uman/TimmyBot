using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTO.Response
{
    public class FileMetadataFullResponseDTO
    {
        public FileMetadataDTO[] Metadata { get; set; }
        public double AvgWordCount { get; set; }
    }
}

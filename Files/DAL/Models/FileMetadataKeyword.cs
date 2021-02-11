using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Files.DAL.Models
{
    public class FileMetadataKeyword
    {
        [Required]
        public Guid FileId { get; set; }
        public FileMetadata File { get; set; }
        [Required]
        public string KeywordId { get; set; }
        public Keyword Keyword { get; set; }
        [Required]
        public int Times { get; set; }
    }
}

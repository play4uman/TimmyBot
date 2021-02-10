using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Files.DAL.Models
{
    public class FileMetadata
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public string OriginalFileName { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string FilePath { get; set; }
        [Required]
        public int WordCount { get; set; }
        // \r = 10, \n = 10, \f = 12
        public int? PreferredParagraphDelimiter { get; set; }
        public ICollection<FileTag> Tags { get; set; }
    }
}

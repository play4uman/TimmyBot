using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Files.DAL.Models
{
    public class FileTag
    {
        [Key]
        public string Tag { get; set; }
        public ICollection<FileMetadata> TaggedFiles { get; set; }
    }
}

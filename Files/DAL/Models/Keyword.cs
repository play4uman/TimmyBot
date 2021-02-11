using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Files.DAL.Models
{
    public class Keyword
    {
        [Key]
        public string Word { get; set; }
        public ICollection<FileMetadataKeyword> OfFiles { get; set; }
    }
}

using Shared.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.Extensions.Enumerable
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<FileMetadataDTO> GetMetadataBasedOnSubject(this IEnumerable<FileMetadataDTO> fileMetadata, string subject)
        {
            return fileMetadata.Where(fm => subject != null ? fm.Category.Equals(subject, StringComparison.OrdinalIgnoreCase) : true);
        }
    }
}

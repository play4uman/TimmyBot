using Shared.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.Algorithm
{
    public static class Tags
    {
        public static FileMetadataDTO BestMatch(string[] keywords, IEnumerable<FileMetadataDTO> filesMetadata)
        {
            var result = filesMetadata
                            .Select(fm =>
                            {
                                var numberOfMatchedTags = fm.Tags.Intersect(keywords).Count();
                                return new { fm, numberOfMatchedTags };
                            })
                            .OrderByDescending(pair => pair.numberOfMatchedTags)
                            .First();
            return result.numberOfMatchedTags != 0 ? result.fm : null;         
        }
    }
}

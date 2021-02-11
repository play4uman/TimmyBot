using BrunoZell.ModelBinding;
using Files.DAL;
using Files.DAL.Models;
using Files.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Request;
using Shared.DTO.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Files.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        public FileController(IFileUploadService fileUploadService, FilesDbContext dbContext)
        {
            _fileUploadService = fileUploadService;
            _dbContext = dbContext;
        }

        private readonly IFileUploadService _fileUploadService;
        private readonly FilesDbContext _dbContext;

        [HttpPost("new")]
        public async Task<ActionResult> PostFile(
            [ModelBinder(BinderType = typeof(JsonModelBinder))] FileMetadataUploadDTO fileMetadata,
            IFormFile file)
        {
            var fileUploadResult = await _fileUploadService.UploadFileAsync(file, fileMetadata.Category);
            var fileMetadataEntity = new DAL.Models.FileMetadata
            {
                Id = fileUploadResult.fileId,
                FileName = $"{fileUploadResult.fileId.ToString().ToUpper()}.{fileUploadResult.fileExtension}",
                OriginalFileName = file.FileName,
                Category = fileMetadata.Category,
                FilePath = fileUploadResult.relativeUploadPath.Replace(@"\", "/"),
                WordCount = CountWords(file),
                PreferredParagraphDelimiter = fileMetadata.PreferredParagraphDelimiter
            };
            await ConnectTags(fileMetadataEntity, fileMetadata.Tags);

            _dbContext.FileMetadata.Add(fileMetadataEntity);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        // Gets the job done but it's just a quick hack. Needs rewriting
        private async Task ConnectTags(DAL.Models.FileMetadata metadataEntity, IEnumerable<string> tagCandidates)
        {
            var existingTags = await _dbContext.FileTags
                .Where(t => tagCandidates.Any(c => t.Tag == c))
                .Include(t => t.TaggedFiles)
                .ToListAsync();

            foreach (var existing in existingTags)
            {
                existing.TaggedFiles.Add(metadataEntity);
            }

            var newTags = tagCandidates.Except(existingTags.Select(et => et.Tag));
            metadataEntity.Tags = newTags.Select(nt => new DAL.Models.FileTag { Tag = nt }).ToList();
        }

        // Quick and dirty - not optimized. Also it should be in a separate service
        private int CountWords(IFormFile file)
        {
            using var sr = new StreamReader(file.OpenReadStream());
            var text = sr.ReadToEnd();
            int wordCount = text.Split(null).Length;
            return wordCount;
        }

        [HttpGet]
        public async Task<ActionResult<FileMetadataFullResponseDTO>> GetFileMetadata()
        {
            var result = new FileMetadataFullResponseDTO
            {
                Metadata = await _dbContext.FileMetadata
                    .Include(fm => fm.Tags)
                    .Select(fm => fm.ToDto())
                    .ToArrayAsync(),
                AvgWordCount = await _dbContext.FileMetadata
                    .AverageAsync(fm => fm.WordCount)
            };
                    
            return Ok(result);
        }
    }
}

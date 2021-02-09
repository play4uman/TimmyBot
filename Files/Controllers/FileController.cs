using BrunoZell.ModelBinding;
using Files.DAL;
using Files.DAL.Models;
using Files.DTO;
using Files.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        [HttpPost("/file/new")]
        public async Task<ActionResult> PostFile(
            [ModelBinder(BinderType = typeof(JsonModelBinder))] DTO.FileMetadataRequest fileMetadata,
            IFormFile file)
        {
            var fileUploadResult = await _fileUploadService.UploadFileAsync(file, fileMetadata.Category);
            var fileMetadataEntity = new DAL.Models.FileMetadata
            {
                Id = fileUploadResult.fileId,
                FileName = $"{fileUploadResult.fileId.ToString().ToUpper()}.{fileUploadResult.fileExtension}",
                FilePath = fileUploadResult.relativeUploadPath,
                WordCount = 1 // todo calculate
            };
            await ConnectTags(fileMetadataEntity, fileMetadata.Tags);

            _dbContext.FileMetadata.Add(fileMetadataEntity);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }



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
            metadataEntity.Tags = newTags.Select(nt => new DAL.Models.FileTag { Tag = nt}).ToList();
        }
    }
}

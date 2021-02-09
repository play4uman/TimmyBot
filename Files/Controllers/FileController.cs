using BrunoZell.ModelBinding;
using Files.DTO;
using Files.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public FileController(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        private readonly IFileUploadService _fileUploadService;

        [HttpPost("/file/new")]
        public async Task<ActionResult> PostFile(
            [ModelBinder(BinderType = typeof(JsonModelBinder))] FileMetadata fileMetadata,
            IFormFile file)
        {
            var fileUploadResult = await _fileUploadService.UploadFileAsync(file, fileMetadata.Category);


            return Ok();
        }
    }
}

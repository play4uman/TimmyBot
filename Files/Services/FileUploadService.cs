using Files.DTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Files.Services
{
    public class FileUploadService : IFileUploadService
    {
        public FileUploadService(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        private IWebHostEnvironment _hostingEnvironment;
        public string AppBasePath { get => _hostingEnvironment.WebRootPath; }
        public const string DocumentsFolder = "Documents";

        public async Task<(Guid fileId, string relativeUploadPath)> UploadFileAsync(IFormFile file, string category)
        {
            var originalExtension = file.FileName.Split('.').Last();

            var fileIdGuid = Guid.NewGuid();
            var fileId = fileIdGuid.ToString();
            var newFileName = $"{fileId}.{originalExtension}";

            var relativeToRootPath = Path.Combine(DocumentsFolder, category, newFileName);

            var fullFilePath = Path.Combine(AppBasePath, relativeToRootPath);

            using var fs = new FileStream(fullFilePath, FileMode.Create);
            await file.CopyToAsync(fs);
            
            return (fileIdGuid, relativeToRootPath);
        }
    }
}

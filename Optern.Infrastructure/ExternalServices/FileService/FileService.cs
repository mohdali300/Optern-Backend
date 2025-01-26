using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Optern.Infrastructure.ExternalInterfaces.IFileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.ExternalServices.FileService
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetFileUrl(string path)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
                return path;

            path = path.TrimStart('/');

            var absoluteUrl = $"{request.Scheme}://{request.Host}/{path.Replace("\\", "/")}";
            return absoluteUrl;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folderName = "general")
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            var targetFolder = Path.Combine(_env.WebRootPath, "uploads", folderName);
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(targetFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var httpContext = _httpContextAccessor.HttpContext;
            var request = httpContext.Request;

            var fullUrl = $"{request.Scheme}://{request.Host}/uploads/{folderName}/{uniqueFileName}";

            return fullUrl;
        }
    }
}

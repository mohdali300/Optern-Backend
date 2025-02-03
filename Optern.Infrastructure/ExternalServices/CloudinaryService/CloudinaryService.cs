using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Optern.Infrastructure.ExternalInterfaces.IFileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace Optern.Infrastructure.ExternalServices.FileService
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var account = new Account
            {
                Cloud = configuration["Cloudinary:CloudName"],
                ApiKey = configuration["Cloudinary:ApiKey"],
                ApiSecret = configuration["Cloudinary:ApiSecret"]
            };

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadFileAsync(IFile file, string folderName = "general")
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            var originalFileName = file.Name;
            var fileExtension = Path.GetExtension(originalFileName).ToLower();
            var fileName = !string.IsNullOrEmpty(originalFileName)
                ? originalFileName
                : $"{Guid.NewGuid()}{fileExtension}";

            using var stream = file.OpenReadStream();

            bool isImage = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".webp" }.Contains(fileExtension);

            var uploadParams = isImage
                ? new ImageUploadParams
                {
                    File = new FileDescription(fileName, stream),
                    Folder = folderName,
                    UseFilename = true,
                    UniqueFilename = true
                }
                : new RawUploadParams
                {
                    File = new FileDescription(fileName, stream),
                    Folder = folderName,
                    UseFilename = true,
                    UniqueFilename = true
                };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return uploadResult?.SecureUrl?.ToString() ?? string.Empty;
        }


        string ICloudinaryService.GetFileUrl(string path)
        {
            throw new NotImplementedException();
        }

     
    }
}

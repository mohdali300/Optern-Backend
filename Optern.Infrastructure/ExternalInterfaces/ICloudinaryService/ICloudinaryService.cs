using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.ExternalInterfaces.IFileService
{
    public interface ICloudinaryService
    {
        public string GetFileUrl(string path);
        public Task<(string PublicId, string Url)> UploadFileAsync(IFile file, string folderName = "general");
        public Task<bool> DeleteFileAsync(string publicId);
    }
}

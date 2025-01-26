using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.ExternalInterfaces.IFileService
{
    public interface IFileService
    {
        public string GetFileUrl(string path);
        public Task<string> SaveFileAsync(IFormFile file, string folderName = "general");
    }
}

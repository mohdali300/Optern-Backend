using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.RepositoryFile
{
    public class RepositoryFileDTO
    {
        public string Description  { get; set; }
        public int RepositoryId { get; set; }

        public RepositoryFileDTO() {
        
            Description = string.Empty;
            RepositoryId = 0;
        }

    }
}

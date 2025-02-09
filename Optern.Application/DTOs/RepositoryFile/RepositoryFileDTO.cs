

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

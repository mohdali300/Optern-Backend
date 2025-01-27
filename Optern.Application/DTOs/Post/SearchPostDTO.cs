using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Post
{
    public class SearchPostDTO
    {
       public SearchPostDTO()
        {
        SourceType=string.Empty;
         Highlight=string.Empty;

        }
        
        public string? SourceType { get; set; } 
        public string? Highlight { get; set; } 
        public PostWithDetailsDTO? Data { get; set; } 
    }
}

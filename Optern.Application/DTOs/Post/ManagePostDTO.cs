using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Post
{
    public class ManagePostDTO
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string>? Tags { get; set; } 
    }
}

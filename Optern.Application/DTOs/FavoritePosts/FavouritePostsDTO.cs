using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Optern.Application.DTOs.Skills;
using Optern.Application.DTOs.Tags;
using Optern.Domain.Enums;

namespace Optern.Application.DTOs.FavoritePosts
{
    public class FavouritePostsDTO
    {

        public FavouritePostsDTO()
        {
            Title = string.Empty;
            Content = string.Empty;
            CreatorId = string.Empty;
            CreatorUserName = string.Empty;
            SavedAt = DateTime.Now;
        }
        public int ? Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreatorId { get; set; }
        public string CreatorUserName { get; set; }
        public ICollection<TagDTO> Tags { get; set; } = new List<TagDTO>();
        public DateTime SavedAt { get; set; } = DateTime.Now;
    }
}

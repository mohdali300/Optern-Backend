using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optern.Application.DTOs.Post;
using Optern.Infrastructure.Response;

namespace Optern.Application.Interfaces.IFavoritePostsService
{
    public interface IFavoritePostsService
    {
        public Task<Response<List<PostDTO>>> GetFavoritePostsByUserAsync(string userId);

    }
}

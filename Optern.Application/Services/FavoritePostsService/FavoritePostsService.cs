using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs.Post;
using Optern.Application.Interfaces.IFavoritePostsService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Repositories;
using Optern.Infrastructure.Response;

namespace Optern.Application.Services.FavoritePostsService
{
    public class FavoritePostsService :GenericRepository<FavoritePosts>, IFavoritePostsService
    {
        private readonly IMapper _mapper;
        public FavoritePostsService(OpternDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<Response<List<PostDTO>>> GetFavoritePostsByUserAsync(string userId)
        {
            try
            {
                var userExists = await _dbContext.Users.AnyAsync(u => u.Id == userId);
                if (!userExists)
                {
                    return Response<List<PostDTO>>.Failure("User not found.", 404);
                }
                var favoritePosts = await _dbContext.FavoritePosts
                    .Where(fp => fp.UserId == userId) 
                    .Include(fp => fp.Post) 
                    .ToListAsync();

                if (favoritePosts.Any())
                {
                    var postDtos = _mapper.Map<List<PostDTO>>(favoritePosts.Select(fp => fp.Post));

                    return Response<List<PostDTO>>.Success(postDtos, "Favorite posts fetched successfully.");
                }

                return Response<List<PostDTO>>.Failure("No favorite posts found for this user.", 404);
            }
            catch (Exception ex)
            {
                return Response<List<PostDTO>>.Failure("Error occurred while fetching favorite posts.", 500, new List<string> { ex.Message });
            }
        }

    }
}

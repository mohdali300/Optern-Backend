using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs.FavoritePosts;
using Optern.Application.DTOs.Post;
using Optern.Application.DTOs.Room;
using Optern.Application.DTOs.Tags;
using Optern.Application.Interfaces.IFavoritePostsService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Repositories;
using Optern.Infrastructure.Response;
using Optern.Infrastructure.UnitOfWork;

namespace Optern.Application.Services.FavoritePostsService
{
    public class FavoritePostsService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper) : IFavoritePostsService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;


        #region Add Post to Favourite
        public async Task<Response<string>> AddToFavoriteAsync(AddToFavoriteDTO model)
        {
            try
            {

                if (string.IsNullOrEmpty(model.UserId) || model.PostId<=0 )
                {
                    return Response<string>.Failure("Invalid Data Model", 400);
                }
                var user = await _unitOfWork.Users.GetByIdAsync(model.UserId);
                var post = await _unitOfWork.Posts.GetByIdAsync(model.PostId);


                if (user == null || post==null)
                {
                    return Response<string>.Failure("Invalid Data", 404);
                }
               

                var existingFavorite = await _unitOfWork.FavoritePosts
                    .GetByExpressionAsync(fp => fp.UserId == model.UserId && fp.PostId == model.PostId);
                if (existingFavorite != null)
                {
                    return Response<string>.Failure("This post is already in your favorites.", 400);
                }

                var favoritePost = _mapper.Map<FavoritePosts>(model);
                favoritePost.SavedAt=DateTime.UtcNow;
                await _unitOfWork.FavoritePosts.AddAsync(favoritePost);
                await _unitOfWork.SaveAsync();


                return Response<string>.Success("", "Post added to favorites successfully.", 200);
            }
            catch (Exception ex)
            {
                return Response<string>.Failure($"An error occurred while adding the post to favorites: {ex.Message}");
            }
        }
        #endregion

        #region Get Favourite Posts 
        public async Task<Response<IEnumerable<FavouritePostsDTO>>> GetFavoritePostsByUserAsync(string userId)
        {
            try
            {
                var userExists = await _unitOfWork.Users.GetByIdAsync(userId);
                if (userExists == null)
                {
                    return Response<IEnumerable<FavouritePostsDTO>>.Success(new List<FavouritePostsDTO>(), "User does not exist.", 404);

                }

                var favoritePosts = await _unitOfWork.FavoritePosts.GetAllAsync(
                    filter: fp => fp.UserId == userId,
                    includeProperties: "Post.Creator,Post.PostTags.Tag");

                if (!favoritePosts.Any())
                {
                    return Response<IEnumerable<FavouritePostsDTO>>.Success(new List<FavouritePostsDTO>(), "No favorite posts found for the user.", 404);

                }

                var favoritePostDtos = _mapper.Map<IEnumerable<FavouritePostsDTO>>(favoritePosts);

                return Response<IEnumerable<FavouritePostsDTO>>.Success(favoritePostDtos,"",200);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<FavouritePostsDTO>>.Failure($"An error occurred while fetching favorite posts: {ex.Message}");
            }
        }
        #endregion

        #region Delete Favorite Post

public async Task<Response<string>> DeleteFavoritePostAsync(string userId, int? postId = null)
{
    try
    {
        
                if (string.IsNullOrEmpty(userId) || postId<=0 )
                 {
                     return Response<string>.Failure("Invalid Data Model", 400);
                 }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (user == null)
                {
                    return Response<string>.Failure("User Not Found", 404);
                }


                if (postId == null)
        {
            var favoritePosts = await _unitOfWork.FavoritePosts
                .GetAllByExpressionAsync(fp => fp.UserId == userId);

            if (!favoritePosts.Any())
            {
                return Response<string>.Failure("No favorite posts found for this user.", 404);
            }

            foreach (var favoritePost in favoritePosts)
            {
                await _unitOfWork.FavoritePosts.DeleteAsync(favoritePost);
            }
        }
        else
        {
            var favoritePost = await _unitOfWork.FavoritePosts
                .GetByExpressionAsync(fp => fp.UserId == userId && fp.PostId == postId);

            if (favoritePost == null)
            {
                return Response<string>.Failure("Favorite post not found.", 404);
            }

            await _unitOfWork.FavoritePosts.DeleteAsync(favoritePost);
        }

        await _unitOfWork.SaveAsync();

        return Response<string>.Success("", postId == null
            ? "All favorite posts deleted successfully."
            : "Favorite post deleted successfully.", 200);
    }
    catch (Exception ex)
    {
        return Response<string>.Failure($"An error occurred while deleting the favorite post(s): {ex.Message}");
    }
}

        #endregion

    }
}

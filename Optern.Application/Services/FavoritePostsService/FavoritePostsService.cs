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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
          
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(model.UserId) || model.PostId <= 0)
                {
                    return Response<string>.Failure("", "Invalid Data Model", 400);
                }
                var user = await _unitOfWork.Users.GetByIdAsync(model.UserId);
                var post = await _unitOfWork.Posts.GetByIdAsync(model.PostId);

                if (user == null || post == null)
                {
                    return Response<string>.Failure("","User or Post not found", 404);
                }

                var existingFavorite = await _unitOfWork.FavoritePosts
                    .GetByExpressionAsync(fp => fp.UserId == model.UserId && fp.PostId == model.PostId);
                if (existingFavorite != null)
                {
                    return Response<string>.Failure("","This post is already in your favorites.", 400);
                }

                var favoritePost = _mapper.Map<FavoritePosts>(model);
                favoritePost.SavedAt = DateTime.UtcNow;
                await _unitOfWork.FavoritePosts.AddAsync(favoritePost);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return Response<string>.Success("Post added to favorites successfully", "Post added to favorites successfully.", 200);
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return Response<string>.Failure("Database error occurred while adding the post to favorites.", dbEx.Message, 500);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<string>.Failure("An unexpected error occurred while adding the post to favorites.", ex.Message, 500);
            }
        }

        #endregion


        #region Get Favourite Posts 
        public async Task<Response<IEnumerable<PostDTO>>> GetFavoritePostsByUserAsync(string userId, int lastIdx = 0, int limit = 10)
        {
            try
            {

                var favoritePosts =  _unitOfWork.FavoritePosts.GetQueryable(
                    filter: fp => fp.UserId == userId,
                    includeProperties: "Post.Creator,Post.PostTags.Tag",
                    orderBy: q => q.OrderByDescending(fp => fp.SavedAt)
                    )
                  .Skip(lastIdx)
                  .Take(limit);


                if (!favoritePosts.Any())
                {
                    return Response<IEnumerable<PostDTO>>.Success(new List<PostDTO>(), "No favorite posts found for the user.", 404);
                }

                var favoritePostDtos = await favoritePosts.Select(f => new PostDTO
                {
                    Id = f.PostId,
                    Title =f.Post.Title,
                    Content = f.Post.Content,
                    CreatorName = $"{f.Post.Creator.FirstName} {f.Post.Creator.LastName}",
                    ProfilePicture = f.Post.Creator.ProfilePicture,
                    Tags = f.Post.PostTags.Select(pt => pt.Tag.Name).ToList(),
                    CreatedDate = f.Post.CreatedDate,
                    EditedDate = f.Post.EditedDate,
                    ReactsCount = f.Post.Reacts.Count,
                    CommentsCount = f.Post.Comments.Count
                }).ToListAsync();

                return Response<IEnumerable<PostDTO>>.Success(favoritePostDtos, "", 200);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<PostDTO>>.Failure($"An error occurred while fetching favorite posts: {ex.Message}");
            }
        }

        #endregion

        #region Delete Favorite Post

        public async Task<Response<string>> DeleteFavoritePostAsync(string userId, int? postId = null)
{
            await using var transaction = await _context.Database.BeginTransactionAsync();

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
        await transaction.CommitAsync();


                return Response<string>.Success("", postId == null
            ? "All favorite posts deleted successfully."
            : "Favorite post deleted successfully.", 200);
    }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return Response<string>.Failure("Database error occurred while Deleting the post to favorites.", dbEx.Message, 500);
            }
            catch (Exception ex)
    {
                await transaction.RollbackAsync();
                return Response<string>.Failure($"An error occurred while deleting the favorite post(s): {ex.Message}");
    }
}

        #endregion

    }
}

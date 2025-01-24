using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Optern.Application.DTOs.Comment;
using Optern.Application.DTOs.Post;
using Optern.Application.DTOs.React;
using Optern.Application.DTOs.Tags;
using Optern.Application.DTOs.Track;
using Optern.Application.Interfaces.IPostService;
using Optern.Application.Interfaces.ITrackService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Repositories;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Services.PostService
{
    public class PostService : GenericRepository<Post>, IPostService
    {
        private readonly IMapper _mapper;
        public PostService(OpternDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<Response<List<PostDTO>>> GetLatestPostsAsync(int count)
        {
            if (count <= 0)
                return Response<List<PostDTO>>.Failure("Invalid count value", 400);
            try
            {
                var latestPosts = await _dbContext.Posts
                    .Include(post => post.Creator)
                .OrderByDescending(post => post.CreatedDate)
                .Take(count)
                .ToListAsync();
                if (latestPosts != null && latestPosts.Any())
                {

                    var postDtos = _mapper.Map<List<PostDTO>>(latestPosts);

                    return Response<List<PostDTO>>.Success(postDtos, "Latest posts fetched successfully");
                }
                return Response<List<PostDTO>>.Failure("No Posts found!", 404);
            }
            catch (Exception ex)
            {
                return Response<List<PostDTO>>.Failure($"Failed to fetch latest posts: {ex.Message}");
            }
        
        }
        public async Task<Response<PostDTO>> GetPostByIdAsync(int id)
        {

            try
            {
                var post = await GetByIdAsync(id);
                if (post != null )
                {

                    var postDtos = _mapper.Map<PostDTO>(post);

                    return Response<PostDTO>.Success(postDtos, "post fetched successfully");
                }
                return Response<PostDTO>.Failure("post not found!", 404);
            }
            catch (Exception ex)
            {
                return Response<PostDTO>.Failure($"Failed to fetch latest posts: {ex.Message}");
            }

        }

        public async Task<Response<List<CommentDTO>>> GetCommentsByPostIdAsync(int postId)
        {
            try
            {
                var comments = await _dbContext.Comments
                    .Include(comment => comment.User)
                    .Where(comment => comment.PostId == postId)
                    .OrderBy(comment => comment.CommentDate)
                    .ToListAsync();

                if (comments == null || !comments.Any())
                    return Response<List<CommentDTO>>.Failure("No comments found for the specified post.", 404);

                var commentDtos = _mapper.Map<List<CommentDTO>>(comments);
                return Response<List<CommentDTO>>.Success(commentDtos, "Comments fetched successfully.");
            }
            catch (Exception ex)
            {
                return Response<List<CommentDTO>>.Failure($"Failed to fetch comments: {ex.Message}");
            }
        }

        public async Task<Response<List<ReactDTO>>> GetReactsByPostIdAsync(int postId)
        {
            try
            {
                var reacts = await _dbContext.Reacts
                    .Include(react => react.User)
                    .Where(react => react.PostId == postId)
                    .OrderBy(react => react.ReactDate)
                    .ToListAsync();

                if (reacts == null || !reacts.Any())
                    return Response<List<ReactDTO>>.Failure("No reacts found for the specified post.", 404);

                var reactDtos = _mapper.Map<List<ReactDTO>>(reacts);
                return Response<List<ReactDTO>>.Success(reactDtos, "Reacts fetched successfully.");
            }
            catch (Exception ex)
            {
                return Response<List<ReactDTO>>.Failure($"Failed to fetch reacts: {ex.Message}");
            }
        }

        public async Task<Response<List<PostDTO>>> GetRecommendedPostsAsync(int topN)
        {
            try
            {
                var recommendedPosts = await _dbContext.Posts
                    .Include(p => p.Reacts) 
                    .Include(p => p.PostTags) 
                    .OrderByDescending(p => p.Reacts.Count) 
                    .Take(topN) 
                    .ToListAsync(); 

                if (recommendedPosts.Any())
                {
                    var postDtos = _mapper.Map<List<PostDTO>>(recommendedPosts);
                    return Response<List<PostDTO>>.Success(postDtos, "Posts fetched successfully.");
                }

                return Response<List<PostDTO>>.Failure(new List<PostDTO>(),"No posts found.", 404);
            }
            catch (Exception ex)
            {
                return Response<List<PostDTO>>.Failure("Error occurred while fetching recommended posts.", 500, new List<string> { ex.Message });
            }
        }


    }
}

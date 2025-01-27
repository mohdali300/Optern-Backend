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
using Optern.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Services.PostService
{
    public class PostService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper) : IPostService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<List<PostDTO>>> GetLatestPostsAsync(int count)
        {
            if (count <= 0)
                return Response<List<PostDTO>>.Failure("Invalid count value", 400);
            try
            {
                var latestPosts = await _context.Posts
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

        public async Task<Response<PostWithDetailsDTO>> GetPostDetailsByIdAsync(int id)
        {
            try
            {

                var post = await _context.Posts
                    .Include(p => p.Creator)
                    .Include(p => p.Comments)
                        .ThenInclude(c => c.User)
                    .Include(p => p.Reacts)
                        .ThenInclude(r => r.User)
                    .Include(p => p.PostTags)
                    .SingleOrDefaultAsync(p => p.Id == id);


                if (post == null)
                {
                    return Response<PostWithDetailsDTO>.Failure("Post not found!", 404);
                }


                var postDetailsDto = _mapper.Map<PostWithDetailsDTO>(post);

                return Response<PostWithDetailsDTO>.Success(postDetailsDto, "Post details fetched successfully.");
            }
            catch (Exception ex)
            {

                return Response<PostWithDetailsDTO>.Failure($"Failed to fetch post details: {ex.Message}");
            }
        }


        #region Get Recommended Posts (Only Post Title  Required (post creator (Optoinal)))
        public async Task<Response<IEnumerable<PostDTO>>> GetRecommendedPostsAsync(int topN)
        {
            try
            {
                var recommendedPosts = await _context.Posts
                    .Include(p => p.Creator)
                    .OrderByDescending(p => p.Reacts.Count)
                    .Take(topN)
                    .ToListAsync();

                if (recommendedPosts.Any())
                {
                    var postDtos = _mapper.Map<IEnumerable<PostDTO>>(recommendedPosts);
                    return Response<IEnumerable<PostDTO>>.Success(postDtos, "Posts fetched successfully.");
                }

                return Response<IEnumerable<PostDTO>>.Failure(new List<PostDTO>(), "No posts found.", 404);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<PostDTO>>.Failure("Error occurred while fetching recommended posts.", 500, new List<string> { ex.Message });
            }
        }
        #endregion

        #region Search Posts By (Keyword , tag , Username)

        public async Task<Response<IEnumerable<SearchPostDTO>>> SearchPostsAsync(
      string? tagName = null,
      string? username = null,
      string? keyword = null)
        {
            try
            {
                var searchResults = new List<SearchPostDTO>();

                // Search by Tag Name
                if (!string.IsNullOrEmpty(tagName))
                {
                    var postsWithTags = await _context.PostTags
                        .Where(pt => EF.Functions.Like(pt.Tag.Name, $"%{tagName}%"))
                        .Include(pt => pt.Post)
                            .ThenInclude(p => p.Creator)
                        .Include(pt => pt.Post)
                            .ThenInclude(p => p.PostTags)
                                .ThenInclude(pt => pt.Tag)
                        .Select(pt => new SearchPostDTO
                        {
                            SourceType = "Post",
                            Highlight = $"Tag Name: {pt.Tag.Name}",
                            Data = new PostWithDetailsDTO
                            {
                                Id = pt.Post.Id,
                                Title = pt.Post.Title,
                                Content = pt.Post.Content,
                                CreatedDate = pt.Post.CreatedDate,
                                UserName = pt.Post.Creator.UserName,
                                Tags = pt.Post.PostTags.Select(tag => new TagDTO { Name = tag.Tag.Name }).ToList()
                            }
                        })
                        .ToListAsync();

                    searchResults.AddRange(postsWithTags);
                }

                // Search by Username
                if (!string.IsNullOrEmpty(username))
                {
                    var users = await _context.Users
                        .Where(u => EF.Functions.Like(u.UserName, $"%{username}%"))
                        .Include(u => u.CreatedPosts)
                        .ToListAsync();

                    searchResults.AddRange(users.SelectMany(u => u.CreatedPosts.Select(post => new SearchPostDTO
                    {
                        SourceType = "User",
                        Highlight = $"Username: {u.UserName}",
                        Data = new PostWithDetailsDTO
                        {
                            Id = post.Id,
                            Title = post.Title,
                            Content = post.Content,
                            CreatedDate = post.CreatedDate,
                            UserName = u.UserName
                        }
                    })));
                }

                // Search by Keyword in Posts
                if (!string.IsNullOrEmpty(keyword))
                {
                    var posts = await _context.Posts
                        .Where(p => EF.Functions.Like(p.Title, $"%{keyword}%") || EF.Functions.Like(p.Content, $"%{keyword}%"))
                        .Include(p => p.Creator)
                        .Include(p => p.PostTags)
                            .ThenInclude(pt => pt.Tag)
                        .Select(p => new SearchPostDTO
                        {
                            SourceType = "Post",
                            Highlight = $"Post Title: {p.Title}",
                            Data = new PostWithDetailsDTO
                            {
                                Id = p.Id,
                                Title = p.Title,
                                Content = p.Content,
                                CreatedDate = p.CreatedDate,
                                UserName = p.Creator.UserName,
                                Tags = p.PostTags.Select(pt => new TagDTO { Name = pt.Tag.Name }).ToList()
                            }
                        })
                        .ToListAsync();

                    searchResults.AddRange(posts);
                }

                searchResults = searchResults
                    .OrderByDescending(r => r.Data?.CreatedDate) 
                    .ToList();

                if (!searchResults.Any())
                {
                    return Response<IEnumerable<SearchPostDTO>>.Success(
                        new List<SearchPostDTO>(),
                        "No matching results found for the provided criteria.",
                        200
                    );
                }

                return Response<IEnumerable<SearchPostDTO>>.Success(
                    searchResults,
                    "Search results fetched successfully.",
                    200
                );
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<SearchPostDTO>>.Failure(
                    $"An error occurred while performing the search: {ex.Message}",
                    500
                );
            }
        }

        #endregion
    }
}

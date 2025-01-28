using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Optern.Application.DTOs.Comment;
using Optern.Application.DTOs.Post;
using Optern.Application.DTOs.React;
using Optern.Application.DTOs.Room;
using Optern.Application.DTOs.Tags;
using Optern.Application.DTOs.Track;
using Optern.Application.Interfaces.IPostService;
using Optern.Application.Interfaces.ITrackService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Repositories;
using Optern.Infrastructure.Response;
using Optern.Infrastructure.UnitOfWork;
using Optern.Infrastructure.Validations;
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

        #region Get Latest Posts

        public async Task<Response<IEnumerable<PostDTO>>> GetLatestPostsAsync(int count)
        {
            if (count <= 0)
                return Response<IEnumerable<PostDTO>>.Failure("Invalid count value", 400);

            try
            {
                var latestPosts = await _context.Posts
                    .Include(post => post.Creator)
                    .Include(post => post.PostTags)
                        .ThenInclude(postTag => postTag.Tag)
                    .OrderByDescending(post => post.CreatedDate)
                    .Take(count)
                    .ToListAsync();

                if (latestPosts != null && latestPosts.Any())
                {
                    var postDtos = _mapper.Map<List<PostDTO>>(latestPosts);

                    return Response<IEnumerable<PostDTO>>.Success(postDtos, "Latest posts fetched successfully");
                }

                return Response<IEnumerable<PostDTO>>.Failure("No Posts found!", 404);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<PostDTO>>.Failure($"Failed to fetch latest posts: {ex.Message}", 500);
            }
        }
        #endregion

        #region Get Posts (id - name -all)
        public async Task<Response<IEnumerable<PostWithDetailsDTO>>> GetPostsByIdOrUserAsync(int? postId = null, string? username =null)
        {
            try
            {
                var query = _context.Posts
                    .Include(p => p.Creator)
                    .Include(p => p.PostTags)
                        .ThenInclude(pt => pt.Tag)
                    .Include(p => p.Comments)
                        .ThenInclude(c => c.CommentReacts)
                    .Include(p => p.Reacts) 
                    .AsQueryable();

                if (!string.IsNullOrEmpty(username))
                {
                    query = query.Where(p => p.Creator.UserName == username);
                }

                if (postId.HasValue)
                {
                    query = query.Where(p => p.Id == postId);
                }

                query = query.OrderByDescending(p => p.CreatedDate);

                var posts = await query.ToListAsync();
                

                var postDetails = posts.Select(p => new PostWithDetailsDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    CreatedDate = p.CreatedDate,
                    UserName = p.Creator.UserName,
                    // Reacts for the post itself
                    Reacts = p.Reacts.Select(r => new ReactDTO
                    {
                        UserId = r.UserId,
                        ReactType = r.ReactType,
                        UserName = r.User.UserName
                    }).ToList(),
                    Tags = p.PostTags.Select(pt => new TagDTO { Name = pt.Tag.Name }).ToList(),
                    ReactCount = p.Reacts.Count(),
                    CommentCount = p.Comments.Count(),
                    // Comments with reacts
                    Comments = p.Comments.Select(c => new CommentDTO
                    {
                        Id = c.Id,
                        Content = c.Content,
                        CommentDate = c.CommentDate,
                        UserName = c.User.UserName,
                        ReactCommentCount = c.CommentReacts.Count, 
                       // ReplyCommentCount = c.Replies.Count,
                        // Reacts for the comment itself
                        Reacts = c.CommentReacts.Select(r => new ReactDTO
                        {
                            UserId = r.UserId,
                            ReactType = r.ReactType,
                            UserName = r.User.UserName
                        }).ToList()
                    }).ToList()  
                }).ToList();
                

              //  var postDetails = _mapper.Map<List<PostWithDetailsDTO>>(posts);


                if (!postDetails.Any())
                {
                    return Response<IEnumerable<PostWithDetailsDTO>>.Success(
                        new List<PostWithDetailsDTO>(),
                        "No posts found",
                        200
                    );
                }

                return Response<IEnumerable<PostWithDetailsDTO>>.Success(
                    postDetails,
                    "Posts retrieved successfully.",
                    200
                );
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<PostWithDetailsDTO>>.Failure(
                    $"An error occurred while retrieving posts: {ex.Message}",
                    500
                );
            }
        }
        #endregion


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


        #region Add Post


        public async Task<Response<PostDTO>> CreatePostAsync(string userId, ManagePostDTO model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.Content))
                {
                    return Response<PostDTO>.Failure(new PostDTO(),"Title and Content cannot be empty.", 400);
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<PostDTO>.Failure(new PostDTO(),"User not found.", 404);
                }

                var post = new Post
                {
                    Title = model.Title,
                    Content = model.Content,
                    CreatorId = user.Id,
                    CreatedDate = DateTime.UtcNow,
                    ContentType = Domain.Enums.ContentType.Text
                };
                var validate = new PostValidator().Validate(post);

                if (!validate.IsValid)
                {
                    var errorMessages = string.Join(", ", validate.Errors.Select(e => e.ErrorMessage));
                    return Response<PostDTO>.Failure(new PostDTO(), $"Invalid Data Model: {errorMessages}", 400);
                }
                await _unitOfWork.Posts.AddAsync(post);
                await _unitOfWork.SaveAsync();

                var postTags = new List<PostTags>();
                if (model.Tags != null && model.Tags.Any())
                {
                    var distinctTags = model.Tags.Select(tag => tag.Trim().ToLowerInvariant()).Distinct();

                    foreach (var tagName in distinctTags)
                    {
                        var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                        if (tag == null)
                        {
                            tag = new Tags { Name = tagName };
                            await _unitOfWork.Tags.AddAsync(tag);
                            await _unitOfWork.SaveAsync();
                        }

                        postTags.Add(new PostTags
                        {
                            PostId = post.Id,
                            TagId = tag.Id
                        });
                    }

                    await _unitOfWork.PostTags.AddRangeAsync(postTags);
                    await _unitOfWork.SaveAsync();
                }

                await transaction.CommitAsync();

                var postResponse = new PostDTO
                {
                    Title = post.Title ?? string.Empty,
                    Content = post.Content ?? string.Empty,
                    Tags = postTags.Select(pt => pt.Tag.Name).ToList() ?? new List<string>(),
                    CreatedDate = post.CreatedDate,
                    ProfilePicture = user.ProfilePicture ?? string.Empty,
                    CreatorName = $"{user.FirstName ?? ""} {user.LastName ?? ""}",
                    ReactsCount = 0,
                    CommentsCount = 0
                };

                return Response<PostDTO>.Success(postResponse ?? new PostDTO(), "Post created successfully.", 201);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return Response<PostDTO>.Failure($"Database error: {ex.Message}", 500);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<PostDTO>.Failure($"Server error: {ex.Message}", 500);
            }
        }


        #endregion


        #region Delete Post
        public async Task<Response<string>> DeletePostAsync(int postId, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var post = await _unitOfWork.Posts.GetByIdAsync(postId);
                if (post == null)
                {
                    return Response<string>.Failure("", "Post not found", 404);
                }

                if (post.CreatorId != userId)
                {
                    return Response<string>.Failure("","You are not authorized to delete this post", 403);
                }

                await _unitOfWork.Posts.DeleteAsync(post);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return Response<string>.Success("Post deleted successfully", "Post deleted successfully", 200);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return Response<string>.Failure($"Database error: {ex.Message}", 500, new List<string> { "Database error occurred" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<string>.Failure($"Server error: {ex.Message}", 500, new List<string> { "Server error occurred" });
            }

        }
        #endregion

        #region Edit Post
        public async Task<Response<PostDTO>> EditPostAsync(int postId, string userId, ManagePostDTO model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var post = await _context.Posts
                     .Include(p => p.Creator)
                     .Include(p => p.PostTags)
                     .ThenInclude(pt => pt.Tag)
                     .FirstOrDefaultAsync(p => p.Id == postId);

                if (post == null)
                {
                    return Response<PostDTO>.Failure(new PostDTO(), "Post not found", 404);
                }

                if (post.Creator == null)
                {
                    return Response<PostDTO>.Failure(new PostDTO(), "Post creator is missing", 400);
                }

                if (post.CreatorId != userId)
                {
                    return Response<PostDTO>.Failure(new PostDTO(), "You are not authorized to edit this post", 403);
                }

                bool isUpdated = false;

                if (!string.IsNullOrEmpty(model.Title) && model.Title != post.Title)
                {
                    post.Title = model.Title;
                    isUpdated = true;
                }

                if (!string.IsNullOrEmpty(model.Content) && model.Content != post.Content)
                {
                    post.Content = model.Content;
                    isUpdated = true;
                }

                var validate = new PostValidator().Validate(post);
                if (!validate.IsValid)
                {
                    var errorMessages = string.Join(", ", validate.Errors.Select(e => e.ErrorMessage));
                    return Response<PostDTO>.Failure(new PostDTO(), $"Invalid Data Model: {errorMessages}", 400);
                }

                if (post.PostTags == null)
                {
                    post.PostTags = new List<PostTags>();
                }

                if (model.Tags != null)
                {
                    var existingTags = post.PostTags.Select(pt => pt.Tag).ToList();
                    var tagsToAdd = model.Tags.Except(existingTags.Select(t => t.Name)).ToList();
                    var tagsToRemove = existingTags.Where(et => !model.Tags.Contains(et.Name)).ToList();

                    foreach (var tagName in tagsToAdd)
                    {
                        var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                        if (tag == null)
                        {
                            tag = new Tags { Name = tagName };
                            await _unitOfWork.Tags.AddAsync(tag);
                            await _unitOfWork.SaveAsync();
                        }

                        post.PostTags.Add(new PostTags { PostId = post.Id, TagId = tag.Id });
                        isUpdated = true;
                    }

                    foreach (var tagToRemove in tagsToRemove)
                    {
                        var postTagToRemove = post.PostTags.FirstOrDefault(pt => pt.TagId == tagToRemove.Id);
                        if (postTagToRemove != null)
                        {
                            post.PostTags.Remove(postTagToRemove);
                            isUpdated = true;
                        }
                    }
                }

                if (isUpdated)
                {
                    post.EditedDate = DateTime.UtcNow;
                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();
                }

                var postDTO = new PostDTO
                {
                    Id = post.Id,
                    Title = post.Title ?? string.Empty,
                    Content = post.Content ?? string.Empty,
                    Tags = post.PostTags?.Select(pt => pt.Tag.Name).ToList() ?? new List<string>(),
                    CreatedDate = post.CreatedDate,
                    EditedDate = post.EditedDate,
                    ProfilePicture = post.Creator?.ProfilePicture ?? string.Empty,
                    CreatorName = $"{post.Creator?.FirstName ?? ""} {post.Creator?.LastName ?? ""}",
                    ReactsCount = post.Reacts?.Count() ?? 0, 
                    CommentsCount = post.Comments?.Count() ?? 0  
                };

                return Response<PostDTO>.Success(postDTO, "Post edited successfully.", 200);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return Response<PostDTO>.Failure(new PostDTO(), $"Database error: {ex.Message}", 500);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<PostDTO>.Failure(new PostDTO(), $"Server error: {ex.Message}", 500);
            }
        }

        #endregion

    }
}

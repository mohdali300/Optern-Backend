using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs.Comment;
using Optern.Application.DTOs.Post;
using Optern.Application.DTOs.Tags;
using Optern.Application.Interfaces.IPostService;
using Optern.Domain.Entities;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Response;
using Optern.Infrastructure.UnitOfWork;
using Optern.Infrastructure.Validations;


namespace Optern.Application.Services.PostService
{
	public class PostService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper) : IPostService
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly OpternDbContext _context = context;
		private readonly IMapper _mapper = mapper;

        #region Get Latest
        public async Task<Response<IEnumerable<PostDTO>>> GetLatestPostsAsync(string? userId = null,int lastIdx = 0,int limit = 10)
		{
			try
			{
				var query = _context.Posts
					.Include(p => p.Creator)
					.Include(p => p.PostTags)
						.ThenInclude(pt => pt.Tag)
					.AsQueryable();

				if (!string.IsNullOrEmpty(userId))
				{
					query = query.Where(p => p.Creator.Id == userId);
				}
				
				

				query = query.OrderByDescending(p => p.CreatedDate);

				var posts = await query.Select(p => new PostDTO
									   {
										   Id = p.Id,
										   Title = p.Title,
										   Content = p.Content,
										   CreatorName = $"{p.Creator.FirstName} {p.Creator.LastName}",
										   ProfilePicture = p.Creator.ProfilePicture, 
										   Tags = p.PostTags.Select(pt => pt.Tag.Name).ToList(),
										   CreatedDate = p.CreatedDate,
										   EditedDate=p.EditedDate,
										   ReactsCount = p.Reacts.Count,
										   CommentsCount = p.Comments.Count
									   })
									   .ToListAsync();

				if (!posts.Any())
				{
					return Response<IEnumerable<PostDTO>>.Success(
						new List<PostDTO>(),
						"No posts found.",
						200
					);
				}
				
				
				posts = posts.Skip(lastIdx).Take(limit).ToList();
			
				
				return Response<IEnumerable<PostDTO>>.Success(
					posts,
					"Posts retrieved successfully.",
					200
				);
			}
			catch (Exception ex)
			{
				return Response<IEnumerable<PostDTO>>.Failure(
					new List<PostDTO>(),
					$"An error occurred while retrieving posts: {ex.Message}",
					500
				);
			}
		}
        #endregion
        #region Get Post
        public async Task<Response<PostWithDetailsDTO>> GetPostByIdAsync(int postId,string? userId = null)
		{
			try
			{
				var post = await _context.Posts
					.Include(p => p.Creator)
					.Include(p => p.PostTags)
						.ThenInclude(pt => pt.Tag)
					.Include(p => p.Reacts)
					.Where(p => p.Id == postId)
					.Select(p => new PostWithDetailsDTO
					{
						Id = p.Id,
						Title = p.Title,
						Content = p.Content,
						CreatedDate = p.CreatedDate,
						UserId=p.CreatorId,
						UserName = $"{p.Creator.FirstName} {p.Creator.LastName}",
						ProfilePicture = p.Creator.ProfilePicture,
						Tags = p.PostTags.Select(pt => new TagDTO {Id=pt.Tag.Id, Name = pt.Tag.Name }).ToList(),
						ReactCount = p.Reacts.Count,
						CommentCount = _context.Comments.Count(c => c.PostId == p.Id),
						UserReact = ReactType.NOTVOTEYET,
                         IsFav = false
                    })
					.FirstOrDefaultAsync();

				if (post == null)
				{
					return Response<PostWithDetailsDTO>.Failure(
						new PostWithDetailsDTO(),
						"Post not found.",
						404
					);
				}

				var parentComments = await _context.Comments
					.Include(c => c.User)
					.Include(c => c.CommentReacts)
					.Where(c => c.PostId == postId && c.ParentId == null) 
					.OrderBy(c => c.CommentDate)
					.ToListAsync();

				post.Comments = parentComments.Select(parent => new CommentDTO
				{
					Id = parent.Id,
					Content = parent.Content,
					CommentDate = parent.CommentDate,
					UserId = parent.UserId,
					UserName = $"{parent.User?.FirstName} {parent.User?.LastName}",
					ProfilePicture = parent.User?.ProfilePicture,
					ReactCommentCount = parent.CommentReacts.Count(),
					ReplyCommentCount = _context.Comments.Count(c => c.ParentId == parent.Id),
                    UserVote = userId != null ? parent.CommentReacts.FirstOrDefault(r => r.UserId == userId)?.ReactType ?? ReactType.NOTVOTEYET : ReactType.NOTVOTEYET


                }).ToList();
				
				if(userId is not null)
				{
					var react = await _context.Reacts.Where(r => r.UserId == userId && r.PostId == postId).Select(r=>r.ReactType).ToListAsync();
					
					if(react is not null && react.Any())
					{
						post.UserReact = react[0];
					}
                    post.IsFav = await _context.FavoritePosts
               .AnyAsync(fp => fp.UserId == userId && fp.PostId == postId);
                }

				return Response<PostWithDetailsDTO>.Success(
					post,
					"Post retrieved successfully.",
					200
				);
			}
			catch (Exception ex)
			{
				return Response<PostWithDetailsDTO>.Failure(
					new PostWithDetailsDTO(),
					$"An error occurred while retrieving the post: {ex.Message}",
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
    string? keyword = null,
    int lastIdx = 0, int limit = 10)
        {
            try
            {
                var searchResults = new List<SearchPostDTO>();

                var commentCounts = await _context.Comments
                    .GroupBy(c => c.PostId)
                    .Select(g => new { PostId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(g => g.PostId, g => g.Count);

                var tasks = new List<Task<List<SearchPostDTO>>>();

                // Search by Tag Name
                if (!string.IsNullOrEmpty(tagName))
                {
                    tasks.Add(_context.PostTags
                        .Where(pt => EF.Functions.Like(pt.Tag.Name, $"%{tagName}%"))
                        .Include(pt => pt.Post)
                            .ThenInclude(p => p.Creator)
                        .Include(pt => pt.Post.PostTags)
                            .ThenInclude(pt => pt.Tag)
                        .Include(pt => pt.Post.Reacts)
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
                                UserName = $"{pt.Post.Creator.FirstName} {pt.Post.Creator.LastName}",
                                ProfilePicture = pt.Post.Creator.ProfilePicture,
                                Tags = pt.Post.PostTags.Select(tag => new TagDTO { Id = tag.Id, Name = tag.Tag.Name }).ToList(),
                                ReactCount = pt.Post.Reacts.Count,
                                CommentCount = commentCounts.ContainsKey(pt.Post.Id) ? commentCounts[pt.Post.Id] : 0
                            }
                        }).Skip(lastIdx)
                          .Take(limit)
						  .ToListAsync());
                }

                // Search by Username (FirstName + LastName)
          
                if (!string.IsNullOrEmpty(username))
                {
                    var user = await _unitOfWork.Users
                        .GetByExpressionAsync(u =>
                            (u.FirstName.ToLower() + " " + u.LastName.ToLower()) == username.ToLower());

                    if (user == null)
                    {
                        return Response<IEnumerable<SearchPostDTO>>.Success(
                            new List<SearchPostDTO>(),
                            $"User with username '{username}' does not exist.",
                            404
                        );
                    }

                    // Then, search for posts by the user
                    var userPostsQuery = _context.Users
                        .Where(u => EF.Functions.Like((u.FirstName + " " + u.LastName).ToLower(), $"%{username.ToLower()}%"))
                        .Include(u => u.CreatedPosts)
                            .ThenInclude(p => p.Creator)
                        .Include(u => u.CreatedPosts)
                            .ThenInclude(p => p.Reacts)
                        .Include(u => u.CreatedPosts)
                            .ThenInclude(p => p.PostTags)
                            .ThenInclude(pt => pt.Tag)
                        .SelectMany(u => u.CreatedPosts)
                         .Skip(lastIdx)
                         .Take(limit)
                        .AsQueryable(); 

                    var userPosts = await userPostsQuery.ToListAsync(); 

                    if (userPosts == null || !userPosts.Any())
                    {
                        return Response<IEnumerable<SearchPostDTO>>.Success(
                            new List<SearchPostDTO>(),
                            "No posts found for the specified user.",
                            404
                        );
                    }

                    searchResults.AddRange(userPosts.Select(post => new SearchPostDTO
                    {
                        SourceType = "User",
                        Highlight = $"Username: {user.UserName}",
                        Data = new PostWithDetailsDTO
                        {
                            Id = post.Id,
                            Title = post.Title,
                            Content = post.Content,
                            CreatedDate = post.CreatedDate,
                            UserName = user.UserName,
                            ProfilePicture = post.Creator.ProfilePicture,
                            Tags = post.PostTags.Select(tag => new TagDTO { Id = tag.Id, Name = tag.Tag.Name }).ToList(),
                            ReactCount = post.Reacts.Count,
                            CommentCount = commentCounts.ContainsKey(post.Id) ? commentCounts[post.Id] : 0
                        }
                    }));
                }


                // Search by Keyword
                if (!string.IsNullOrEmpty(keyword))
                {
                    tasks.Add(_context.Posts
                        .Where(p => EF.Functions.Like(p.Title, $"%{keyword}%") || EF.Functions.Like(p.Content, $"%{keyword}%"))
                        .Include(p => p.Creator)
                        .Include(p => p.PostTags)
                            .ThenInclude(pt => pt.Tag)
                        .Include(p => p.Reacts)
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
                                UserName = $"{p.Creator.FirstName} {p.Creator.LastName}",
                                ProfilePicture = p.Creator.ProfilePicture,
                                Tags = p.PostTags.Select(pt => new TagDTO { Id = pt.Tag.Id, Name = pt.Tag.Name }).ToList(),
                                ReactCount = p.Reacts.Count,
                                CommentCount = commentCounts.ContainsKey(p.Id) ? commentCounts[p.Id] : 0
                            }
                        }).Skip(lastIdx)
                          .Take(limit)
						  .ToListAsync());
                }



                var results = await System.Threading.Tasks.Task.WhenAll(tasks);

                searchResults.AddRange(results.SelectMany(r => r));

                searchResults = searchResults
                    .OrderByDescending(x => x.Data?.CreatedDate)
                    .ToList();

                return Response<IEnumerable<SearchPostDTO>>.Success(searchResults, "Search completed successfully.");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<SearchPostDTO>>.Failure($"An error occurred: {ex.Message}", 500);
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
					Id = post.Id,
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

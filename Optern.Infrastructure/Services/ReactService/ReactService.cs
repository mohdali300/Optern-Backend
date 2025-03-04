
namespace Optern.Infrastructure.Services.ReactService
{
	public class ReactService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper) :IReactService
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly OpternDbContext _context = context;
		private readonly IMapper _mapper = mapper;

        #region Manage React
        public async Task<Response<ReactDTO>> ManageReactAsync(int postId, string userId, ReactType reactType)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				// Check if the post exists
				var post = await _context.Posts.FindAsync(postId);
				if (post == null)
				{
					return Response<ReactDTO>.Failure(new ReactDTO(), "Post not found.", 404);
				}

				var existingReact = await _context.Reacts
					.Include(r => r.User)
					.FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);

				if (existingReact != null)
				{
					if (existingReact.ReactType == reactType)
					{
						// Remove 
						_context.Reacts.Remove(existingReact);
						await _context.SaveChangesAsync();
						await transaction.CommitAsync();

						var removedReactDTO = _mapper.Map<ReactDTO>(existingReact);
						removedReactDTO.ReactType = ReactType.NOTVOTEYET;
						return Response<ReactDTO>.Success(removedReactDTO, "React removed successfully.",200);
					}
					else
					{
						// Update 
						existingReact.ReactType = reactType;
						existingReact.ReactDate = DateTime.UtcNow;
						_context.Reacts.Update(existingReact);
						await _context.SaveChangesAsync();
						await transaction.CommitAsync();

						
						var updatedReactDTO = _mapper.Map<ReactDTO>(existingReact);
						return Response<ReactDTO>.Success(updatedReactDTO, "React updated successfully.",200);
					}
				}

				// Create 
				var newReact = new Reacts
				{
					PostId = postId,
					UserId = userId,
					ReactType = reactType,
					ReactDate = DateTime.UtcNow,
					User = await _context.Users.FindAsync(userId)
				};

				_context.Reacts.Add(newReact);
				await _context.SaveChangesAsync();

				await transaction.CommitAsync();

				var newReactDTO = _mapper.Map<ReactDTO>(newReact);
				return Response<ReactDTO>.Success(newReactDTO, "React added successfully.",200);
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				return Response<ReactDTO>.Failure(new ReactDTO(), $"An error occurred while processing the react: {ex.Message}");
			}
		}
        #endregion

        #region Manage Comment React 
        public async Task<Response<CommentReactDTO>> ManageCommentReactAsync(int commentId, string userId, ReactType reactType)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var comment = await _context.Comments.FindAsync(commentId);
				if (comment == null)
				{
					return Response<CommentReactDTO>.Failure(new CommentReactDTO(), "Comment not found.", 404);
				}

				var existingReact = await _context.CommentReacts
					.Include(cr => cr.User) 
					.FirstOrDefaultAsync(cr => cr.CommentId == commentId && cr.UserId == userId);

				if (existingReact != null)
				{
					//delete
					if (existingReact.ReactType == reactType)
					{
						_context.CommentReacts.Remove(existingReact);
						await _context.SaveChangesAsync();
						await transaction.CommitAsync();

						var dto = new CommentReactDTO
						{
							ReactType = existingReact.ReactType,
							UserId = existingReact.UserId,
							UserName = existingReact.User?.UserName,
						};
						return Response<CommentReactDTO>.Success(dto, "React removed successfully.",200);
					}
					else
					{
						// Update 
						existingReact.ReactType = reactType;
						_context.CommentReacts.Update(existingReact);
						await _context.SaveChangesAsync();
						await transaction.CommitAsync();

						var dto = new CommentReactDTO
						{
							ReactType = existingReact.ReactType,
							UserId = existingReact.UserId,
							UserName = existingReact.User?.UserName,
						};
						return Response<CommentReactDTO>.Success(dto, "React updated successfully.",200);
					}
				}

				// Create 
				var newReact = new CommentReacts
				{
					CommentId = commentId,
					UserId = userId,
					ReactType = reactType
				};

				_context.CommentReacts.Add(newReact);
				await _context.SaveChangesAsync();

				var user = await _context.Users.FindAsync(userId);

				await transaction.CommitAsync();

				var newDto = new CommentReactDTO
				{
					ReactType = newReact.ReactType,
					UserId = newReact.UserId,
					UserName = user?.UserName
				};

				return Response<CommentReactDTO>.Success(newDto, "React added successfully.",201);
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				return Response<CommentReactDTO>.Failure(new CommentReactDTO(), $"An error occurred while processing the comment react: {ex.Message}");
			}
		}
        #endregion

        #region Get Reacts
        public async Task<Response<List<ReactDTO>>> GetReactsAsync(int postId, ReactType? reactType = null)
		{
			try
			{
				var postExists = await _context.Posts.AnyAsync(p => p.Id == postId);
				if (!postExists)
				{
					return Response<List<ReactDTO>>.Failure(new List<ReactDTO>(), "Post not found.",404);
				}

				var query = _context.Reacts
					.Include(r => r.User)
					.Where(r => r.PostId == postId) 
					.AsQueryable();

				if (reactType.HasValue)
				{
					query = query.Where(r => r.ReactType == reactType.Value);
				}

				query = query.OrderByDescending(r => r.ReactDate);

				var reacts = await query.ToListAsync();

				if (!reacts.Any())
				{
					return Response<List<ReactDTO>>.Success(new List<ReactDTO>(), "No reacts found for the specified criteria.",204);
				}

				var reactDTOs = _mapper.Map<List<ReactDTO>>(reacts);

				return Response<List<ReactDTO>>.Success(reactDTOs, "Reacts retrieved successfully.",200);
			}
			catch (Exception ex)
			{
			   
				return Response<List<ReactDTO>>.Failure(new List<ReactDTO>(), "An error occurred while retrieving reacts.");
			}
		}
        #endregion

        #region Get Comment Reacts 
        public async Task<Response<List<CommentReactDTO>>> GetCommentReactsAsync(int commentId, ReactType? reactType = null)
		{
			try
			{
				var commentExists = await _context.Comments.AnyAsync(c => c.Id == commentId);
				if (!commentExists)
				{
					return Response<List<CommentReactDTO>>.Failure(new List<CommentReactDTO>(), "Comment not found.",404);
				}

				var query = _context.CommentReacts
					.Include(cr => cr.User)
					.Where(cr => cr.CommentId == commentId) 
					.AsQueryable();

				if (reactType.HasValue)
				{
					query = query.Where(cr => cr.ReactType == reactType.Value);
				}


				var commentReacts = await query.ToListAsync();

				if (!commentReacts.Any())
				{
					return Response<List<CommentReactDTO>>.Success(new List<CommentReactDTO>(), "No reacts found for the specified criteria.",204);
				}

				var commentReactDTOs = _mapper.Map<List<CommentReactDTO>>(commentReacts);

				return Response<List<CommentReactDTO>>.Success(commentReactDTOs, "Comment reacts retrieved successfully.",200);
			}
			catch (Exception ex)
			{
				return Response<List<CommentReactDTO>>.Failure(new List<CommentReactDTO>(), "An error occurred while retrieving comment reacts.",500);
			}
		}
        #endregion


    }
}

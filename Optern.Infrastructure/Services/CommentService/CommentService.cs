
namespace Optern.Infrastructure.Services.CommentService
{
    public class CommentService(IUnitOfWork unitOfWork, OpternDbContext context) : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork= unitOfWork;
        private readonly OpternDbContext _context= context;

        #region Get Replies For Comment
        public async Task<Response<List<CommentDTO>>> GetRepliesForCommentAsync(int commentId,string userId)
        {
            try
            {
                var targetComment = await _context.Comments
                    .Include(comment => comment.User)
                    .FirstOrDefaultAsync(comment => comment.Id == commentId);

                if (targetComment == null)
                    return Response<List<CommentDTO>>.Failure(new List<CommentDTO>(), "Comment not found.", 404);

                var allComments = await _context.Comments
                    .Include(comment => comment.User)
                    .Include(comment=>comment.CommentReacts)
                    .Where(comment => comment.ParentId == commentId)
                    .OrderBy(comment => comment.CommentDate)
                    .ToListAsync();

				var commentDTOs = allComments.Select(comment => new CommentDTO
				{
					Id = comment.Id,
					Content = comment.Content,
					CommentDate = comment.CommentDate,
					UserName = $"{comment.User?.FirstName} {comment.User?.LastName}",
					ProfilePicture=comment.User?.ProfilePicture,
					ReplyCommentCount = _context.Comments.Count(comment => comment.ParentId == commentId),
					ReactCommentCount = comment.CommentReacts.Count(r => r.ReactType == ReactType.VOTEUP) - comment.CommentReacts.Count(r => r.ReactType == ReactType.VOTEDOWN),
					UserVote = userId != null ? comment.CommentReacts.FirstOrDefault(r => r.UserId == userId && r.CommentId == comment.Id)?.ReactType ?? ReactType.NOTVOTEYET : ReactType.NOTVOTEYET
				}).ToList();

                return Response<List<CommentDTO>>.Success(commentDTOs, "Replies for the comment fetched successfully.");
            }
            catch (Exception ex)
            {
                return Response<List<CommentDTO>>.Failure(new List<CommentDTO>(), $"Failed to fetch replies for the comment: {ex.Message}", 500);
            }
        }
        #endregion

        #region Add Comment
        public async Task<Response<CommentDTO>> AddCommentAsync(AddCommentInputDTO input, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            if (string.IsNullOrEmpty(userId) || input == null) 
            {
                return Response<CommentDTO>.Failure(new CommentDTO(), $"Invalid Data",400);
            }
            var isUserExist = await _context.Users.FirstOrDefaultAsync(u=>u.Id==userId);
            if (isUserExist == null)
            {
                return Response<CommentDTO>.Failure(new CommentDTO(), $"User Not Found", 404);
            }
            try
            {
                var newComment = new Comment
                {
                    PostId = input.PostId,
                    Content = input.Content,
                    CommentDate = DateTime.UtcNow,
                    UserId = userId
                };

                _context.Comments.Add(newComment);
                await _context.SaveChangesAsync();

                var commentDto = new CommentDTO
                {
                    Id = newComment.Id,
                    Content = newComment.Content,
                    CommentDate = newComment.CommentDate,
                    UserName = (await _context.Users.FindAsync(userId))?.FirstName + " " + (await _context.Users.FindAsync(userId))?.LastName
                };

                await transaction.CommitAsync();

                return Response<CommentDTO>.Success(commentDto, "Comment added successfully.",201);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<CommentDTO>.Failure(new CommentDTO(), $"Error adding comment: {ex.Message}",500);
            }
        }
        #endregion

        #region Add Reply
        public async Task<Response<CommentDTO>> AddReplyAsync(AddReplyInputDTO input, string userId)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();
            if (string.IsNullOrEmpty(userId) || input == null)
            {
                return Response<CommentDTO>.Failure(new CommentDTO(), $"Invalid Data", 400);
            }
            try
            {
                var parentComment = await _context.Comments.FindAsync(input.ParentId);
                if (parentComment == null)
                {
                    return Response<CommentDTO>.Failure(new CommentDTO(), "Parent comment not found.",404);
                }

                var newReply = new Comment
                {
                    PostId = input.PostId,
                    Content = input.Content,
                    CommentDate = DateTime.UtcNow,
                    UserId = userId,
                    ParentId = input.ParentId
                };

                _context.Comments.Add(newReply);
                await _context.SaveChangesAsync();

                var replyDto = new CommentDTO
                {
                    Id = newReply.Id,
                    Content = newReply.Content,
                    CommentDate = newReply.CommentDate,
                    UserName = (await _context.Users.FindAsync(userId))?.UserName,
                };

                await transaction.CommitAsync();

                return Response<CommentDTO>.Success(replyDto, "Reply added successfully.",201);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<CommentDTO>.Failure(new CommentDTO(), $"Error adding reply: {ex.Message}");
            }
        }
        #endregion

        #region Update Comment
        public async Task<Response<CommentDTO>> UpdateCommentAsync(int commentId, UpdateCommentInputDTO input)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                
                var comment = await _context.Comments
                    .Include(c => c.User) 
                    .FirstOrDefaultAsync(c => c.Id == commentId);

                if (comment == null)
                {
                    return Response<CommentDTO>.Failure(new CommentDTO(),"Comment not found.", 404);
                }

                
                comment.Content = input.UpdatedContent;
                comment.CommentDate = DateTime.UtcNow;

                _context.Comments.Update(comment);
                await _context.SaveChangesAsync();

                
                await transaction.CommitAsync();

                
                var updatedCommentDto = new CommentDTO
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    CommentDate = comment.CommentDate,
                    UserName = comment.User.UserName
                };

                return Response<CommentDTO>.Success(updatedCommentDto, "Comment updated successfully.",200);
            }
            catch (Exception ex)
            {
                
                await transaction.RollbackAsync();
                return Response<CommentDTO>.Failure(new CommentDTO(),$"Failed to update comment: {ex.Message}",500);
            }
        }
        #endregion

        #region Delete Comment
        public async Task<Response<bool>> DeleteCommentAsync(int commentId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var comment = await _context.Comments
                    .Include(c => c.comment) // Include replies to cascade deletion
                    .FirstOrDefaultAsync(c => c.Id == commentId);

                if (comment == null)
                {
                    return Response<bool>.Failure(false, "Comment not found.",404);
                }

                _context.Comments.Remove(comment);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Response<bool>.Success(true, "Comment deleted successfully.",200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure(false, $"Error deleting comment: {ex.Message}",500);
            }
        }
        #endregion


    }
}

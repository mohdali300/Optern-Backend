using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs.Comment;
using Optern.Application.Interfaces.ICommentService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Repositories;
using Optern.Infrastructure.Response;
using Pipelines.Sockets.Unofficial.Arenas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Services.CommentService
{
    public class CommentService : GenericRepository<Comment>, ICommentService
    {
        public CommentService(OpternDbContext context) : base(context)
        {
        }


        public async Task<Response<List<CommentDTO>>> GetRepliesForCommentAsync(int commentId)
        {
            try
            {
                var targetComment = await _dbContext.Comments
                    .Include(comment => comment.User)
                    .FirstOrDefaultAsync(comment => comment.Id == commentId);

                if (targetComment == null)
                    return Response<List<CommentDTO>>.Failure(new List<CommentDTO>(), "Comment not found.", 404);

                var allComments = await _dbContext.Comments
                    .Include(comment => comment.User)
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
                    ReactCommentCount = comment.CommentReacts?.Count() ?? 0, 
                }).ToList();

                return Response<List<CommentDTO>>.Success(commentDTOs, "Replies for the comment fetched successfully.");
            }
            catch (Exception ex)
            {
                return Response<List<CommentDTO>>.Failure(new List<CommentDTO>(), $"Failed to fetch replies for the comment: {ex.Message}", 500);
            }
        }
        public async Task<Response<CommentDTO>> AddCommentAsync(AddCommentInputDTO input, string userId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var newComment = new Comment
                {
                    PostId = input.PostId,
                    Content = input.Content,
                    CommentDate = DateTime.UtcNow,
                    UserId = userId
                };

                _dbContext.Comments.Add(newComment);
                await _dbContext.SaveChangesAsync();

                var commentDto = new CommentDTO
                {
                    Id = newComment.Id,
                    Content = newComment.Content,
                    CommentDate = newComment.CommentDate,
                    UserName = (await _dbContext.Users.FindAsync(userId))?.UserName
                };

                await transaction.CommitAsync();

                return Response<CommentDTO>.Success(commentDto, "Comment added successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<CommentDTO>.Failure(new CommentDTO(), $"Error adding comment: {ex.Message}");
            }
        }
        public async Task<Response<CommentDTO>> AddReplyAsync(AddReplyInputDTO input, string userId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var parentComment = await _dbContext.Comments.FindAsync(input.ParentId);
                if (parentComment == null)
                {
                    return Response<CommentDTO>.Failure(new CommentDTO(), "Parent comment not found.");
                }

                var newReply = new Comment
                {
                    PostId = input.PostId,
                    Content = input.Content,
                    CommentDate = DateTime.UtcNow,
                    UserId = userId,
                    ParentId = input.ParentId
                };

                _dbContext.Comments.Add(newReply);
                await _dbContext.SaveChangesAsync();

                var replyDto = new CommentDTO
                {
                    Id = newReply.Id,
                    Content = newReply.Content,
                    CommentDate = newReply.CommentDate,
                    UserName = (await _dbContext.Users.FindAsync(userId))?.UserName
                };

                await transaction.CommitAsync();

                return Response<CommentDTO>.Success(replyDto, "Reply added successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<CommentDTO>.Failure(new CommentDTO(), $"Error adding reply: {ex.Message}");
            }
        }
        public async Task<Response<CommentDTO>> UpdateCommentAsync(int commentId, UpdateCommentInputDTO input)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                
                var comment = await _dbContext.Comments
                    .Include(c => c.User) 
                    .FirstOrDefaultAsync(c => c.Id == commentId);

                if (comment == null)
                {
                    return Response<CommentDTO>.Failure(new CommentDTO(),"Comment not found.", 404);
                }

                
                comment.Content = input.UpdatedContent;
                comment.CommentDate = DateTime.UtcNow;

                _dbContext.Comments.Update(comment);
                await _dbContext.SaveChangesAsync();

                
                await transaction.CommitAsync();

                
                var updatedCommentDto = new CommentDTO
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    CommentDate = comment.CommentDate,
                    UserName = comment.User.UserName
                };

                return Response<CommentDTO>.Success(updatedCommentDto, "Comment updated successfully.");
            }
            catch (Exception ex)
            {
                
                await transaction.RollbackAsync();
                return Response<CommentDTO>.Failure(new CommentDTO(),$"Failed to update comment: {ex.Message}");
            }
        }
        public async Task<Response<bool>> DeleteCommentAsync(int commentId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var comment = await _dbContext.Comments
                    .Include(c => c.comment) // Include replies to cascade deletion
                    .FirstOrDefaultAsync(c => c.Id == commentId);

                if (comment == null)
                {
                    return Response<bool>.Failure(false, "Comment not found.");
                }

                _dbContext.Comments.Remove(comment);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return Response<bool>.Success(true, "Comment deleted successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure(false, $"Error deleting comment: {ex.Message}");
            }
        }


    }
}

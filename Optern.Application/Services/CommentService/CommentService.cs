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

        
        public async Task<Response<List<CommentDTO>>> GetCommentsWithRepliesAsync(int id)
        {
            try
            {
                // Fetch all comments for the post
                var comments = await _dbContext.Comments
                    .Include(comment => comment.User)
                    .Where(comment => comment.PostId == id)
                    .OrderBy(comment => comment.CommentDate)
                    .ToListAsync();

                if (comments == null || !comments.Any())
                    return Response<List<CommentDTO>>.Failure("No comments found for the specified post.", 404);

                // Map comments with their replies
                var commentDtos = comments
                    .Where(c => c.ParentId == null) // Parent comments
                    .Select(parent => MapCommentWithReplies(parent, comments)) // Include replies recursively
                    .ToList();

                return Response<List<CommentDTO>>.Success(commentDtos, "Comments with replies fetched successfully.");
            }
            catch (Exception ex)
            {
                return Response<List<CommentDTO>>.Failure(new List<CommentDTO>(),$"Failed to fetch comments with replies: {ex.Message}");
            }
        }

        public async Task<Response<CommentDTO>> AddCommentAsync(AddCommentInputDTO input, string userId)
        {
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

                // Map to DTO
                var commentDto = new CommentDTO
                {
                    Id = newComment.Id,
                    Content = newComment.Content,
                    CommentDate = newComment.CommentDate,
                    UserName = (await _dbContext.Users.FindAsync(userId))?.UserName
                };

                return Response<CommentDTO>.Success(commentDto, "Comment added successfully.");
            }
            catch (Exception ex)
            {
                return Response<CommentDTO>.Failure(new CommentDTO(), $"Error adding comment: {ex.Message}");
            }
        }
        public async Task<Response<CommentDTO>> AddReplyAsync(AddReplyInputDTO input, string userId)
        {
            try
            {
                var parentComment = await _dbContext.Comments.FindAsync(input.ParentId);
                if (parentComment == null)
                    return Response<CommentDTO>.Failure(new CommentDTO(),"Parent comment not found.");

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

                // Map to DTO
                var replyDto = new CommentDTO
                {
                    Id = newReply.Id,
                    Content = newReply.Content,
                    CommentDate = newReply.CommentDate,
                    UserName = (await _dbContext.Users.FindAsync(userId))?.UserName
                };

                return Response<CommentDTO>.Success(replyDto, "Reply added successfully.");
            }
            catch (Exception ex)
            {
                return Response<CommentDTO>.Failure(new CommentDTO(),$"Error adding reply: {ex.Message}");
            }
        }

        //recursive helper for comments with their replies
        private CommentDTO MapCommentWithReplies(Comment parent, List<Comment> allComments)
        {
            return new CommentDTO
            {
                Id = parent.Id,
                Content = parent.Content,
                CommentDate = parent.CommentDate,
                UserName = parent.User?.UserName,
                Replies = allComments
                    .Where(c => c.ParentId == parent.Id) // Get replies to this comment
                    .Select(reply => MapCommentWithReplies(reply, allComments)) // Recursive mapping
                    .ToList()
            };
        }
    }
}

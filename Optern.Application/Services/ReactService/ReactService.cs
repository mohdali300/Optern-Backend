using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs.React;
using Optern.Application.Interfaces.IReactService;
using Optern.Domain.Entities;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Response;
using Optern.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace Optern.Application.Services.ReactService
{
    public class ReactService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper) :IReactService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<ReactDTO>> ManageReactAsync(int postId, string userId, ReactType reactType)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var post = await _context.Posts.FindAsync(postId);
                if (post == null)
                {
                    return Response<ReactDTO>.Failure(new ReactDTO(), "Post not found.", 404);
                }

                var existingReact = await _context.Reacts
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);
                
                //delete
                if (existingReact != null)
                {
                    if (existingReact.ReactType == reactType)
                    {
                        _context.Reacts.Remove(existingReact);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        var Dto = new ReactDTO
                        {
                            ReactDate = existingReact.ReactDate,
                            UserId = existingReact.UserId,
                            ReactType = existingReact.ReactType,
                            UserName = existingReact.User?.UserName
                        };
                        return Response<ReactDTO>.Success(Dto, "React removed successfully.");
                    }
                    else
                    {
                        //update
                        existingReact.ReactType = reactType;
                        existingReact.ReactDate = DateTime.UtcNow;
                        _context.Reacts.Update(existingReact);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        var dto = new ReactDTO
                        {
                            ReactDate = existingReact.ReactDate,
                            UserId = existingReact.UserId,
                            ReactType = existingReact.ReactType,
                            UserName = existingReact.User?.UserName
                        };
                        return Response<ReactDTO>.Success(dto, "React updated successfully.");
                    }
                }

                //create
                var newReact = new Reacts
                {
                    PostId = postId,
                    UserId = userId,
                    ReactType = reactType,
                    ReactDate = DateTime.UtcNow
                };

                _context.Reacts.Add(newReact);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                var reactDto = new ReactDTO
                {
                    ReactDate = existingReact.ReactDate,
                    UserId = existingReact.UserId,
                    ReactType = existingReact.ReactType,
                    UserName = existingReact.User?.UserName
                };
                return Response<ReactDTO>.Success(reactDto, "React added successfully.");
            }
            catch (Exception ex)
            {
                
                await transaction.RollbackAsync();
                return Response<ReactDTO>.Failure(new ReactDTO(), $"An error occurred while processing the react: {ex.Message}");
            }
        }

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
                        return Response<CommentReactDTO>.Success(dto, "React removed successfully.");
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
                        return Response<CommentReactDTO>.Success(dto, "React updated successfully.");
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

                return Response<CommentReactDTO>.Success(newDto, "React added successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<CommentReactDTO>.Failure(new CommentReactDTO(), $"An error occurred while processing the comment react: {ex.Message}");
            }
        }

    }
}

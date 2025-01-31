using AutoMapper;
using GreenDonut;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs.RoomUset;
using Optern.Application.Interfaces.IRoomUserService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Response;
using Optern.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Services.RoomUserService
{
    internal class RoomUserService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper) : IRoomUserService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;


        public async Task<Response<List<RoomUserDTO>>> GetAllCollaboratorsAsync(string roomId, bool? isAdmin = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roomId))
                {
                    return Response<List<RoomUserDTO>>.Failure(new List<RoomUserDTO>(),"Room ID cannot be empty.",400);
                }

                var roomExists = await _context.Rooms.AnyAsync(r => r.Id == roomId);
                if (!roomExists)
                {
                    return Response<List<RoomUserDTO>>.Failure(new List<RoomUserDTO>(),"Room not found.",404);
                }

                var query = _context.UserRooms
                    .Include(ur => ur.User)
                    .Where(ur => ur.RoomId == roomId);

                if (isAdmin.HasValue)
                {
                    query = query.Where(ur => ur.IsAdmin == isAdmin.Value);
                }

                var collaborators = await query
                    .OrderByDescending(ur => ur.IsAdmin)
                    .ThenBy(ur => ur.User.UserName)
                    .ToListAsync();

                if (!collaborators.Any())
                {
                    return Response<List<RoomUserDTO>>.Failure(new List<RoomUserDTO>(),"No collaborators found.",404);
                }

                var collaboratorsDto = _mapper.Map<List<RoomUserDTO>>(collaborators);

                return Response<List<RoomUserDTO>>.Success(collaboratorsDto,"Collaborators retrieved successfully.",200);
            }
            catch (Exception ex)
            {
                return Response<List<RoomUserDTO>>.Failure(
                    new List<RoomUserDTO>(),
                    $"An error occurred while retrieving collaborators: {ex.Message}",
                    500
                );
            }
        }
        public async Task<Response<RoomUserDTO>> DeleteCollaboratorAsync(string RoomId, string TargetUserId, string currentUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var room = await _context.Rooms.FindAsync(RoomId);
                if (room == null)
                {
                    return Response<RoomUserDTO>.Failure(new RoomUserDTO(), "Room not found", 404);
                }

                var currentUserRoom = await _context.UserRooms
                    .FirstOrDefaultAsync(ur => ur.RoomId == RoomId && ur.UserId == currentUserId);

                if (currentUserRoom == null || !currentUserRoom.IsAdmin)
                {
                    return Response<RoomUserDTO>.Failure(new RoomUserDTO(), "Unauthorized: Only leaders can remove collaborators", 403);
                }

                var targetUserRoom = await _context.UserRooms
                    .Include(ur => ur.User) 
                    .FirstOrDefaultAsync(ur => ur.RoomId == RoomId && ur.UserId == TargetUserId);

                if (targetUserRoom == null)
                {
                    return Response<RoomUserDTO>.Failure(new RoomUserDTO(), "User not found in this room", 404);
                }

                if (targetUserRoom.IsAdmin)
                {
                    var remainingLeaders = await _context.UserRooms
                        .CountAsync(ur => ur.RoomId == RoomId && ur.IsAdmin && ur.UserId != TargetUserId);

                    if (remainingLeaders < 1)
                    {
                        return Response<RoomUserDTO>.Failure(new RoomUserDTO(), "Cannot remove the last leader", 400);
                    }
                }

                if (TargetUserId == currentUserId && targetUserRoom.IsAdmin)
                {
                    var otherLeaders = await _context.UserRooms
                        .CountAsync(ur => ur.RoomId == RoomId && ur.IsAdmin && ur.UserId != currentUserId);

                    if (otherLeaders < 1)
                    {
                        return Response<RoomUserDTO>.Failure(new RoomUserDTO(), "Cannot remove yourself as the last leader", 400);
                    }
                }

                var removedUserDto = _mapper.Map<RoomUserDTO>(targetUserRoom);

                _context.UserRooms.Remove(targetUserRoom);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Response<RoomUserDTO>.Success(removedUserDto, "Collaborator removed successfully", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<RoomUserDTO>.Failure(new RoomUserDTO(), $"An error occurred while removing collaborator: {ex.Message}", 500);
            }
        }

        public async Task<Response<RoomUserDTO>> ToggleLeadershipAsync(string roomId, string targetUserId, string currentUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var room = await _context.Rooms.FindAsync(roomId);
                if (room == null)
                {
                    return Response<RoomUserDTO>.Failure(new RoomUserDTO(), "Room not found", 404); 
                }

                var currentUserRoom = await _context.UserRooms
                    .FirstOrDefaultAsync(ur => ur.RoomId == roomId && ur.UserId == currentUserId);

                if (currentUserRoom == null || !currentUserRoom.IsAdmin)
                {
                    return Response<RoomUserDTO>.Failure(new RoomUserDTO(), "Only existing leaders can modify leadership status", 403); //not authorized 
                }

                var targetUserRoom = await _context.UserRooms
                    .Include(ur => ur.User) 
                    .FirstOrDefaultAsync(ur => ur.RoomId == roomId && ur.UserId == targetUserId);

                if (targetUserRoom == null)
                {
                    return Response<RoomUserDTO>.Failure(new RoomUserDTO(), "Target user is not a room member", 404);  
                }

                bool isPromoting = !targetUserRoom.IsAdmin; //new status

                if (!isPromoting)
                {
                    
                    var remainingLeaders = await _context.UserRooms
                        .CountAsync(ur => ur.RoomId == roomId && ur.IsAdmin && ur.UserId != targetUserId);

                    if (remainingLeaders < 1) //to prevent remove last leader
                    {
                        return Response<RoomUserDTO>.Failure(new RoomUserDTO(), "Cannot remove the last leader", 400);
                    }

                    if (targetUserId == currentUserId && remainingLeaders == 0)
                    {
                        return Response<RoomUserDTO>.Failure(new RoomUserDTO(), "Cannot remove yourself as the last leader", 400);
                    }
                }

                targetUserRoom.IsAdmin = isPromoting; //change status

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var updatedUserDto = _mapper.Map<RoomUserDTO>(targetUserRoom);
                return Response<RoomUserDTO>.Success(updatedUserDto, isPromoting ? "User Assigned to leader" : "User Revoked from leader", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<RoomUserDTO>.Failure(new RoomUserDTO(), "An error occurred while updating leadership status", 500);
            }
        }




    }
}

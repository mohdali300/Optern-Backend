namespace Optern.Infrastructure.Services.RoomUserService
{
    internal class RoomUserService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper, IChatService chatService, ICacheService cacheService) : IRoomUserService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IChatService _chatService = chatService;
        private readonly ICacheService _cacheService = cacheService;


        public async Task<Response<List<RoomUserDTO>>> GetAllCollaboratorsAsync(string roomId, bool? isAdmin = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roomId))
                {
                    return Response<List<RoomUserDTO>>.Failure(new List<RoomUserDTO>(), "Room ID cannot be empty.", 400);
                }

                var roomExists = await _context.Rooms.AnyAsync(r => r.Id == roomId);
                if (!roomExists)
                {
                    return Response<List<RoomUserDTO>>.Failure(new List<RoomUserDTO>(), "Room not found.", 404);
                }

                var query = _context.UserRooms
                    .Include(ur => ur.User)
                    .Where(ur => ur.RoomId == roomId && ur.IsAccepted == true);

                if (isAdmin.HasValue)
                {
                    query = query.Where(ur => ur.IsAdmin == isAdmin.Value);
                }

                var collaborators = await query
                    .OrderByDescending(ur => ur.JoinedAt)
                    .ThenBy(ur => ur.User.FirstName)
                    .ToListAsync();

                if (!collaborators.Any())
                {
                    return Response<List<RoomUserDTO>>.Failure(new List<RoomUserDTO>(), "No collaborators found.", 404);
                }

                var collaboratorsDto = _mapper.Map<List<RoomUserDTO>>(collaborators);

                return Response<List<RoomUserDTO>>.Success(collaboratorsDto, "Collaborators retrieved successfully.", 200);
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

        public async Task<Response<List<RoomUserDTO>>> GetPendingRequestsAsync(string roomId, string leaderId)
        {
            try
            {
                var room = await _context.Rooms.FindAsync(roomId);
                if (room == null)
                {
                    return Response<List<RoomUserDTO>>.Failure(new List<RoomUserDTO>(), "Room not found", 404);
                }

                var isLeader = await _context.UserRooms
                    .AnyAsync(ur => ur.RoomId == roomId &&
                                  ur.UserId == leaderId &&
                                  ur.IsAdmin);

                if (!isLeader)
                {
                    return Response<List<RoomUserDTO>>.Failure(new List<RoomUserDTO>(), "Unauthorized access", 403);
                }

                var pendingRequests = await _context.UserRooms
                    .Include(ur => ur.User)
                    .Where(ur => ur.RoomId == roomId && !ur.IsAccepted)
                    .OrderBy(ur => ur.JoinedAt)
                    .Select(ur => new RoomUserDTO
                    {
                        Id = ur.Id,
                        RoomId = ur.RoomId,
                        UserId = ur.UserId,
                        UserName = $"{ur.User.FirstName} {ur.User.LastName}",
                        ProfilePicture = ur.User.ProfilePicture,
                        JoinedAt = ur.JoinedAt,
                        IsAccepted = ur.IsAccepted,
                        IsAdmin = ur.IsAdmin,
                    })
                    .ToListAsync();

                return Response<List<RoomUserDTO>>.Success(pendingRequests, pendingRequests.Any() ? $"Found {pendingRequests.Count} pending requests" : "No pending requests found", 200);
            }
            catch (Exception ex)
            {
                return Response<List<RoomUserDTO>>.Failure(new List<RoomUserDTO>(), "An error occurred while retrieving requests", 500);
            }
        }

        public async Task<Response<string>> RequestToRoom(JoinRoomDTO model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.RoomId))
                {
                    return Response<string>.Failure("Invalid Data Model", 400);
                }

                var roomExist = await _context.Rooms.FindAsync(model.RoomId);
                if (roomExist == null)
                {
                    return Response<string>.Failure("Room not found", 404);
                }

                var userExist = await _context.Users.FindAsync(model.UserId);
                if (userExist == null)
                {
                    return Response<string>.Failure("User not found", 404);
                }

                var isUserInRoom = await _context.UserRooms
                    .AnyAsync(u => u.UserId == model.UserId && u.RoomId == model.RoomId);

                if (isUserInRoom)
                {
                    return Response<string>.Failure("You have already requested to join this room!", 400);
                }

                var joinRequest = new UserRoom
                {
                    RoomId = model.RoomId,
                    UserId = model.UserId,
                    JoinedAt = DateTime.UtcNow,
                    IsAdmin = false,
                    IsAccepted = false
                };

                await _context.UserRooms.AddAsync(joinRequest);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Response<string>.Success("Your request to join has been sent successfully", "Waiting for approval", 201);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<string>.Failure($"Server error. Please try again later. Error: {ex.Message}", 500);
            }
        }

        public async Task<Response<RoomUserDTO>> DeleteCollaboratorAsync(string RoomId, string TargetUserId, string leaderId)
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
                    .FirstOrDefaultAsync(ur => ur.RoomId == RoomId && ur.UserId == leaderId);

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

                if (TargetUserId == leaderId && targetUserRoom.IsAdmin)
                {
                    var otherLeaders = await _context.UserRooms
                        .CountAsync(ur => ur.RoomId == RoomId && ur.IsAdmin && ur.UserId != leaderId);

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

        public async Task<Response<RoomUserDTO>> ToggleLeadershipAsync(string roomId, string targetUserId, string leaderId)
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
                    .FirstOrDefaultAsync(ur => ur.RoomId == roomId && ur.UserId == leaderId);

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

                    if (targetUserId == leaderId && remainingLeaders == 0)
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

        public async Task<Response<List<RoomUserDTO>>> AcceptRequestsAsync(string roomId, string leaderId, int? userRoomId = null, bool? approveAll = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!userRoomId.HasValue && approveAll != true)
                    return Response<List<RoomUserDTO>>.Failure(new List<RoomUserDTO>(), "Specify either UserRoomId or set ApproveAll to true", 400);

                if (!await _context.UserRooms.AnyAsync(ur => ur.RoomId == roomId && ur.UserId == leaderId && ur.IsAdmin && ur.IsAccepted))
                    return Response<List<RoomUserDTO>>.Failure(new List<RoomUserDTO>(), "Unauthorized: Only room leaders can process requests", 403);

                List<UserRoom> requestsToApprove = new();

                if (userRoomId.HasValue)
                {
                    var userRoom = await _context.UserRooms.Include(ur => ur.User).FirstOrDefaultAsync(ur => ur.Id == userRoomId.Value);
                    if (userRoom == null)
                        return Response<List<RoomUserDTO>>.Failure(new List<RoomUserDTO>(), "Request not found", 404);

                    await _chatService.JoinToRoomChatAsync(roomId, userRoom.UserId); // add user to room chat
                    userRoom.IsAccepted = true;
                    userRoom.JoinedAt = DateTime.UtcNow;
                    userRoom.IsAdmin = false;
                    _context.UserRooms.Attach(userRoom);
                    _context.Entry(userRoom).State = EntityState.Modified;
                }
                else
                {
                    requestsToApprove = await _context.UserRooms.Where(ur => ur.RoomId == roomId && !ur.IsAccepted).Include(ur => ur.User).ToListAsync();
                    if (!requestsToApprove.Any())
                        return Response<List<RoomUserDTO>>.Success(new List<RoomUserDTO>(), "No pending requests to approve.");

                    List<string> UsersIds = new List<string>();
                    requestsToApprove.ForEach(ur => UsersIds.Add(ur.UserId));
                    await _chatService.JoinAllToRoomChatAsync(roomId, UsersIds); // add all to room caht

                    requestsToApprove.ForEach(ur =>
                    {
                        ur.IsAccepted = true;
                        ur.JoinedAt = DateTime.UtcNow;
                        ur.IsAdmin = false;
                    });
                }
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var updatedUsers = requestsToApprove.Select(ur => new RoomUserDTO
                {
                    UserId = ur.UserId,
                    UserName = $"{ur.User.FirstName} {ur.User.LastName}",
                    ProfilePicture = ur.User.ProfilePicture,
                    RoomId = ur.RoomId,
                    IsAdmin = ur.IsAdmin,
                    AcceptedAt = ur.JoinedAt
                }).ToList();

                return Response<List<RoomUserDTO>>.Success(updatedUsers, "Requests approved successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<List<RoomUserDTO>>.Failure(new List<RoomUserDTO>(),$"An error occurred processing the request: {ex.Message}", 500);
            }
        }

        public async Task<Response<string>> RejectRequestsAsync(string roomId, string leaderId, int? userRoomId = null, bool? rejectAll = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!userRoomId.HasValue && rejectAll != true)
                    return Response<string>.Failure("Specify either UserRoomId or set RejectAll to true", 400);

                if (!await _context.UserRooms.AnyAsync(ur => ur.RoomId == roomId && ur.UserId == leaderId && ur.IsAdmin && ur.IsAccepted))
                    return Response<string>.Failure("Unauthorized: Only room leaders can process requests", 403);

                List<UserRoom> requestsToReject = new();

                if (userRoomId.HasValue)
                {
                    var userRoom = await _context.UserRooms.FirstOrDefaultAsync(ur => ur.Id == userRoomId.Value);
                    if (userRoom == null)
                        return Response<string>.Failure("Request not found", 404);

                    requestsToReject.Add(userRoom);
                }
                else
                {
                    requestsToReject = await _context.UserRooms.Where(ur => ur.RoomId == roomId && !ur.IsAccepted).ToListAsync();
                    if (!requestsToReject.Any())
                        return Response<string>.Success("No pending requests to reject.");
                }

                _context.UserRooms.RemoveRange(requestsToReject);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Response<string>.Success("Requests rejected successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<string>.Failure($"An error occurred processing the request: {ex.Message}", 500);
            }
        }

        public async Task<Response<IEnumerable<ProfileUserRoomDTO>>> getLatestUserRoomsAsync(string userId, bool? isPublic = null)
        {
            if (string.IsNullOrEmpty(userId))
                return Response<IEnumerable<ProfileUserRoomDTO>>.Failure(new List<ProfileUserRoomDTO>(), "Invalid user id.", 400);

            try
            {
                var userCachedRooms = _cacheService.GetData<IEnumerable<ProfileUserRoomDTO>>($"{userId}LatestRooms");
                if(userCachedRooms != null && userCachedRooms.Any())
                {
                    return Response<IEnumerable<ProfileUserRoomDTO>>.Success(userCachedRooms, "Latest user rooms fetched sccessfully.", 200);
                }

                if (!await _unitOfWork.Users.AnyAsync(u=>u.Id == userId))
                {
                    return Response<IEnumerable<ProfileUserRoomDTO>>.Failure(new List<ProfileUserRoomDTO>(), "User not found.", 404);
                }

                var userRooms = await _context.UserRooms.Include(ur => ur.Room)
                    .Where(ur => ur.UserId == userId).OrderByDescending(ur => ur.JoinedAt)
                    .ToListAsync();
                if (isPublic.HasValue && isPublic is true)
                {
                    userRooms = userRooms.Where(ur => ur.Room.RoomType == RoomType.Public).ToList();
                }

                if (!userRooms.Any())
                {
                    return Response<IEnumerable<ProfileUserRoomDTO>>.Success(new List<ProfileUserRoomDTO>(), "User didn’t join to any room.", 204);
                }

                var roomDto = userRooms.Take(3).Select(ur => new ProfileUserRoomDTO
                {
                    RoomId = ur.RoomId,
                    Name = ur.Room.Name,
                    Description = ur.Room.Description,
                    Type = ur.Room.RoomType,
                });

                _cacheService.SetData($"{userId}LatestRooms", roomDto, TimeSpan.FromMinutes(15));
                return Response<IEnumerable<ProfileUserRoomDTO>>.Success(roomDto, "Latest user rooms fetched sccessfully.", 200);
            }
            catch(Exception ex)
            {
                return Response<IEnumerable<ProfileUserRoomDTO>>.Success(new List<ProfileUserRoomDTO>(), $"Server error: {ex.Message}", 500);
            }
        }

    }
}

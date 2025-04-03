
using Optern.Domain.Entities;
using Optern.Domain.Specifications;

namespace Optern.Infrastructure.Services.RoomService
{
    public class RoomService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper, IUserService userService,
        ICloudinaryService cloudinaryService, IRoomPositionService roomPositionService, IRoomTrackService roomTrackService, ISkillService skillService, IRoomSkillService roomSkillService,
        IRepositoryService repositoryService, IChatService chatService, ICacheService cacheService) : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICacheService _cacheService = cacheService;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IUserService _userService = userService;
        private readonly ICloudinaryService _cloudinaryService = cloudinaryService;
        private readonly IRoomPositionService _roomPositionService = roomPositionService;
        private readonly IRoomTrackService _roomTrackService = roomTrackService;
        private readonly ISkillService _skillService = skillService;
        private readonly IRoomSkillService _roomSkillService = roomSkillService;
        private readonly IRepositoryService _repositoryService = repositoryService;
        private readonly IChatService _chatService = chatService;

        #region GetAllAsync

        public async Task<bool> IsRoomExist(string roomId)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
            return room == null ? false : true;
        }

        public async Task<Response<IEnumerable<ResponseRoomDTO>>> GetAllAsync()
        {
            var cachedrooms = _cacheService.GetData<IEnumerable<ResponseRoomDTO>>("Rooms");

            if (cachedrooms is not null)
            {
                return Response<IEnumerable<ResponseRoomDTO>>.Success(cachedrooms, "Cached Data Retrieved Successfully", 200);
            }

            try
            {
                var rooms = await _unitOfWork.Rooms.GetAllAsync();
                if (!rooms.Any())
                {
                    return Response<IEnumerable<ResponseRoomDTO>>.Failure("No Rooms Found", 404);
                }
                var roomsDtos = _mapper.Map<IEnumerable<ResponseRoomDTO>>(rooms);

                _cacheService.SetData("Rooms", roomsDtos, TimeSpan.FromMinutes(5));

                return Response<IEnumerable<ResponseRoomDTO>>.Success(roomsDtos, "", 200);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<ResponseRoomDTO>>.Failure($"There is a server error. Please try again later. {ex.Message}", 500);
            }
        }
        #endregion

        #region GetPopularRooms
        public async Task<Response<IEnumerable<ResponseRoomDTO>>> GetPopularRooms()
        {
            var cachedrooms = _cacheService.GetData<IEnumerable<ResponseRoomDTO>>("PopulerRooms");

            if (cachedrooms is not null)
            {
                return Response<IEnumerable<ResponseRoomDTO>>.Success(cachedrooms, "Cached Data Retrieved Successfully", 200);
            }

            try
            {
                var rooms = await _context.Rooms
                    .GroupJoin(_context.UserRooms,
                      room => room.Id,
                      userRooms => userRooms.RoomId,
                      (room, userRooms) => new
                      {
                          Room = room,
                          NumberOfUsers = userRooms.Count(r => r.IsAccepted)
                      }
                    )
                    .OrderByDescending(r => r.NumberOfUsers)
                    .Take(4)
                     .Select(r => new ResponseRoomDTO
                     {
                         Id = r.Room.Id,
                         Name = r.Room.Name,
                         Description = r.Room.Description,
                         CoverPicture = r.Room.CoverPicture,
                         Members = r.NumberOfUsers,
                         CreatedAt = r.Room.CreatedAt,
                         Tracks = r.Room.RoomTracks.Select(rt => new TrackDTO
                         {
                             Id = rt.Track.Id,
                             Name = rt.Track.Name,
                         }).ToList()

                     })
                     .ToListAsync();

                _cacheService.SetData("PopulerRooms", rooms, TimeSpan.FromMinutes(5));

                return rooms.Any() ? Response<IEnumerable<ResponseRoomDTO>>.Success(rooms, "Populer Rooms Rooms Fetched Successfully", 200) :
                                     Response<IEnumerable<ResponseRoomDTO>>.Success(rooms, "There Are No Created Rooms Until Now", 204);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<ResponseRoomDTO>>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
            }
        }
        #endregion

        #region GetCreatedRooms
        public async Task<Response<IEnumerable<ResponseRoomDTO>>> GetCreatedRooms(string id, int lastIdx = 0, int limit = 1)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Response<IEnumerable<ResponseRoomDTO>>.Failure(new List<ResponseRoomDTO>(), "Invalid CreatorId", 400); ;
            }
            try
            {
                var isUserExist = await _unitOfWork.Users.GetByIdAsync(id);
                if (isUserExist == null)
                {
                    return Response<IEnumerable<ResponseRoomDTO>>.Failure(new List<ResponseRoomDTO>(), "User Not Found", 404);
                }
                var createdRooms = await _context.Rooms
                    .Include(r => r.UserRooms)
                    .Include(r => r.RoomTracks)
                    .Where(r => r.CreatorId == id)
                    .Skip(lastIdx)
                    .Take(limit)
                    .Select(r => new ResponseRoomDTO
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        CoverPicture = r.CoverPicture,
                        Members = r.UserRooms.Count(r => r.IsAccepted),
                        CreatedAt = r.CreatedAt,
                        RoomType = r.RoomType,
                        Tracks = r.RoomTracks.Select(rt => new TrackDTO
                        {
                            Id = rt.Track.Id,
                            Name = rt.Track.Name,
                        }).ToList()

                    })
                    .ToListAsync();

                return createdRooms.Any() ? Response<IEnumerable<ResponseRoomDTO>>.Success(createdRooms, "Created Rooms Fetched Successfully", 200, _context.Rooms.Where(r => r.CreatorId == id).Count()) :
                                            Response<IEnumerable<ResponseRoomDTO>>.Success(createdRooms, "There Are no Created Rooms Until Now", 204);

            }
            catch (Exception ex)
            {
                return Response<IEnumerable<ResponseRoomDTO>>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);

            }
        }
        #endregion

        #region GetJoinedRooms
        public async Task<Response<IEnumerable<ResponseRoomDTO>>> GetJoinedRooms(string id, int lastIdx = 0, int limit = 8)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Response<IEnumerable<ResponseRoomDTO>>.Failure(new List<ResponseRoomDTO>(), "Invalid UserId", 400); ;
            }
            try
            {
                var isUserExist = await _unitOfWork.Users.GetByIdAsync(id);
                if (isUserExist == null)
                {
                    return Response<IEnumerable<ResponseRoomDTO>>.Failure(new List<ResponseRoomDTO>(), "User Not Found", 404);
                }

                var joinedRooms = await _context.Rooms.Include(r => r.UserRooms)
                     .Where(r => r.UserRooms.Any(r => r.UserId == id && r.IsAccepted))
                     .Skip(lastIdx)
                     .Take(limit)
                     .Select(r => new ResponseRoomDTO
                     {
                         Id = r.Id,
                         Name = r.Name,
                         Description = r.Description,
                         CoverPicture = r.CoverPicture,
                         Members = r.UserRooms.Count(r => r.IsAccepted),
                         CreatedAt = r.CreatedAt,
                         RoomType = r.RoomType,
                         Tracks = r.RoomTracks.Select(rt => new TrackDTO
                         {
                             Id = rt.Track.Id,
                             Name = rt.Track.Name,
                         }).ToList()
                     })
                    .ToListAsync();

                return joinedRooms.Any() ? Response<IEnumerable<ResponseRoomDTO>>.Success(joinedRooms, "", 200, _context.Rooms.Include(r => r.UserRooms).Where(r => r.UserRooms.Any(r => r.UserId == id && r.IsAccepted)).Count()) :
                                           Response<IEnumerable<ResponseRoomDTO>>.Success(joinedRooms, "You have not joined any room yet.", 204);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<ResponseRoomDTO>>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
            }
        }
        #endregion

        #region JoinToRoom
        public async Task<Response<string>> JoinToRoom(JoinRoomDTO model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.RoomId))
                {
                    return Response<string>.Failure($"Invalid Data", $"Invalid Data", 400);

                }
                var roomExist = await _unitOfWork.Rooms.GetByIdAsync(model.RoomId);
                var userExist = await _unitOfWork.Users.GetByIdAsync(model.UserId);

                if (roomExist == null || userExist == null)
                {
                    return Response<string>.Failure("Either Room or User is not found!", "Either Room or User is not found!", 404);
                }

                var isUserInRoom = await _unitOfWork.UserRoom
                    .GetByExpressionAsync(u => u.UserId == model.UserId && u.RoomId == model.RoomId);
                if (isUserInRoom != null)
                {
                    return Response<string>.Failure($"You have already requested to join this room!", $"You have already requested to join this room!", 400);
                }
                var joinRoom = await _unitOfWork.UserRoom.AddAsync(new UserRoom
                {
                    RoomId = model.RoomId,
                    UserId = model.UserId,
                    IsAdmin = false,
                    JoinedAt = DateTime.UtcNow,
                    IsAccepted = false
                });

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                return Response<string>.Success($"Your request to join has been sent successfully", "Waiting for approval", 201);

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<string>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
            }
        }
        #endregion

        #region Create Room
        public async Task<Response<ResponseRoomDTO>> CreateRoom(CreateRoomDTO model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (model == null)
                {
                    return Response<ResponseRoomDTO>.Failure("Invalid Data Model", 400);
                }
                // current login User  
                var currentUser = await _userService.GetCurrentUserAsync();
                var chat = await _chatService.CreateRoomChatAsync(model.CreatorId, ChatType.Group); // create chat for room
                if (!chat.IsSuccess)
                {
                    return Response<ResponseRoomDTO>.Failure($"{chat.Message}", 400);
                }
                var room = new Room
                {
                    Name = model.Name,
                    Description = model.Description,
                    RoomType = model.RoomType,
                    CreatedAt = DateTime.UtcNow,
                    CreatorId = model.CreatorId, // replace with ==> _userService.GetCurrentUserAsync()
                    ChatId = chat.Data.Id
                };

                var validate = new RoomValidator().Validate(room);

                if (!validate.IsValid)
                {
                    var errorMessages = string.Join(", ", validate.Errors.Select(e => e.ErrorMessage));
                    return Response<ResponseRoomDTO>.Failure(new ResponseRoomDTO(), $"Invalid Data Model: {errorMessages}", 400);
                }
                await _unitOfWork.Rooms.AddAsync(room);
                await _unitOfWork.SaveAsync();

                if (model.Positions != null && model.Positions.Any())
                {
                    await _roomPositionService.AddRoomPosition(room.Id, model.Positions);
                }
                if (model.Tracks != null && model.Tracks.Any())
                {
                    await _roomTrackService.AddRoomTrack(room.Id, model.Tracks);
                }
                if (model.Skills != null && model.Skills.Any())
                {
                    await ManageSkillOperationistRoomCreation(room, model.Skills);
                }

                await _repositoryService.AddRepository(room.Id); // add Repository for Room By Default while creation process


                await _unitOfWork.UserRoom.AddAsync(new UserRoom
                {
                    UserId = room.CreatorId,  // replace with ==> _userService.GetCurrentUserAsync()
                    RoomId = room.Id,
                    IsAdmin = true,
                    JoinedAt = DateTime.UtcNow,
                    IsAccepted = true
                });

                await _chatService.JoinToRoomChatAsync(room.Id, room.CreatorId); // add creator to chat

                await _unitOfWork.SaveAsync();

                await transaction.CommitAsync();
                var roomDto = _mapper.Map<ResponseRoomDTO>(room);
                roomDto.Skills = _context.RoomSkills.Where(rs => rs.RoomId == room.Id).Select(rs => new SkillDTO
                {
                    Name = rs.Skill.Name
                }
                ).ToList();
				roomDto.Tracks = _context.RoomTracks.Where(rt => rt.RoomId == room.Id).Select(rt => new TrackDTO
                {
                    Name = rt.Track.Name
                }
                ).ToList();
				roomDto.Position = _context.RoomPositions.Where(rp => rp.RoomId == room.Id).Select(rt => new PositionDTO
                {
                    Name = rt.Position.Name
                }
                ).ToList();
                return Response<ResponseRoomDTO>.Success(roomDto, "Room Added Successfully", 200);

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<ResponseRoomDTO>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
            }
        }
        #endregion

        #region  Get Room

        public async Task<Response<ResponseRoomDTO>> GetRoomById(string id, string? userId)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Response<ResponseRoomDTO>.Failure(new ResponseRoomDTO(), "Invalid Room Id", 400);
            }
            try
            {
                var room = await _context.Rooms
                    .Include(room => room.RoomSkills)
                        .ThenInclude(room => room.Skill)
                    .Include(room => room.UserRooms)
                    .Where(r => r.Id == id)
                    .Select(room => new ResponseRoomDTO
                    {
                        Id = room.Id,
                        Name = room.Name,
                        UserStatus = room.UserRooms.Any(r => r.UserId == userId && r.IsAccepted == true) ?
                        UserRoomStatus.Accepted : room.UserRooms.Any(r => r.UserId == userId) ? UserRoomStatus.Requested : UserRoomStatus.NONE,
                        Description = room.Description,
                        RoomType = room.RoomType,
                        CreatorId = room.CreatorId,
                        CoverPicture = room.CoverPicture,
                        CreatedAt = room.CreatedAt,
                        chatId = room.ChatId,
                        Members = room.UserRooms.Count(r => r.IsAccepted == true),
                        Skills = room.RoomSkills
                        .Select(rs => new SkillDTO
                        {
                            Id = rs.Skill.Id,
                            Name = rs.Skill.Name
                        }).Distinct().ToList(),
                        Tracks = room.RoomTracks
                        .Select(track => new TrackDTO
                        {
                            Id = track.Track.Id,
                            Name = track.Track.Name
                        }).Distinct().ToList(),
                        Position = room.RoomPositions
                        .Select(position => new PositionDTO
                        {
                            Id = position.Position.Id,
                            Name = position.Position.Name
                        }).Distinct().ToList()
                    }).FirstOrDefaultAsync();


                return room == null ? Response<ResponseRoomDTO>.Failure(new ResponseRoomDTO(), "This Room Not Found", 404) :
                                     Response<ResponseRoomDTO>.Success(room, "Room Fetched Successfully", 200);
            }
            catch (Exception ex)
            {
                return Response<ResponseRoomDTO>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
            }
        }
        #endregion

        #region Get Room By Track

        public async Task<Response<IEnumerable<ResponseRoomDTO>>> GetRoomsByTrack(int trackId, int lastIdx = 0, int limit = 10)
        {
            if (trackId == 0)
            {
                return Response<IEnumerable<ResponseRoomDTO>>.Failure(new List<ResponseRoomDTO>(), "Invalid Room Id", 400);
            }
            try
            {
                var isTrackExist = await _unitOfWork.Tracks.GetByIdAsync(trackId);
                if (isTrackExist == null)
                {
                    return Response<IEnumerable<ResponseRoomDTO>>.Failure(new List<ResponseRoomDTO>(), "Track Not Found", 404);
                }
                var rooms = await _context.Rooms
                    .Where(r => r.RoomTracks.Any(rt => rt.TrackId == trackId))
                    .Include(r => r.UserRooms)
                    .Include(r => r.RoomTracks)
                        .ThenInclude(rt => rt.Track)
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip(lastIdx)
                    .Take(limit)
                    .Select(r => new ResponseRoomDTO
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        CoverPicture = r.CoverPicture ?? string.Empty,
                        Members = r.UserRooms.Count(),
                        CreatedAt = r.CreatedAt,
                        RoomType = r.RoomType,
                        Tracks = r.RoomTracks.Select(rt => new TrackDTO
                        {
                            Id = rt.Track.Id,
                            Name = rt.Track.Name
                        }).ToList()
                    })
                    .ToListAsync();

                if (!rooms.Any())
                {
                    return Response<IEnumerable<ResponseRoomDTO>>.Failure(new List<ResponseRoomDTO>(), "No rooms found for the given track.", 404);
                }

                return Response<IEnumerable<ResponseRoomDTO>>.Success(rooms, "Rooms retrieved successfully.", 200, _context.Rooms.Count(r => r.RoomTracks.Any(t => t.TrackId == trackId)));
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<ResponseRoomDTO>>.Failure(new List<ResponseRoomDTO>(), $"Server error: {ex.Message}", 500);
            }
        }

        #endregion

        // Helper Function

        #region Helper Function For Manage Skills in Room
        private async Task<bool> ManageSkillOperationistRoomCreation(Room room, IEnumerable<SkillDTO> data)
        {
            if (data.Any())
            {
                var skill = await _skillService.AddSkills(data);
            }
            var allSkills = await _context.Skills
                    .Where(s => data.Select(ms => ms.Name).Contains(s.Name))
                    .ToListAsync();

            var roomSkills = allSkills.Select(skill => skill.Id).ToList();
            await _roomSkillService.AddRoomSkills(room.Id, roomSkills);
            return true;
        }
        #endregion
    }
}

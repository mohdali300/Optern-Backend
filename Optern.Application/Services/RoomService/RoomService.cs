using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Optern.Application.DTOs.Room;
using Optern.Application.DTOs.Track;
using Optern.Application.Interfaces.IRoomService;
using Optern.Application.Interfaces.IUserService;
using Optern.Domain.Entities;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.ExternalInterfaces.IFileService;
using Optern.Infrastructure.ExternalServices.FileService;
using Optern.Infrastructure.Repositories;
using Optern.Infrastructure.Response;
using Optern.Infrastructure.UnitOfWork;
using Optern.Infrastructure.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Services.RoomService
{
    public class RoomService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper,IUserService userService, ICloudinaryService cloudinaryService) : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IUserService _userService = userService;
        private readonly ICloudinaryService _cloudinaryService= cloudinaryService;

        #region GetAllAsync
        public async Task<Response<IEnumerable<RoomDTO>>> GetAllAsync()
        {
            try
            {
                var rooms = await _unitOfWork.Rooms.GetAllAsync();
                if (!rooms.Any())
                {
                    return Response<IEnumerable<RoomDTO>>.Failure("No Rooms Found", 404);
                }

                var roomsDtos = _mapper.Map<IEnumerable<RoomDTO>>(rooms);

                return Response<IEnumerable<RoomDTO>>.Success(roomsDtos, "", 200);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<RoomDTO>>.Failure($"There is a server error. Please try again later. {ex.Message}", 500);
            }
        } 
        #endregion

        #region GetPopularRooms
        public async Task<Response<IEnumerable<RoomDTO>>> GetPopularRooms()
        {
            try
            {
                var rooms = await _context.Rooms
                    .GroupJoin(_context.UserRooms,
                      room => room.Id,
                      userRooms => userRooms.RoomId,
                      (room, userRooms) => new
                      {
                          Room = room,
                          NumberOfUsers = userRooms.Count()
                      }
                    )
                    .OrderByDescending(r => r.NumberOfUsers)
                     .Select(r => new RoomDTO
                     {
                         Name = r.Room.Name,
                         Description = r.Room.Description,
                         Capacity = r.Room.Capacity,
                         CoverPicture = r.Room.CoverPicture,
                         NumberOfParticipants = r.NumberOfUsers,
                         CreatedAt = r.Room.CreatedAt,
                     })
                     .ToListAsync();

                return rooms.Any() ? Response<IEnumerable<RoomDTO>>.Success(rooms, "", 200) :
                                     Response<IEnumerable<RoomDTO>>.Failure(new List<RoomDTO>(), "There Are No Created Rooms Until Now", 404);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<RoomDTO>>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
            }
        } 
        #endregion

        #region GetCreatedRooms
        public async Task<Response<IEnumerable<RoomDTO>>> GetCreatedRooms(string id)
        {
            try
            {
                var createdRooms = await _context.Rooms
                    .Include(r => r.UserRooms)
                    .Where(r => r.CreatorId == id)
                    .Select(r => new RoomDTO
                    {
                        Name = r.Name,
                        Description = r.Description,
                        Capacity = r.Capacity,
                        CoverPicture = r.CoverPicture.ToString(),
                        NumberOfParticipants = r.UserRooms.Count(),
                        CreatedAt = r.CreatedAt,
                        RoomType = r.RoomType,
                    })
                    .ToListAsync();

                return createdRooms.Any() ? Response<IEnumerable<RoomDTO>>.Success(createdRooms, "", 200) :
                                            Response<IEnumerable<RoomDTO>>.Failure(new List<RoomDTO>(), "There is no Created Rooms Until Now", 404);

            }
            catch (Exception ex)
            {
                return Response<IEnumerable<RoomDTO>>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);

            }
        } 
        #endregion

        #region GetJoinedRooms
        public async Task<Response<IEnumerable<RoomDTO>>> GetJoinedRooms(string id)
        {
            try
            {
                var joinedRooms = await _context.Rooms.Include(r=>r.UserRooms)
                     .Where(r => r.UserRooms.Any(r=>r.UserId==id))
                     .Select(r => new RoomDTO
                     {
                         Name = r.Name,
                         Description = r.Description,
                         Capacity = r.Capacity,
                         CoverPicture = r.CoverPicture,
                         NumberOfParticipants = r.UserRooms.Count(),
                         CreatedAt = r.CreatedAt,
                         RoomType = r.RoomType,
                     })
                    .ToListAsync();

                return joinedRooms.Any() ? Response<IEnumerable<RoomDTO>>.Success(joinedRooms, "", 200) :
                                           Response<IEnumerable<RoomDTO>>.Failure(new List<RoomDTO>(), "You have not joined any room yet.", 404);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<RoomDTO>>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
            }
        } 
        #endregion

        #region JoinToRoom
        public async Task<Response<string>> JoinToRoom(JoinRoomDTO model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.RoomId))
                {
                    return Response<string>.Failure("Invalid Data Model", 400);
                }
                var roomExist = await _unitOfWork.Rooms.GetByIdAsync(model.RoomId);
                var userExist = await _unitOfWork.Users.GetByIdAsync(model.UserId);
                
                if (roomExist == null || userExist == null)
                {
                    return Response<string>.Failure($"Invalid Data", 400);
                }

                var isUserInRoom = await _unitOfWork.UserRoom
                    .GetByExpressionAsync(u => u.UserId == model.UserId && u.RoomId == model.RoomId);
                if (isUserInRoom != null)
                {
                    return Response<string>.Failure($"You Already Joined To This Room Before!", 400);
                }
                var joinRoom = await _unitOfWork.UserRoom.AddAsync(new UserRoom
                {
                    RoomId = model.RoomId,
                    UserId = model.UserId,
                });
                await _unitOfWork.SaveAsync();
                return Response<string>.Success($"You have successfully to joined the room ", "You have successfully joined to the room ", 201);

            }
            catch (Exception ex)
            {
                return Response<string>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
            }
        }
        #endregion


        #region Create Room
        public async Task<Response<RoomDTO>> CreateRoom(RoomDTO model, IFile CoverPicture)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (model == null)
                {
                    return Response<RoomDTO>.Failure("Invalid Data Model", 400);
                }
                // current login User  
                var currentUser = await _userService.GetCurrentUserAsync();
                var CoverPicturePath = await _cloudinaryService.UploadFileAsync(CoverPicture, "RoomsCoverPictures");
                var room = new Room
                {
                    Name = model.Name,
                    Description = model.Description,
                    RoomType = model.RoomType,
                    CoverPicture = CoverPicturePath,
                    CreatedAt = DateTime.UtcNow,
                    Capacity = model.Capacity,
                    CreatorId = model.CreatorId, // replace with ==> _userService.GetCurrentUserAsync()
                };

                var validate = new RoomValidator().Validate(room);

                if (!validate.IsValid)
                {
                    var errorMessages = string.Join(", ", validate.Errors.Select(e => e.ErrorMessage));
                    return Response<RoomDTO>.Failure(new RoomDTO(), $"Invalid Data Model: {errorMessages}", 400);
                }

                await _unitOfWork.Rooms.AddAsync(room);
                await _unitOfWork.SaveAsync();

                if (model.SubTracks != null && model.SubTracks.Any())
                {
                    var roomSubTracks = model.SubTracks.Select(subTrack => new RoomTrack
                    {
                        RoomId = room.Id,
                        SubTrackId = subTrack,
                    });
                    await _unitOfWork.RoomTracks.AddRangeAsync(roomSubTracks);
                }
                if (model.Skills != null && model.Skills.Any())
                {
                    var roomSkills = model.Skills.Select(skill => new RoomSkills
                    {
                        RoomId = room.Id,
                        SkillId = skill,
                    });
                    await _unitOfWork.RoomSkills.AddRangeAsync(roomSkills);
                }
                await _unitOfWork.SaveAsync();

                await transaction.CommitAsync();

                var roomDto = _mapper.Map<RoomDTO>(room);

                return Response<RoomDTO>.Success(roomDto, "Room Added Successfully", 201);

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<RoomDTO>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
            }

        }
        #endregion
    }
}

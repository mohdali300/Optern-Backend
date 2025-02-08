using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Optern.Application.DTOs.Room;
using Optern.Application.DTOs.Skills;
using Optern.Application.DTOs.Position;
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
using Optern.Application.Interfaces.IRoomTrackService;
using Optern.Application.Interfaces.ISkillService;
using Optern.Application.Interfaces.IRoomSkillService;
using Optern.Application.Interfaces.IRepositoryService;

namespace Optern.Application.Services.RoomService
{
	public class RoomService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper, IUserService userService,
		ICloudinaryService cloudinaryService, IRoomPositionService roomPositionService, IRoomTrackService roomTrackService, ISkillService skillService, IRoomSkillService roomSkillService, IRepositoryService repositoryService) : IRoomService
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly OpternDbContext _context = context;
		private readonly IMapper _mapper = mapper;
		private readonly IUserService _userService = userService;
		private readonly ICloudinaryService _cloudinaryService = cloudinaryService;
		private readonly IRoomPositionService _roomPositionService = roomPositionService;
		private readonly IRoomTrackService _roomTrackService= roomTrackService;
		private readonly ISkillService _skillService= skillService;
		private readonly IRoomSkillService _roomSkillService = roomSkillService;
		private readonly IRepositoryService _repositoryService = repositoryService;

		#region GetAllAsync
		public async Task<Response<IEnumerable<ResponseRoomDTO>>> GetAllAsync()
		{
			try
			{
				var rooms = await _unitOfWork.Rooms.GetAllAsync();
				if (!rooms.Any())
				{
					return Response<IEnumerable<ResponseRoomDTO>>.Failure("No Rooms Found", 404);
				}
				var roomsDtos = _mapper.Map<IEnumerable<ResponseRoomDTO>>(rooms);

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
					.Take(4)
					 .Select(r => new ResponseRoomDTO
					 {
						 Id = r.Room.Id,
						 Name = r.Room.Name,
						 Description = r.Room.Description,
						 CoverPicture = r.Room.CoverPicture,
						 Members = r.NumberOfUsers,
						 CreatedAt = r.Room.CreatedAt,
						 
					 })
					 .ToListAsync();

				return rooms.Any() ? Response<IEnumerable<ResponseRoomDTO>>.Success(rooms, "", 200) :
									 Response<IEnumerable<ResponseRoomDTO>>.Failure(new List<ResponseRoomDTO>(), "There Are No Created Rooms Until Now", 404);
			}
			catch (Exception ex)
			{
				return Response<IEnumerable<ResponseRoomDTO>>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
			}
		}
		#endregion

		#region GetCreatedRooms
		public async Task<Response<IEnumerable<ResponseRoomDTO>>> GetCreatedRooms(string id)
		{
			try
			{
				var createdRooms = await _context.Rooms
					.Include(r => r.UserRooms)
					.Where(r => r.CreatorId == id)
					.Select(r => new ResponseRoomDTO
					{
						Id=r.Id,
						Name = r.Name,
						Description = r.Description,
						CoverPicture = r.CoverPicture.ToString(),
						Members = r.UserRooms.Count(),
						CreatedAt = r.CreatedAt,
						RoomType = r.RoomType,
					})
					.ToListAsync();

				return createdRooms.Any() ? Response<IEnumerable<ResponseRoomDTO>>.Success(createdRooms, "", 200) :
											Response<IEnumerable<ResponseRoomDTO>>.Failure(new List<ResponseRoomDTO>(), "There is no Created Rooms Until Now", 404);

			}
			catch (Exception ex)
			{
				return Response<IEnumerable<ResponseRoomDTO>>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);

			}
		}
		#endregion

		#region GetJoinedRooms
		public async Task<Response<IEnumerable<ResponseRoomDTO>>> GetJoinedRooms(string id)
		{
			try
			{
				var joinedRooms = await _context.Rooms.Include(r => r.UserRooms)
					 .Where(r => r.UserRooms.Any(r => r.UserId == id))
					 .Select(r => new ResponseRoomDTO
					 {
						 Id= r.Id,
						 Name = r.Name,
						 Description = r.Description,
						 CoverPicture = r.CoverPicture,
						 Members = r.UserRooms.Count(),
						 CreatedAt = r.CreatedAt,
						 RoomType = r.RoomType,
					 })
					.ToListAsync();

				return joinedRooms.Any() ? Response<IEnumerable<ResponseRoomDTO>>.Success(joinedRooms, "", 200) :
										   Response<IEnumerable<ResponseRoomDTO>>.Failure(new List<ResponseRoomDTO>(), "You have not joined any room yet.", 404);
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
					IsAdmin = false ,
					JoinedAt=DateTime.UtcNow,
					IsAccepted=false
				});
				await _unitOfWork.SaveAsync();
				await transaction.CommitAsync();
				return Response<string>.Success($"You have successfully to joined the room ", "You have successfully joined to the room ", 201);

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
				var room = new Room
				{
					Name = model.Name,
					Description = model.Description,
					RoomType = model.RoomType,
					CreatedAt = DateTime.UtcNow,
					CreatorId = model.CreatorId, // replace with ==> _userService.GetCurrentUserAsync()
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
					await _roomPositionService.AddRoomPosition(room.Id,model.Positions);
				}
				if(model.Tracks != null && model.Tracks.Any())
				{
					await _roomTrackService.AddRoomTrack(room.Id,model.Tracks);
				}
				if (model.Skills != null && model.Skills.Any())
				{
				   await ManageSkillOperationistRoomCreation(room,model.Skills);
				}
	
				await _repositoryService.AddRepository(room.Id); // add Repository for Room By Default while creation process


				await _unitOfWork.UserRoom.AddAsync(new UserRoom {
					UserId = room.CreatorId,  // replace with ==> _userService.GetCurrentUserAsync()
					RoomId = room.Id,
					IsAdmin = true ,
					JoinedAt=DateTime.UtcNow,
					IsAccepted=true
				});
				await _unitOfWork.SaveAsync();

				await transaction.CommitAsync();

				var roomDto = _mapper.Map<ResponseRoomDTO>(room);
			
				return Response<ResponseRoomDTO>.Success(roomDto, "Room Added Successfully", 201);

			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				return Response<ResponseRoomDTO>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
			}
		}
		#endregion

		public async Task<Response<ResponseRoomDTO>> GetRoomById(string id,string? userId)
		{
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
						UserStatus = room.UserRooms.Any(r => r.UserId == userId && r.IsAccepted == true)? 
						UserRoomStatus.Accepted:room.UserRooms.Any(r=>r.UserId == userId)?UserRoomStatus.Requested:UserRoomStatus.NONE,
						Description = room.Description,
						RoomType = room.RoomType,
						CreatorId = room.CreatorId,
						CoverPicture = room.CoverPicture,
						CreatedAt = room.CreatedAt,
						Members = room.UserRooms.Count(r=>r.IsAccepted == true),
						Skills = room.RoomSkills
						.Select(rs => new SkillDTO {
							Id=rs.Skill.Id,
							Name=rs.Skill.Name
						}).Distinct().ToList(),
						Tracks=room.RoomTracks
						.Select(track => new TrackDTO
						{
							Id = track.Track.Id,
							Name = track.Track.Name
						}).Distinct().ToList(),
						Position=room.RoomPositions
						.Select(position=> new PositionDTO 
						{ Id=position.Position.Id,
							Name=position.Position.Name
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

		// Helper Function

		#region Helper Function For Manage Skills in Room
		private async Task<bool> ManageSkillOperationistRoomCreation(Room room, IEnumerable<SkillDTO> data)
		{
			var existingSkills = await _context.Skills
						.Select(s => new SkillDTO
						{
							Id = s.Id,
							Name = s.Name,
						})
						.ToListAsync();

			var newSkills = data
				  .Where(skill => !existingSkills.Any(es => es.Name.ToLower() == skill.Name.ToLower()))
				  .Select(skill => new SkillDTO
				  {
					  Name = skill.Name,
				  })
				  .ToList();

			if (newSkills.Any())
			{
				var skill = await _skillService.AddSkills(newSkills);
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

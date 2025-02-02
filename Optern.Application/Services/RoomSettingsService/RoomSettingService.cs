using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs.Room;
using Optern.Application.DTOs.Skills;
using Optern.Application.DTOs.Position;
using Optern.Application.Interfaces.IRoomService;
using Optern.Application.Interfaces.IRoomSettingService;
using Optern.Application.Interfaces.IUserService;
using Optern.Application.Services.UserService;
using Optern.Domain.Entities;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.ExternalInterfaces.IFileService;
using Optern.Infrastructure.ExternalServices.FileService;
using Optern.Infrastructure.Response;
using Optern.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optern.Application.Interfaces.ISkillService;
using Optern.Application.Interfaces.IRoomSkillService;

namespace Optern.Application.Services.RoomSettings
{
    public class RoomSettingService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper, IUserService userService,
        ICloudinaryService cloudinaryService, IRoomService roomService, ISkillService skillService, IRoomSkillService roomSkillService) : IRoomSettingService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IUserService _userService = userService;
        private readonly ICloudinaryService _cloudinaryService = cloudinaryService;
        private readonly IRoomService _roomService = roomService;
        private readonly ISkillService  _skillService = skillService;
        private readonly IRoomSkillService _roomSkillService = roomSkillService;


        #region EditRoom Settings
        public async Task<Response<string>> EditRoom(string id, EditRoomDTO model, IFile? CoverPicture)
        {
            if (model == null)
            {
                return Response<string>.Failure("Invalid Data Model", 400);
            }

            var room = await _context.Rooms
                .Include(r => r.RoomPositions)
                .Include(r=>r.RoomTracks)
                .Include(r => r.RoomSkills)
                .ThenInclude(s => s.Skill)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (room == null)
            {
                return Response<string>.Failure("Room Not Found!", 404);
            }

            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                room.Name = model.Name ?? room.Name;
                room.RoomType = model.RoomType ?? room.RoomType;
                room.Description = model.Description ?? room.Description;

                if (CoverPicture != null)
                {
                    var newCoverPicture = await _cloudinaryService.UploadFileAsync(CoverPicture, "RoomsCoverPictures");
                    if (!string.IsNullOrEmpty(newCoverPicture))
                    {
                        room.CoverPicture = newCoverPicture;
                    }
                }
                await _unitOfWork.Rooms.UpdateAsync(room);
                if (model.Positions != null)
                {
                    await UpdateRoomPositions(room, model.Positions);
                }
                if (model.Skills != null)
                {
                    await UpdateRoomSkills(room, model.Skills);
                }
                if (model.Tracks != null) {
                   await UpdateRoomTracks(room, model.Tracks);
                }
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return Response<string>.Success("Room Updated Successfully", "Updated Successfully", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<string>.Failure($"There is a server error. Please try again later. {ex.Message}", 500);
            }
        } 
        #endregion

        #region Delete Room
        public async Task<Response<bool>> DeleteRoom(string roomId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
                if (room == null)
                {
                    return Response<bool>.Failure(false, "Room not Found !", 404);
                }
                await _unitOfWork.Rooms.DeleteAsync(room);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                return Response<bool>.Success(true, "Room Deleted Successfully", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
            }
        } 
        #endregion

        #region Update Skills For Room
        private async Task<bool> UpdateRoomSkills(Room room, List<SkillDTO>? newSkills)
        {
            if (newSkills == null || !newSkills.Any())
            {
                return false;
            }

            if (room.RoomSkills == null)
             {
                room.RoomSkills = new List<RoomSkills>();
             }

            var roomSkillsIds = room.RoomSkills.Select(rs => rs.SkillId).ToHashSet();
            var newSkillNames = newSkills.Select(s => s.Name).ToHashSet(); 

            var existingSkills = await _context.Skills
                .Where(s => newSkillNames.Contains(s.Name))
                .ToListAsync(); 

            var existingSkillIdsInDb = existingSkills.Select(s => s.Id).ToHashSet(); 

            var notExistingSkills = room.RoomSkills.Where(rs => !newSkillNames.Contains(rs.Skill.Name)).ToList(); 
            foreach (var skill in notExistingSkills)
            {
                await _unitOfWork.RoomSkills.DeleteAsync(skill); 
            }

            foreach (var item in newSkills)
            {
                Skills skill;

                if (existingSkillIdsInDb.Contains(item.Id))
                {
                    skill = existingSkills.First(s => s.Id == item.Id);
                }
                else
                {
                    skill = new Skills { Name = item.Name };
                    await _unitOfWork.Skills.AddAsync(skill);
                    await _unitOfWork.SaveAsync();
                }

                if (!roomSkillsIds.Contains(skill.Id))
                {
                    var newRoomSkill = new RoomSkills { SkillId = skill.Id, RoomId = room.Id };
                    await _unitOfWork.RoomSkills.AddAsync(newRoomSkill);
                }
            }

            await _unitOfWork.SaveAsync();
            return true;
        }

        #endregion

        #region Update Positions For Room
        private async Task<bool> UpdateRoomPositions(Room room, List<int>? newPositions)
        {
            if (newPositions == null || !newPositions.Any()) 
                return false;

            var existingPositionIds = room.RoomPositions.Select(rt => rt.PositionId).ToHashSet();
            var newPositionIds = newPositions.ToHashSet();

            var notExistingPositions = room.RoomPositions.Where(rt => !newPositionIds.Contains(rt.PositionId)).ToList();
            foreach (var position in notExistingPositions)
            {
                await _unitOfWork.RoomPositions.DeleteAsync(position);
            }

            foreach (var item in newPositionIds)
            {
                if (!existingPositionIds.Contains(item))
                {
                    var newRoomTrack = new RoomPosition { PositionId = item, RoomId = room.Id };
                    await _unitOfWork.RoomPositions.AddAsync(newRoomTrack);
                }
            }

            await _unitOfWork.SaveAsync();
            return true;
        } 
        #endregion

        public async Task<bool> UpdateRoomTracks(Room room,IEnumerable<int> newTracks)
        {
            if(newTracks==null || !newTracks.Any())
            {
                return false;
            }
            var existingRoomsTracks= room.RoomTracks.Select(roomTrack=>roomTrack.TrackId).ToHashSet();
            var newTracksIds= newTracks.ToHashSet(); 
            var notExistedRoomsTracks = room.RoomTracks.Where(room => !newTracksIds.Contains(room.TrackId)).ToList(); 

                foreach (var roomTrack in notExistedRoomsTracks)
                {
                    await _unitOfWork.RoomTracks.DeleteAsync(roomTrack);
                }
                
                foreach(var roomTrack in newTracksIds) 
                {
                if (!existingRoomsTracks.Contains(roomTrack)) 
                {
                    await _unitOfWork.RoomTracks.AddAsync(new RoomTrack { RoomId = room.Id, TrackId = roomTrack });
                }
               }
                await _unitOfWork.SaveAsync();
            return true;
        }


    }
}

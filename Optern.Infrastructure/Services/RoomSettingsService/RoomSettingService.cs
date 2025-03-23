namespace Optern.Infrastructure.Services.RoomSettings
{
    public class RoomSettingService(IUnitOfWork unitOfWork, OpternDbContext context, ICloudinaryService cloudinaryService,
        ISkillService skillService, IRoomSkillService roomSkillService, IRoomPositionService roomPositionService, IRoomTrackService roomTrackService, IChatService chatService) : IRoomSettingService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly ICloudinaryService _cloudinaryService = cloudinaryService;
        private readonly ISkillService _skillService = skillService;
        private readonly IRoomSkillService _roomSkillService = roomSkillService;
        private readonly IRoomPositionService _roomPositionService = roomPositionService;
        private readonly IRoomTrackService _roomTrackService = roomTrackService;
        private readonly IChatService _chatService = chatService;

        #region EditRoom Settings

        public async Task<Response<string>> EditRoom(string id, EditRoomDTO model)
        {
            if (model == null || string.IsNullOrEmpty(id))
            {
                return Response<string>.Failure("Invalid Data Model", 400);
            }

            var room = await _context.Rooms
                .Include(r => r.RoomPositions)
                .Include(r => r.RoomTracks)
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
                await _unitOfWork.Rooms.UpdateAsync(room);
                if (model.Positions != null)
                {
                    await UpdateRoomPositions(room, model.Positions);
                }
                if (model.Skills != null)
                {
                    await UpdateRoomSkills(room, model.Skills);
                }
                if (model.Tracks != null)
                {
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

        public async Task<Response<string>> EditRoomImage(string roomId, [GraphQLType(typeof(UploadType))] IFile? CoverPicture)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
                if (room == null)
                {
                    return Response<string>.Failure("Room Not Found!", 404);
                }
                if (CoverPicture != null)
                {
                    var (publicID, newCoverPicture) = await _cloudinaryService.UploadFileAsync(CoverPicture, "RoomsCoverPictures");
                    if (!string.IsNullOrEmpty(newCoverPicture))
                    {
                        room.CoverPicture = newCoverPicture;
                    }
                }
                await _unitOfWork.Rooms.UpdateAsync(room);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                return Response<string>.Success($"{room.CoverPicture}", "Room Image Updated Successfully", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<string>.Failure($"There is a server error. Please try again later. {ex.Message}", 500);
            }
        }

        #endregion EditRoom Settings

        #region Delete Room

        public async Task<Response<bool>> DeleteRoom(string roomId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
                if (room == null)
                {
                    return Response<bool>.Failure(false, "Room not Found!", 404);
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

        #endregion Delete Room

        #region Reset Room

        public async Task<Response<bool>> ResetRoom(string roomId)
        {
            var room = await _unitOfWork.Rooms.GetByIdWithIncludeAsync(roomId, r => r.RoomTracks, r => r.RoomSkills, r => r.RoomPositions);
            if (room == null)
            {
                return Response<bool>.Failure(false, "Room not Found !", 404);
            }
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                room.CoverPicture = string.Empty;
                room.Name = string.Empty;
                room.Description = string.Empty;
                if (room.RoomTracks.Any())
                {
                    await _unitOfWork.RoomTracks.DeleteRangeAsync(room.RoomTracks);
                }
                if (room.RoomSkills.Any())
                {
                    await _unitOfWork.RoomSkills.DeleteRangeAsync(room.RoomSkills);
                }
                if (room.RoomPositions.Any())
                {
                    await _unitOfWork.RoomPositions.DeleteRangeAsync(room.RoomPositions);
                }
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                return Response<bool>.Success(true, "Room Reseted Successfully", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
            }
        }

        #endregion Reset Room

        #region Leave Room

        public async Task<Response<bool>> LeaveRoomAsync(string roomId, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var userRoom = await _context.UserRooms
                    .Include(ur => ur.Room)
                    .Include(ur => ur.User)
                    .FirstOrDefaultAsync(ur => ur.RoomId == roomId && ur.UserId == userId);

                if (userRoom == null)
                {
                    return Response<bool>.Failure(false, "User is not in this room", 404);
                }

                var totalMembers = await _context.UserRooms.CountAsync(ur => ur.RoomId == roomId);

                if (userRoom.IsAdmin)
                {
                    if (totalMembers > 1)
                    {
                        var remainingAdmins = await _context.UserRooms.CountAsync(ur => ur.RoomId == roomId && ur.IsAdmin && ur.UserId != userId);
                        if (remainingAdmins == 0)
                        {
                            return Response<bool>.Failure(false, "You must specify a new admin before leaving as the last admin", 400);
                        }
                    }
                    else
                    {
                        _context.Rooms.Remove(userRoom.Room);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return Response<bool>.Success(true, "You left the room and the room was deleted because it became empty", 200);
                    }
                }

                _context.UserRooms.Remove(userRoom);
                await _chatService.RemoveFromRoomChatAsync(userRoom.Room.ChatId, userId); // remove from room chat
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return Response<bool>.Success(true, "Successfully left the room", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure(false, $"An error occurred while leaving the room: {ex.Message}", 500);
            }
        }

        #endregion Leave Room

        // Helpers Functions

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

            var existingSkillINamesInDb = existingSkills.Select(s => s.Name).ToHashSet();

            var notExistingSkills = room.RoomSkills
                .Where(rs => !newSkillNames.Contains(rs.Skill.Name))
                .ToList();

            foreach (var skill in notExistingSkills)
            {
                var roomSkills = new List<RoomSkills> { new RoomSkills { SkillId = skill.SkillId } };
                await _roomSkillService.DeleteRoomSkills(room.Id, roomSkills.Select(s => s.SkillId).ToList());
            }

            foreach (var item in newSkills)
            {
                Skills skill;

                if (existingSkillINamesInDb.Contains(item.Name))
                {
                    skill = existingSkills.First(s => s.Name == item.Name);
                }
                else
                {
                    var newSkillList = new List<SkillDTO> { new SkillDTO { Name = item.Name } };
                    var skillResponse = await _skillService.AddSkills(newSkillList);

                    if (skillResponse.Data != null && skillResponse.Data.Any())
                    {
                        skill = new Skills { Id = skillResponse.Data.First().Id, Name = skillResponse.Data.First().Name };
                    }
                    else
                    {
                        return false;
                    }
                }

                if (!roomSkillsIds.Contains(skill.Id))
                {
                    var newRoomSkill = new List<RoomSkills> { new RoomSkills { SkillId = skill.Id } };
                    var roomSkillResponse = await _roomSkillService.AddRoomSkills(room.Id, newRoomSkill.Select(s => s.SkillId).ToList());
                }
            }

            return true;
        }

        #endregion Update Skills For Room

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
                await _roomPositionService.DeleteRoomPosition(position.RoomId, position.PositionId);
            }

            foreach (var item in newPositionIds)
            {
                if (!existingPositionIds.Contains(item))
                {
                    var newRoomTrack = new List<RoomPosition> { new RoomPosition { PositionId = item } };
                    var response = await _roomPositionService.AddRoomPosition(room.Id, newRoomTrack.Select(r => r.PositionId).ToList());
                }
            }
            return true;
        }

        #endregion Update Positions For Room

        #region Update Tracks For Room

        public async Task<bool> UpdateRoomTracks(Room room, IEnumerable<int> newTracks)
        {
            if (newTracks == null || !newTracks.Any())
            {
                return false;
            }
            var existingRoomsTracks = room.RoomTracks.Select(roomTrack => roomTrack.TrackId).ToHashSet();
            var newTracksIds = newTracks.ToHashSet();
            var notExistedRoomsTracks = room.RoomTracks.Where(room => !newTracksIds.Contains(room.TrackId)).ToList();

            foreach (var roomTrack in notExistedRoomsTracks)
            {
                await _roomTrackService.DeleteRoomTrack(roomTrack.RoomId, roomTrack.TrackId);
            }

            foreach (var roomTrack in newTracksIds)
            {
                if (!existingRoomsTracks.Contains(roomTrack))
                {
                    var roomTrakcs = new List<RoomTrack> { new RoomTrack { TrackId = roomTrack } };
                    var response = await _roomTrackService.AddRoomTrack(room.Id, roomTrakcs.Select(r => r.TrackId).ToList());
                }
            }
            await _unitOfWork.SaveAsync();
            return true;
        }

        #endregion Update Tracks For Room
    }
}
﻿

namespace Optern.Infrastructure.Services.RoomSkillService
{
    public class RoomSkillService(IUnitOfWork unitOfWork, OpternDbContext context) : IRoomSkillService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;

        #region Add Room Skills
        public async Task<Response<bool>> AddRoomSkills(string roomID, IEnumerable<int> data)
        {
            if (data == null || !data.Any())
            {
                return Response<bool>.Failure(false, "Invalid Data Model", 400);
            }  
            try
            {
                var isRoomExist= await _unitOfWork.Rooms.GetByIdAsync(roomID);
                if (isRoomExist == null) {
                    return Response<bool>.Failure(false, "Room Not Found", 404);
                }
                var roomSkills = data.Select(roomSkill => new RoomSkills
                {
                    SkillId=roomSkill,
                    RoomId=roomID,  
                });
                await _unitOfWork.RoomSkills.AddRangeAsync(roomSkills);
                await _unitOfWork.SaveAsync();
                return Response<bool>.Success(true, "RoomSkills Added Successfully", 201);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure(false,$"There is a server error. Please try again later.{ex.Message}", 500);
            }
         }
        #endregion


        #region Delete Room Skills
        public async Task<Response<bool>> DeleteRoomSkills(string roomID, IEnumerable<int> skillIds)
        {
            if (string.IsNullOrEmpty(roomID) || skillIds == null || !skillIds.Any())
            {
                return Response<bool>.Failure(false, "Invalid Data Model", 400);
            }

            try
            {
                var roomSkills = await _unitOfWork.RoomSkills
                    .GetAllByExpressionAsync(rs => rs.RoomId == roomID && skillIds.Contains(rs.SkillId));

                if (!roomSkills.Any())
                {
                    return Response<bool>.Failure(false, "No matching Room Skills found to delete", 404);
                }
                var isRoomExist = await _unitOfWork.Rooms.GetByIdAsync(roomID);
                if (isRoomExist == null)
                {
                    return Response<bool>.Failure(false, "Room Not Found", 404);
                }

                await _unitOfWork.RoomSkills.DeleteRangeAsync(roomSkills);
                await _unitOfWork.SaveAsync();

                return Response<bool>.Success(true, "Room Skills Deleted Successfully", 200);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure(false, $"There is a server error. Please try again later. {ex.Message}", 500);
            }
        }
        #endregion

    }
}

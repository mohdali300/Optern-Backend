

using Optern.Application.DTOs.User;
using System.Collections.Generic;

namespace Optern.Infrastructure.Services.UserService
{
    public class UserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager,IUnitOfWork unitOfWork,OpternDbContext context, ICloudinaryService cloudinary) :IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor= httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager= userManager;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly ICloudinaryService _cloudinary = cloudinary;
        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            ClaimsPrincipal currentUser = _httpContextAccessor.HttpContext.User;
            return await _userManager.GetUserAsync(currentUser);
        }

        public async Task<bool> IsUserExist(string userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            return user == null ? false : true;
        }

        public async Task<Response<bool>> EditProfile(string userId,EditProfileDTO model)
        {
            if (string.IsNullOrEmpty(userId) || model == null)
            {
                return Response<bool>.Failure(false,"Invalid Data",400);
            }

            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<bool>.Failure(false, "User Not Found", 404);
                }

                user.FirstName = model.FirstName ?? user.FirstName;
                user.LastName= model.LastName ?? user.LastName;
                user.JobTitle= model.JobTitle ?? user.JobTitle;
                user.Gender = model.Gender ?? user.Gender;
                user.Country = model.Country ?? user.Country;
                user.AboutMe= model.AboutMe ?? user.AboutMe;

                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                return Response<bool>.Success(true,"Profile Information Updated Successfully",200);

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure(false, $"An error occurred while processing the react: {ex.Message}",500);
            }

        }

        public async Task<Response<bool>> EditProfilePicture(string userId, IFile picture)
        {
            if (string.IsNullOrEmpty(userId) || picture == null)
            {
                return Response<bool>.Failure(false, "Invalid Data", 400);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<bool>.Failure(false, "User Not Found", 404);
                }

                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    var publicId = string.Join("/", user.ProfilePicture.Split('/').Skip(user.ProfilePicture.Split('/').Length - 2));
                    await _cloudinary.DeleteFileAsync(publicId);
                }

                var imagePath = await _cloudinary.UploadFileAsync(picture, "userProfilePicture");
                user.ProfilePicture = imagePath.Url ?? user.ProfilePicture;

                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return Response<bool>.Success(true, "Profile Picture Updated Successfully", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure(false, $"An error occurred: {ex.Message}", 500);
            }
        }

        public async Task<Response<IEnumerable<UserDTO>>> GetAll()
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllAsync();

                if (users == null || !users.Any())
                {
                    return Response<IEnumerable<UserDTO>>.Failure(Enumerable.Empty<UserDTO>(), "No users found", 404);
                }

                return Response<IEnumerable<UserDTO>>.Success(
                    users.Select(u => new UserDTO
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        UserName = u.UserName
                    }),
                    "Users fetched successfully",
                    200
                );
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<UserDTO>>.Failure($"Server error: {ex.Message}", 500);
            }
        }

    }
}

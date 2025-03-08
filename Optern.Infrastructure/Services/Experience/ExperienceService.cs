using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optern.Application.DTOs.ExperienceDTO;
using Optern.Application.Interfaces.IExperienceService;
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Services.Experience
{
    public class ExperienceService(IUnitOfWork unitOfWork, OpternDbContext context) : IExperienceService
    {
        private readonly IUnitOfWork _unitOfWork= unitOfWork;
        private readonly OpternDbContext _context= context;

        public async Task<Response<bool>> AddExperience(string userId, ExperienceDTO model)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Response<bool>.Failure(false,"Invalid UserId",400);
            }

            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<bool>.Failure(false, "User Not Found", 404);
                }

                var experience = new Domain.Entities.Experience()
                { 
                    JobTitle = model.JobTitle,
                    Company = model.Company,
                    StartDate = model.StartDate,
                    EndDate = model.IsCurrentlyWork == true ? null : model.EndDate,
                    IsCurrentlyWork = model.IsCurrentlyWork??false, 
                    JobDescription = model.JobDescription,
                    Location = model.Location,
                    UserId = user.Id
                };
                var validator = new ExperienceValidator().Validate(experience);
                if (!validator.IsValid)
                {
                    var errorMessages = string.Join(", ", validator.Errors.Select(e => e.ErrorMessage));
                    return Response<bool>.Failure(false, $"Invalid Data Model: {errorMessages}", 400);
                }

                await _unitOfWork.Experience.AddAsync(experience);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                return Response<bool>.Success(true, "Experience Added Successfully", 201);

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure(false,$"There is a server error. Please try again later.{ex.Message}", 500);
            }
        }


        public async Task<Response<IEnumerable<ExperienceDTO>>> GetUserExperiences(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return  Response<IEnumerable<ExperienceDTO>>.Failure(new List<ExperienceDTO>(), "Invalid UserId", 400);
            }
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Response<IEnumerable<ExperienceDTO>>.Failure(new List<ExperienceDTO>(), "User Not Found", 404);
                }

                var userExperiences = await _context.Experiences.Where(e => e.UserId == userId).Select(e=>new ExperienceDTO()
                {
                    Id = e.Id,
                    JobTitle = e.JobTitle,
                    Company = e.Company,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    IsCurrentlyWork = e.IsCurrentlyWork,
                    JobDescription = e.JobDescription,
                    Location = e.Location
                })
                    .AsNoTracking()
                    .ToListAsync();

                return !userExperiences.Any()
                    ? Response<IEnumerable<ExperienceDTO>>.Success(new List<ExperienceDTO>(),
                        "User Has No Experiences yet", 204)
                    : Response<IEnumerable<ExperienceDTO>>.Success(userExperiences,
                        " Experiences Fetched Successfully ", 200);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<ExperienceDTO>>.Failure(new List<ExperienceDTO>(), $"There is a server error. Please try again later.{ex.Message}", 500);
            }
        }

    }
}

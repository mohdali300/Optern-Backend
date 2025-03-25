
using Optern.Application.DTOs.VFeedback;
using Optern.Application.Interfaces.IVFeedbackService;
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Services.VFeedbackService
{
    public class VFeedbackService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper) : IVFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        #region Add VFeedback

        public async Task<Response<string>> AddVFeedback(VFeedbackDTO model)
        {

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (!await IsInterviewExist(model.VirtualInterviewId))
                {
                    return Response<string>.Failure("", "Interview Not Found", 404);
                }
                var feedback = await _context.VFeedBack.FirstOrDefaultAsync(f => f.VirtualInterviewId == model.VirtualInterviewId);

                if (feedback is not null && feedback.Recommendations is null)
                {
                    feedback.Recommendations = model.Recommendations;
                }
                else 
                {
                    var feed = _mapper.Map<VFeedBack>(model);
                    await _unitOfWork.VFeedBack.AddAsync(feed);
                }

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return Response<string>.Success("Feedback submitted successfully", "Feedback added successfully.", 200);
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return Response<string>.Failure("Database error occurred while adding feedback.", dbEx.Message, 500);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<string>.Failure("An unexpected error occurred while adding feedback.", ex.Message, 500);
            }
        }

        #endregion

        #region Get VFeedback
        public async Task<Response<VFeedbackDTO>> GetVFeedback(int vInterviewId)
        {
            try
            {
                if (vInterviewId <= 0)
                {
                    return Response<VFeedbackDTO>.Failure(new VFeedbackDTO(), "Invalid interview ID", 400);
                }

                if (!await IsInterviewExist(vInterviewId))
                {
                    return Response<VFeedbackDTO>.Failure(new VFeedbackDTO(), "Interview not found", 404);
                }

                var feedback = await _unitOfWork.VFeedBack
                    .GetByExpressionAsync(f => f.VirtualInterviewId == vInterviewId);

                if (feedback == null)
                {
                    return Response<VFeedbackDTO>.Failure(new VFeedbackDTO(), "No feedback found for this interview", 404);
                }

                var feedbackDTO = _mapper.Map<VFeedbackDTO>(feedback);
                return Response<VFeedbackDTO>.Success(feedbackDTO, "Feedback retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                return Response<VFeedbackDTO>.Failure(new VFeedbackDTO(), ex.Message, 500);
            }
        }

        #endregion

        #region Helpers
        private async Task<bool> IsInterviewExist(int vInterviewId)
        {
            return await _unitOfWork.VInterviews.AnyAsync(i => i.Id == vInterviewId);
        }
        #endregion
    }
}

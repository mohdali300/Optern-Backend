
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Services.PTPFeedbackService
{
    public class PTPFeedbackService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper) : IPTPFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        #region Add PTP FeedBack

        public async Task<Response<string>> AddPTPFeedback(AddPTPFeedbackDTO model)
        {
            if (string.IsNullOrEmpty(model.UserId) || model.PTPInterviewId <= 0
                || string.IsNullOrEmpty(model.InterviewerPerformance) || string.IsNullOrEmpty(model.Improvement))
            {
                return Response<string>.Failure("", "The Fields Can't Be Empty", 400);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                if (!await IsInterviewExist(model.PTPInterviewId))
                {
                    return Response<string>.Failure("", "Interview Not Found", 404);

                }
                var givenByUser = await _unitOfWork.PTPUsers
                    .GetByExpressionAsync(u => u.UserID == model.UserId && u.PTPIId == model.PTPInterviewId);

                if (givenByUser == null)
                {
                    return Response<string>.Failure("", "User not found in That Interview", 404);
                }


                var partnerUser = await _unitOfWork.PTPUsers
                    .GetByExpressionAsync(u => u.PTPIId == model.PTPInterviewId && u.UserID != model.UserId);

                if (partnerUser == null)
                {
                    return Response<string>.Failure("", "Interview partner not found", 404);
                }

                if (await HasUserGivenFeedback(model.PTPInterviewId, model.UserId))
                {
                    return Response<string>.Failure("", "You Already Gave Your Feedback", 400);
                }

                var feedback = _mapper.Map<PTPFeedBack>(model);
                feedback.GivenByUserId = givenByUser.Id;
                feedback.ReceivedByUserId = partnerUser.Id;
                feedback.PTPInterviewId = model.PTPInterviewId;

                await _unitOfWork.PTPFeedBack.AddAsync(feedback);
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

        #region Get PTPFeedback
        public async Task<Response<PTPFeedbackDTO>> GetPTPFeedback(int ptpInterviewId, string userId)
        {
            try
            {
                if ( ptpInterviewId <= 0 || string.IsNullOrEmpty(userId))
                {
                    return Response<PTPFeedbackDTO>.Failure(new PTPFeedbackDTO(), "Invalid parameters: ptpInterviewId and userId are required.", 400);

                }
                if (!await IsInterviewExist(ptpInterviewId))
                {
                    return Response<PTPFeedbackDTO>.Failure(new PTPFeedbackDTO(), "Interview not found", 404);
                }

                var userInInterview = await _unitOfWork.PTPUsers
                    .GetByExpressionAsync(u => u.PTPIId == ptpInterviewId && u.UserID == userId);

                if (userInInterview == null)
                {
                    return Response<PTPFeedbackDTO>.Failure(new PTPFeedbackDTO(), "User not found in this interview", 404);
                }

                var feedback = await _unitOfWork.PTPFeedBack
                                     .GetByExpressionAsync(f => f.PTPInterviewId == ptpInterviewId &&
                                      f.ReceivedByUserId == userInInterview.Id);

                if (feedback == null )
                {
                    return Response<PTPFeedbackDTO>.Failure(new PTPFeedbackDTO(), "No feedback found for this interview and user", 404);
                }

                var feedbackDTO = _mapper.Map<PTPFeedbackDTO>(feedback);
                return Response<PTPFeedbackDTO>.Success(feedbackDTO, "Feedback retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                return Response<PTPFeedbackDTO>.Failure(new PTPFeedbackDTO(), ex.Message, 500);
            }
        }

        #endregion


        #region Helpers
        private async Task<bool> HasUserGivenFeedback(int ptpInterviewId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            var givenByUser = await _unitOfWork.PTPUsers
                  .GetByExpressionAsync(u => u.UserID == userId && u.PTPIId == ptpInterviewId);

            var feedbackExists = await _unitOfWork.PTPFeedBack
                .AnyAsync(f => f.PTPInterviewId == ptpInterviewId && f.GivenByUserId == givenByUser.Id);

            return feedbackExists;
        }

        private async Task<bool> IsInterviewExist(int ptpInterviewId)
        {
            var interviewExists = await _unitOfWork.PTPInterviews.AnyAsync(i => i.Id == ptpInterviewId);
            return interviewExists;

        }
    }
        #endregion

    
    }

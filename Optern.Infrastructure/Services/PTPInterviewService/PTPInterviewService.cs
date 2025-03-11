using Optern.Application.DTOs.Question;
using Optern.Domain.Entities;
using Optern.Domain.Extensions;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Reflection.Emit;

namespace Optern.Infrastructure.Services.PTPInterviewService
{
    public class PTPInterviewService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper,ICacheService cacheService) : IPTPInterviewService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ICacheService _cacheService = cacheService;

        #region Get Upcoming PTP Interviews 
        public async Task<Response<IEnumerable<UpcomingPTPInterviewDTO>>> GetAllUpcomingPTPInterviews(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return Response<IEnumerable<UpcomingPTPInterviewDTO>>.Failure(new List<UpcomingPTPInterviewDTO>(), "Invalid parameter: userId is required.", 400);
                }

                var currentTime = DateTime.UtcNow;

                var upcomingInterviews = await _unitOfWork.PTPInterviews
                    .GetAllByExpressionAsync(i => i.PeerToPeerInterviewUsers.Any(u => u.UserID == userId) &&
                                                  i.Status == InterviewStatus.Scheduled || i.Status == InterviewStatus.InProgress);


                //var filteredInterviews = upcomingInterviews
                //       .Where(i => DateTime.TryParse(i.ScheduledDate, out DateTime scheduledDate) &&
                //(scheduledDate.Date > currentTime.Date ||
                // (scheduledDate.Date == currentTime.Date && GetTimeSpanFromEnum(i.ScheduledTime) >= currentTime.TimeOfDay))) 
                //.OrderBy(i => i.ScheduledTime)
                //   .ToList();
              

                if (!upcomingInterviews.Any())
                {
                    return Response<IEnumerable<UpcomingPTPInterviewDTO>>.Failure(new List<UpcomingPTPInterviewDTO>(), "No Upcoming Interviews found", 404);
                }

                var orderedInterviews = upcomingInterviews
                   .OrderBy(i => DateTime.Parse(i.ScheduledDate!))
                   .ThenBy(i => GetTimeSpanFromEnum(i.ScheduledTime))
                    .ToList();

                var upcomingInterviewsDTO = _mapper.Map<List<UpcomingPTPInterviewDTO>>(orderedInterviews);

                foreach (var interviewDTO in upcomingInterviewsDTO)
                {
                    var interviewEntity = upcomingInterviews.FirstOrDefault(i => i.Id == interviewDTO.Id);

                    interviewDTO.ScheduledTime = interviewEntity!.ScheduledTime.GetDisplayName();

                    //if (!DateTime.TryParse(interviewEntity.ScheduledDate, out DateTime scheduledDate))
                    //{
                    //    interviewDTO.TimeRemaining = "Invalid date format";
                    //    continue;
                    //}

                    //DateTime scheduledDateUtc = scheduledDate.ToUniversalTime();
                    //DateTime scheduledDateTimeUtc = scheduledDateUtc.Add(GetTimeSpanFromEnum(interviewEntity.ScheduledTime));
                    //scheduledDateTimeUtc = DateTime.SpecifyKind(scheduledDateTimeUtc, DateTimeKind.Utc);
                    //TimeSpan timeRemaining = scheduledDateTimeUtc - DateTime.UtcNow;
                    //interviewDTO.TimeRemaining = FormatTimeRemaining(timeRemaining);


                    interviewDTO.Questions = await GetUserQuestionsForInterview(interviewEntity.Id, userId);
                }

                return Response<IEnumerable<UpcomingPTPInterviewDTO>>.Success(upcomingInterviewsDTO, "Upcoming interviews retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<UpcomingPTPInterviewDTO>>.Failure(new List<UpcomingPTPInterviewDTO>(), ex.Message, 500);
            }
        }


        #endregion

        #region Create PTP Interview
        public async Task<Response<PTPInterviewDTO>> CreatePTPInterviewAsync(CreatePTPInterviewDTO dto, int questionCount, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!Enum.IsDefined(typeof(InterviewCategory), dto.Category) ||
                    !Enum.IsDefined(typeof(InterviewQuestionType), dto.QuestionType) ||
                    !Enum.IsDefined(typeof(InterviewTimeSlot), dto.ScheduledTime))
                {
                    return Response<PTPInterviewDTO>.Failure(new PTPInterviewDTO(), "Invalid category or question type or time slot", 400);
                }

                var existingInterview = await _context.PTPInterviews
                    .Include(i => i.PeerToPeerInterviewUsers)
                    .Include(i => i.PTPQuestionInterviews)
                    .Where(i => i.ScheduledDate == dto.ScheduledDate &&
                                i.ScheduledTime == dto.ScheduledTime &&
                                i.Category == dto.Category &&
                                i.QusestionType == dto.QuestionType)
                    .FirstOrDefaultAsync();

                if (existingInterview != null &&existingInterview.PeerToPeerInterviewUsers.Any(u => u.UserID == userId))
                {
                    return Response<PTPInterviewDTO>.Failure(new PTPInterviewDTO(), "User has already created an interview in this time slot.", 400);
                }

                PTPInterview interview;
                if (existingInterview != null)
                {
                     if (existingInterview.SlotState == TimeSlotState.TakenByTwo)
                    {
                        return Response<PTPInterviewDTO>.Failure(new PTPInterviewDTO(), "Time slot is fully booked", 400);
                    }
                    else if (existingInterview.SlotState == TimeSlotState.TakenByOne)
                    {
                        existingInterview.SlotState = TimeSlotState.TakenByTwo;
                        _context.PTPInterviews.Update(existingInterview);
                         await _context.SaveChangesAsync();
                    }
                    interview = existingInterview;
                }
                else
                {
                    interview = new PTPInterview
                    {
                        ScheduledDate = dto.ScheduledDate,
                        ScheduledTime = dto.ScheduledTime,
                        SlotState = TimeSlotState.TakenByOne,
                        Status = InterviewStatus.Scheduled,
                        Category=dto.Category,
                        QusestionType=dto.QuestionType,
                        PTPQuestionInterviews = new List<PTPQuestionInterview>(),
                        PeerToPeerInterviewUsers = new List<PTPUsers>()
                    };

                    _context.PTPInterviews.Add(interview);
                    await _context.SaveChangesAsync();
                }

                var ptpUser = new PTPUsers
                {
                    UserID = userId,
                    PTPIId = interview.Id,
                    PeerToPeerInterview = interview
                };
                _context.PTPUsers.Add(ptpUser);
                await _context.SaveChangesAsync();


                IEnumerable<int>? excludeQuestionIds = null;
                if (existingInterview != null && existingInterview.PTPQuestionInterviews.Any())
                {
                    excludeQuestionIds = existingInterview.PTPQuestionInterviews.Select(qi => qi.PTPQuestionId);
                }
                var questionResult = await GetRandomQuestionsAsync(dto.QuestionType, dto.Category, questionCount, excludeQuestionIds);
                if (!questionResult.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return Response<PTPInterviewDTO>.Failure(new PTPInterviewDTO(), questionResult.Message, questionResult.StatusCode);
                }
                var randomQuestions = questionResult.Data;

                var questionInterviews = new List<PTPQuestionInterview>();
                foreach (var qDto in randomQuestions)
                {
                    var qi = new PTPQuestionInterview
                    {
                        PTPInterviewId = interview.Id,
                        PTPQuestionId = qDto.Id,
                        PTPUserId = ptpUser.Id,

                    };
                    questionInterviews.Add(qi);
                }
                _context.PTPQuestionInterviews.AddRange(questionInterviews);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var interviewDto = _mapper.Map<PTPInterviewDTO>(interview);
                interviewDto.Questions = _mapper.Map<List<PTPQuestionDTO>>(randomQuestions);
                interviewDto.ScheduledTimeDisplay = interview.ScheduledTime.GetDisplayName();

                return Response<PTPInterviewDTO>.Success(interviewDto, $"Interview created with {questionCount} random question(s).", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<PTPInterviewDTO>.Failure(new PTPInterviewDTO(), $"Failed to create interview: {ex.Message}", 500);
            }
        }

        #endregion

        #region Get PTP Interview TimeSlots
        public async Task<Response<List<PTPInterviewTimeSlotDTO>>> GetPTPInterviewTimeSlotsAsync(InterviewCategory category, InterviewQuestionType questionType, string scheduledDate)
        {
            try
            {
                //string cacheKey = $"InterviewTimeSlots_{category}_{questionType}_{scheduledDate}";

                //var cachedData = _cacheService.GetData<List<PTPInterviewTimeSlotDTO>>(cacheKey);
                //if (cachedData != null)
                //{
                //    return Response<List<PTPInterviewTimeSlotDTO>>.Success(cachedData, "Time slots retrieved from cache.", 200);
                //}

                var timeSlotList = new List<PTPInterviewTimeSlotDTO>();


                foreach (InterviewTimeSlot slot in Enum.GetValues(typeof(InterviewTimeSlot)))
                {
                    var interview = await _context.PTPInterviews
                        .Where(i => i.ScheduledDate == scheduledDate &&
                                    i.ScheduledTime == slot &&
                                    i.Category == category &&
                                    i.QusestionType == questionType)
                        .FirstOrDefaultAsync();


                    var dto = new PTPInterviewTimeSlotDTO
                    {
                        TimeSlot = slot,
                        SlotState = interview != null ? interview.SlotState : TimeSlotState.Empty,
                        TimeSlotName = slot.GetDisplayName(),
                    };

                    timeSlotList.Add(dto);
                }


                //_cacheService.SetData(cacheKey, timeSlotList, TimeSpan.FromMinutes(5));

                return Response<List<PTPInterviewTimeSlotDTO>>.Success(timeSlotList, "Time slots retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                return Response<List<PTPInterviewTimeSlotDTO>>.Failure(new List<PTPInterviewTimeSlotDTO>(), $"An error occurred: {ex.Message}", 500);
            }
        }

        #endregion

        #region Cancel PTP Interview
        public async Task<Response<bool>> CancelPTPInterviewAsync(int interviewId, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var interview = await _context.PTPInterviews
                    .Include(i => i.PeerToPeerInterviewUsers)
                    .Include(i => i.PTPQuestionInterviews)
                    .FirstOrDefaultAsync(i => i.Id == interviewId);

                if (interview == null)
                {
                    return Response<bool>.Failure(false, "Interview not found.", 404);
                }

                var userAssociation = interview.PeerToPeerInterviewUsers.FirstOrDefault(u => u.UserID == userId);

                if (userAssociation == null)
                {
                    return Response<bool>.Failure(false, "Unauthorized: User is not associated with this interview.", 403);
                }

                //if (interview.Status != InterviewStatus.Scheduled)
                //{
                //    return Response<bool>.Failure(false, "Interview cannot be cancelled at this stage.", 400);
                //}

                var isRemainingUser = interview.PeerToPeerInterviewUsers.Any(u => u.UserID != userId);

                if (!isRemainingUser)
                {
                    _context.PTPInterviews.Remove(interview);
                }
                else
                {
                    interview.PeerToPeerInterviewUsers.Remove(userAssociation);
                    var userQuestionInterviews = interview.PTPQuestionInterviews
                        .Where(q => q.PTPUserId == userAssociation.Id)
                        .ToList();

                    if (userQuestionInterviews.Any())
                    {
                        _context.PTPQuestionInterviews.RemoveRange(userQuestionInterviews);
                    }

                    interview.SlotState = TimeSlotState.TakenByOne;
                    _context.PTPInterviews.Update(interview);

                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Response<bool>.Success(true, "Interview cancelled successfully.", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure(false, $"Failed to cancel interview: {ex.Message}", 500);
            }
        }

        #endregion

        #region Get User PTPInterview Questions
        public async Task<Response<List<PTPQuestionDTO>>> GetUserPTPInterviewQuestionsAsync(int interviewId, string userId)
        {
            try
            {
                var interview = await _context.PTPInterviews
                    .Include(i => i.PeerToPeerInterviewUsers)
                    .Include(i => i.PTPQuestionInterviews)
                        .ThenInclude(qi => qi.PTPQuestion)
                    .FirstOrDefaultAsync(i => i.Id == interviewId);

                if (interview == null)
                {
                    return Response<List<PTPQuestionDTO>>.Failure(new List<PTPQuestionDTO>(), "Interview not found.", 404);
                }

                var user = interview.PeerToPeerInterviewUsers
                    .FirstOrDefault(u => u.UserID == userId);

                if (user == null)
                {
                    return Response<List<PTPQuestionDTO>>.Failure(new List<PTPQuestionDTO>(), "Unauthorized: User is not associated with this interview.", 403);
                }

                var userQuestions = interview.PTPQuestionInterviews
                    .Where(qi => qi.PTPUserId == user.Id)
                    .Select(qi => qi.PTPQuestion)
                    .ToList();

                if (!userQuestions.Any())
                {
                    return Response<List<PTPQuestionDTO>>.Success(new List<PTPQuestionDTO>(), "No questions found for this user in the interview.", 200);
                }

                var questionDtos = _mapper.Map<List<PTPQuestionDTO>>(userQuestions);

                return Response<List<PTPQuestionDTO>>.Success(questionDtos, "User interview questions retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                return Response<List<PTPQuestionDTO>>.Failure(new List<PTPQuestionDTO>(), $"An error occurred while retrieving user questions: {ex.Message}", 500);
            }
        }

        #endregion

        #region Start PTP Interview Session
        public async Task<Response<bool>> StartPTPInterviewSessionAsync(int interviewId, string userId)
        {
            if (string.IsNullOrEmpty(userId) || interviewId == 0)
            {
                return Response<bool>.Failure(false, "Invalid user or interview id.", 400);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var interview = await _context.PTPInterviews.Include(ptp => ptp.PeerToPeerInterviewUsers)
                    .Where(ptp => ptp.Id == interviewId).FirstOrDefaultAsync();
                if (interview == null)
                {
                    return Response<bool>.Failure(false, "Interview not found.", 404);
                }

                var isParticipant = interview.PeerToPeerInterviewUsers.Any(ptp => ptp.UserID == userId);
                if (!isParticipant)
                {
                    return Response<bool>.Failure(false, "This user isn’t a participant in this interview.", 403);
                }

                if (!IsInTime(interview.ScheduledTime, interview.ScheduledDate!))
                {
                    return Response<bool>.Failure(false, "Interview start time has not come yet or it is too late.", 400);
                }
                if (interview.SlotState != TimeSlotState.TakenByTwo)
                {
                    return Response<bool>.Failure(false, "Interview doesn’t have another peer, try to schedule another one with a partner.", 400);
                }

                interview.Status = InterviewStatus.InProgress;
                await _unitOfWork.PTPInterviews.UpdateAsync(interview);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return Response<bool>.Success(true, "Interview session strated.", 200);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure(false, $"Database update error: {ex.Message}", 500);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure(false, $"Server error: {ex.Message}", 500);
            }
        } 
        #endregion

        #region End PTP Interview Session
        public async Task<Response<bool>> EndPTPInterviewSessionAsync(int interviewId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var interview = await _context.PTPInterviews.Include(ptp => ptp.PeerToPeerInterviewUsers)
                    .Where(ptp => ptp.Id == interviewId).FirstOrDefaultAsync();
                if (interview == null)
                {
                    return Response<bool>.Failure(false, "Interview not found.", 404);
                }
                if(interview.Status!= InterviewStatus.InProgress)
                {
                    return Response<bool>.Failure(false, "Interview didn’t begin to be ended.", 400);
                }
                interview.Status = InterviewStatus.Completed;
                await _unitOfWork.PTPInterviews.UpdateAsync(interview);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return Response<bool>.Success(true, "Interview session ended.", 200);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure(false, $"Database update error: {ex.Message}", 500);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<bool>.Failure(false, $"Server error: {ex.Message}", 500);
            }
        }
        #endregion

        #region Get User Current PTP Interview Session
        public async Task<Response<PTPInterviewDTO>> GetUserCurrentPTPInterviewSessionAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Response<PTPInterviewDTO>.Failure(new PTPInterviewDTO(), "Invalid user id.", 400);
            }
            try
            {
                var interview = await _context.PTPInterviews.Include(i => i.PeerToPeerInterviewUsers)
                    .FirstOrDefaultAsync(i => i.Status == InterviewStatus.InProgress
                        && i.PeerToPeerInterviewUsers.Any(u => u.UserID == userId));

                return interview == null
                    ? Response<PTPInterviewDTO>.Success(new PTPInterviewDTO(), "User has no running interviews.", 204)
                    : Response<PTPInterviewDTO>.Success(_mapper.Map<PTPInterviewDTO>(interview), "Interview found.", 200);
            }
            catch (Exception ex)
            {
                return Response<PTPInterviewDTO>.Failure(new PTPInterviewDTO(), $"Server error: {ex.Message}.", 500);
            }
        } 
        #endregion


        #region Helpers

        private string FormatTimeRemaining(TimeSpan timeRemaining)
        {
            if (timeRemaining.TotalSeconds <= 0)
                return "Interview has started or passed";

            if (timeRemaining.TotalMinutes < 1)
                return "Starting now!";

            return $"{timeRemaining.Days} d, {timeRemaining.Hours} h, {timeRemaining.Minutes} min";
        }


        private TimeSpan GetTimeSpanFromEnum(InterviewTimeSlot timeSlot)
        {
            return timeSlot switch
            {
                InterviewTimeSlot.EightAM => new TimeSpan(8, 0, 0),
                InterviewTimeSlot.TenAM => new TimeSpan(10, 0, 0),
                InterviewTimeSlot.TwelvePM => new TimeSpan(12, 0, 0),
                InterviewTimeSlot.TwoPM => new TimeSpan(14, 0, 0),
                InterviewTimeSlot.SixPM => new TimeSpan(18, 0, 0),
                InterviewTimeSlot.TenPM => new TimeSpan(22, 0, 0),
                _ => TimeSpan.Zero
            };
        }

        private bool IsInTime(InterviewTimeSlot timeSlot,string date)
        {
            var scheduledTime = GetTimeSpanFromEnum(timeSlot);
            DateTime scheduledDate = DateTime.Parse(date);
            var interviewStartTime = scheduledDate.Date + scheduledTime;

            return interviewStartTime >= DateTime.UtcNow && interviewStartTime <= DateTime.UtcNow.AddHours(1);
        }

        private async Task<List<PTPUpcomingQuestionDTO>> GetUserQuestionsForInterview(int interviewId, string userId)
        {
            var ptpUser = await _unitOfWork.PTPUsers
                .GetByExpressionAsync(u => u.UserID == userId && u.PTPIId == interviewId);

            if (ptpUser == null)
            {
                return new List<PTPUpcomingQuestionDTO>(); 
            }

            var userQuestions = await _unitOfWork.PTPQuestionInterviews
                .GetAllByExpressionAsync(qi => qi.PTPUserId == ptpUser.Id && qi.PTPInterviewId == interviewId,
                                         include: q => q.Include(qi => qi.PTPQuestion));

            return userQuestions.Select(qi => new PTPUpcomingQuestionDTO
            {
                Id = qi.PTPQuestion.Id,
                Title = qi.PTPQuestion.Title ?? string.Empty
            }).ToList();
        }


        private async Task<Response<List<PTPQuestionDTO>>> GetRandomQuestionsAsync(InterviewQuestionType questionType, InterviewCategory category, int questionCount,IEnumerable<int>? excludeQuestionIds = null)
        {
            try
            {
                if (!Enum.IsDefined(typeof(InterviewCategory), category) ||
                !Enum.IsDefined(typeof(InterviewQuestionType), questionType))
                {
                    return Response<List<PTPQuestionDTO>>.Failure(new List<PTPQuestionDTO>(),"Invalid category or question type",400);
                }

                if (questionCount <= 0)
                {
                    return Response<List<PTPQuestionDTO>>.Failure(new List<PTPQuestionDTO>(),"Question count must be a positive integer.",400);
                }

                var query = _context.PTPQuestions
                   .Where(q => q.QusestionType == questionType && q.Category == category);
                if (excludeQuestionIds != null && excludeQuestionIds.Any())
                {
                    query = query.Where(q => !excludeQuestionIds.Contains(q.Id));
                }

                var questionIds = await query.Select(q => q.Id).ToListAsync();

                if (questionIds.Count < questionCount)
                {
                    return Response<List<PTPQuestionDTO>>.Failure(new List<PTPQuestionDTO>(),$"Not enough questions. Requested: {questionCount}, Available: {questionIds.Count}",404);
                }
                var rng = new Random();
                var selectedIds = questionIds.OrderBy(id => rng.Next()).Take(questionCount).ToList();

                var questions = await _context.PTPQuestions
                    .Where(q => selectedIds.Contains(q.Id))
                    .ToListAsync();

                if (questions == null)
                {
                    return Response<List<PTPQuestionDTO>>.Failure(new List<PTPQuestionDTO>(), "Error retrieving questions", 500);
                }

                var questionDtos = _mapper.Map<List<PTPQuestionDTO>>(questions);
                return Response<List<PTPQuestionDTO>>.Success(questionDtos, "Random question retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                return Response<List<PTPQuestionDTO>>.Failure(new List<PTPQuestionDTO>(), $"An error occurred while retrieving the question: {ex.Message}", 500);
            }
        }



        #endregion


    }
}

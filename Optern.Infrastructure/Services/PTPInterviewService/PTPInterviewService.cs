using Optern.Application.DTOs.Question;
using Optern.Domain.Extensions;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Reflection.Emit;

namespace Optern.Infrastructure.Services.PTPInterviewService
{
    public class PTPInterviewService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper) : IPTPInterviewService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;

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
                                                  i.Status == InterviewStatus.Scheduled);
                    
                var filteredInterviews = upcomingInterviews
                    .Where(i => DateTime.TryParse(i.ScheduledDate, out DateTime scheduledDate) &&
                                scheduledDate >= currentTime).OrderBy(i=>i.ScheduledTime)
                    .ToList();

                if (!filteredInterviews.Any())
                {
                    return Response<IEnumerable<UpcomingPTPInterviewDTO>>.Failure(new List<UpcomingPTPInterviewDTO>(), "No Upcoming Interviews found", 404);
                }

                var upcomingInterviewsDTO = _mapper.Map<List<UpcomingPTPInterviewDTO>>(filteredInterviews);

                foreach (var interviewDTO in upcomingInterviewsDTO)
                {
                    var interviewEntity = filteredInterviews.FirstOrDefault(i => i.Id == interviewDTO.Id);

                    interviewDTO.ScheduledTime = interviewEntity.ScheduledTime.GetDisplayName();

                    if (!DateTime.TryParse(interviewEntity.ScheduledDate, out DateTime scheduledDate))
                    {
                        interviewDTO.TimeRemaining = "Invalid date format";
                        continue;
                    }

                    DateTime scheduledDateTime = scheduledDate.Add(GetTimeSpanFromEnum(interviewEntity.ScheduledTime));

                    TimeSpan timeRemaining = scheduledDateTime - DateTime.UtcNow;

                    interviewDTO.TimeRemaining = FormatTimeRemaining(timeRemaining);

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

                var questionResult = await GetRandomQuestionsAsync(dto.QuestionType, dto.Category, questionCount);
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


        private async Task<Response<List<PTPQuestionDTO>>> GetRandomQuestionsAsync(InterviewQuestionType questionType, InterviewCategory category, int questionCount)
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

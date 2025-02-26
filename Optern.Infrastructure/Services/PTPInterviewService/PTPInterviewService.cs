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
                                                  i.ScheduledDate >= currentTime.Date &&
                                                  i.Status == InterviewStatus.Scheduled);

                if (upcomingInterviews == null || !upcomingInterviews.Any())
                {
                    return Response<IEnumerable<UpcomingPTPInterviewDTO>>.Failure(new List<UpcomingPTPInterviewDTO>(), "No Upcoming Interviews found", 404);
                }

                var upcomingInterviewsDTO = _mapper.Map<List<UpcomingPTPInterviewDTO>>(upcomingInterviews);

                foreach (var interviewDTO in upcomingInterviewsDTO)
                {
                    var interviewEntity = upcomingInterviews.First(i => i.ScheduledDate == interviewDTO.ScheduledDate);
                                                                        

                    TimeSpan interviewTime = interviewEntity.ScheduledTime;
                    interviewDTO.ScheduledTime = $"{interviewTime.Hours:D2}:{interviewTime.Minutes:D2}:{interviewTime.Seconds:D2}";
                    DateTime interviewDateTime = interviewEntity.ScheduledDate.Add(interviewEntity.ScheduledTime);
                    TimeSpan timeRemaining = interviewDateTime - currentTime;

                    interviewDTO.TimeRemaining = FormatTimeRemaining(timeRemaining);

                    //var questions = await _unitOfWork.PTPQuestionInterviews
                    //    .GetAllByExpressionAsync(qi => qi.PTPInterviewId == interviewEntity.Id);

                    //var questionDTOs = questions
                    //    .Select(qi => new PTPQuestionDTO
                    //    {
                    //        Id = qi.PTPQuestionId,
                    //        Content = qi.PTPQuestion?.Content, 
                    //    })
                    //    .ToList();

                    //interviewDTO.Questions = questionDTOs;

                }

                return Response<IEnumerable<UpcomingPTPInterviewDTO>>.Success(upcomingInterviewsDTO, "Upcoming interviews retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<UpcomingPTPInterviewDTO>>.Failure(new List<UpcomingPTPInterviewDTO>(), ex.Message, 500);
            }
        }
        #endregion

        #region Helpers

        private string FormatTimeRemaining(TimeSpan timeSpan)
        {
            if (timeSpan.TotalSeconds <= 0)
                return "Interview has already started";

            List<string> parts = new List<string>();

            if (timeSpan.Days > 0)
                parts.Add($"{timeSpan.Days} d");

            if (timeSpan.Hours > 0)
                parts.Add($"{timeSpan.Hours} h");

            if (timeSpan.Minutes > 0)
                parts.Add($"{timeSpan.Minutes} m");

            if (timeSpan.Seconds > 0)
                parts.Add($"{timeSpan.Seconds} sec");

            return string.Join(", ", parts);
        }
        #endregion


    }
}

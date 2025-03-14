using Optern.Application.DTOs.Question;
using Optern.Application.DTOs.VInterview;
using Optern.Application.Interfaces.IVInterviewService;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.Services.VInterviewService
{
    public class VInterviewService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper) : IVInterviewService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        #region Create virtual Interview
        public async Task<Response<VInterviewDTO>> CreateVInterviewAsync(CreateVInterviewDTO dto, int questionCount, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!Enum.IsDefined(typeof(InterviewCategory), dto.Category) ||
                    !Enum.IsDefined(typeof(InterviewQuestionType), dto.QuestionType))
                {
                    return Response<VInterviewDTO>.Failure(new VInterviewDTO(), "Invalid category or question type", 400);
                }

                VInterview interview = new VInterview
                {
                    Category = dto.Category,
                    QusestionType = dto.QuestionType,
                    UserId = userId, 
                    VQuestionInterviews = new List<VQuestionInterview>(),
                    InterviewDate= DateTime.UtcNow,
                    SpeechAnalysisResult = "" 
                };

                _context.VInterview.Add(interview);
                await _context.SaveChangesAsync();

                var questionResult = await GetRandomQuestionsAsync(dto.QuestionType, dto.Category, questionCount);
                if (!questionResult.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return Response<VInterviewDTO>.Failure(new VInterviewDTO(), questionResult.Message, questionResult.StatusCode);
                }

                var randomQuestions = questionResult.Data;
                var questionInterviews = randomQuestions.Select(qDto => new VQuestionInterview
                {
                    VInterviewId = interview.Id,
                    PTPQuestionId = qDto.Id,
                    UserId= userId,
                }).ToList();

                _context.VQuestionInterviews.AddRange(questionInterviews);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                
                var interviewDto = _mapper.Map<VInterviewDTO>(interview);
                interviewDto.Questions = _mapper.Map<List<PTPQuestionDTO>>(randomQuestions);
                interviewDto.UserId = userId; 

                return Response<VInterviewDTO>.Success(interviewDto, $"Interview created with {questionCount} random question(s).", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<VInterviewDTO>.Failure(new VInterviewDTO(), $"Failed to create interview: {ex.Message}", 500);
            }
        }

        #endregion


        #region Helpers
        private async Task<Response<List<PTPQuestionDTO>>> GetRandomQuestionsAsync(InterviewQuestionType questionType, InterviewCategory category, int questionCount)
        {
            try
            {
                if (!Enum.IsDefined(typeof(InterviewCategory), category) ||
                !Enum.IsDefined(typeof(InterviewQuestionType), questionType))
                {
                    return Response<List<PTPQuestionDTO>>.Failure(new List<PTPQuestionDTO>(), "Invalid category or question type", 400);
                }

                if (questionCount <= 0)
                {
                    return Response<List<PTPQuestionDTO>>.Failure(new List<PTPQuestionDTO>(), "Question count must be a positive integer.", 400);
                }

                var query = _context.PTPQuestions
                   .Where(q => q.QusestionType == questionType && q.Category == category);

                var questionIds = await query.Select(q => q.Id).ToListAsync();

                if (questionIds.Count < questionCount)
                {
                    return Response<List<PTPQuestionDTO>>.Failure(new List<PTPQuestionDTO>(), $"Not enough questions. Requested: {questionCount}, Available: {questionIds.Count}", 404);
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

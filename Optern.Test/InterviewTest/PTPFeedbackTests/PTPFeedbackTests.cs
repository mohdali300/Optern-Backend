using NUnit.Framework;
using Moq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.UnitOfWork;
using Optern.Application.Interfaces.IPTPInterviewService;
using Optern.Infrastructure.Services.PTPInterviewService;
using Optern.Infrastructure.ExternalInterfaces.ICacheService;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Optern.Application.Interfaces.IPTPFeedbackService;
using Optern.Infrastructure.Services.PTPFeedbackService;
using Optern.Application.DTOs.PTPInterview;
using Optern.Application.DTOs.Question;
using Optern.Domain.Extensions;
using System.Linq.Expressions;
using Optern.Application.DTOs.PTPFeedback;
using Optern.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Task = System.Threading.Tasks.Task;
using System.Reflection;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Optern.Test.InterviewTest.PTPFeedbackTests
{
    [TestFixture]
    [Category("PTPFeedbackTests")]
    public class PTPFeedbackTests
    {
        private OpternDbContext _context;
        private IMapper _mapper;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private IPTPFeedbackService _pTPFeedbackService;
        private List<PTPInterview> _samplePTPInterviews = new List<PTPInterview>();
        private List<PTPFeedBack> _sampleFeedbacks = new List<PTPFeedBack>();
        private List<PTPUsers> _samplePTPUsers = new List<PTPUsers>();
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<OpternDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new OpternDbContext(options);
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mapper = MappingProfiles().CreateMapper();

            CreateSampleData();
            UOWSetupFeedback();

            _pTPFeedbackService = new PTPFeedbackService(_mockUnitOfWork.Object, _context, _mapper);
        }
        
        [TearDown]
        public void TearDown()
            {
                _context.Database.EnsureDeleted();
                _context.Dispose();
            }

        #region Add PTP FeedBack Tests
        [Test]
        [Category("AddPTPFeedbackTests")]
        public async Task AddPTPFeedbackAsync_WithEmptyOrInvalidFields_ReturnsBadRequest()
        {
            // Arrange: 
            var invalidDto = new AddPTPFeedbackDTO  //Missing required fields.
            {
                UserId = "",  // empty or null
                PTPInterviewId = 0,  // invalid id
                InterviewerPerformance = "",
                Improvement = ""
            };

            // Act
            var result = await _pTPFeedbackService.AddPTPFeedback(invalidDto);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("The Fields Can't Be Empty"));
            });
        }

        [Test]
        [Category("AddPTPFeedbackTests")]
        public async Task AddPTPFeedbackAsync_WithNonExistentInterview_ReturnsNotFound()
        {
            // Arrange: 
            var dto = new AddPTPFeedbackDTO
            {
                UserId = "user1",
                PTPInterviewId = 999,  // non-existent
                InterviewerPerformance = "Good",
                Improvement = "None"
            };

            // Act
            var result = await _pTPFeedbackService.AddPTPFeedback(dto);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("Interview Not Found"));
            });
        }

        [Test]
        [Category("AddPTPFeedbackTests")]
        public async Task AddPTPFeedbackAsync_WithUserNotInInterview_ReturnsNotFound()
        {
            // Arrange:
            var dto = new AddPTPFeedbackDTO
            {
                UserId = "user_not_in_interview",
                PTPInterviewId = 1,
                InterviewerPerformance = "Average",
                Improvement = "Improve communication"
            };

            // Act
            var result = await _pTPFeedbackService.AddPTPFeedback(dto);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("User not found in That Interview"));
            });
        }
        [Test]
        [Category("AddPTPFeedbackTests")]
        public async Task AddPTPFeedbackAsync_WithNoInterviewPartner_ReturnsNotFound()
        {

            var dto = new AddPTPFeedbackDTO
            {
                UserId = "user1", // one participant only.
                PTPInterviewId = 1,
                InterviewerPerformance = "Excellent",
                Improvement = "None"
            };

            // Act
            var result = await _pTPFeedbackService.AddPTPFeedback(dto);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("User not found in That Interview"));
            });
        }
        [Test]
        [Category("AddPTPFeedbackTests")]
        public async Task AddPTPFeedbackAsync_WhenFeedbackAlreadyExists_ReturnsBadRequest()  //DuplicateFeedback
        {
            // Arrange: 

            var dto = new AddPTPFeedbackDTO //feedback already exists for user1 on interview 2
            {
                UserId = "user1",
                PTPInterviewId = 2,
                InterviewerPerformance = "Excellent",
                Improvement = "None"
            };

            // Act
            var result = await _pTPFeedbackService.AddPTPFeedback(dto);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("You Already Gave Your Feedback"));
            });
        }
        [Test]
        [Category("AddPTPFeedbackTests")]
        public async Task AddPTPFeedbackAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange: 
            var dto = new AddPTPFeedbackDTO
            {
                UserId = "user1",
                PTPInterviewId = 3,
                InterviewerPerformance = "Good",
                Improvement = "Work on technical skills"
            };

            // Act
            var result = await _pTPFeedbackService.AddPTPFeedback(dto);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Does.Contain("Feedback added successfully"));
            });
        }


        #endregion
        #region Get PTPFeedback Tests
        [Test]
        [Category("GetPTPFeedbackTests")]
        public async Task GetPTPFeedbackAsync_WithInvalidParameters_ReturnsBadRequest()
        {
            // Arrange: 
            int invalidInterviewId = 0;
            string emptyUserId = ""; //or null

            // Act
            var result = await _pTPFeedbackService.GetPTPFeedback(invalidInterviewId, emptyUserId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False, "Expected failure when parameters are invalid.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("Invalid parameters"));
            });
        }

        [Test]
        [Category("GetPTPFeedbackTests")]
        public async Task GetPTPFeedbackAsync_WithNonExistentInterview_ReturnsNotFound()
        {
            // Arrange:=
            int nonExistentInterviewId = 999;
            string userId = "user1";

            // Act
            var result = await _pTPFeedbackService.GetPTPFeedback(nonExistentInterviewId, userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False, "Expected failure when interview does not exist.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("Interview not found"));
            });
        }

        [Test]
        [Category("GetPTPFeedbackTests")]
        public async Task GetPTPFeedbackAsync_WithUserNotInInterview_ReturnsNotFound()
        {
            // Arrange: 
            int interviewId = 2;
            string nonAssociatedUserId = "user_not_exists";

            // Act
            var result = await _pTPFeedbackService.GetPTPFeedback(interviewId, nonAssociatedUserId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False, "Expected failure when user is not part of the interview.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("User not found in this interview"));
            });
        }

        [Test]
        [Category("GetPTPFeedbackTests")]
        public async Task GetPTPFeedbackAsync_WithNoFeedbackFound_ReturnsNotFound()
        {
            // Arrange: 
            int interviewId = 1;
            string userId = "user1";
            
            // Act
            var result = await _pTPFeedbackService.GetPTPFeedback(interviewId, userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False, "Expected failure when no feedback exists for the given interview and user.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("No feedback found for this interview and user"));
            });
        }

        [Test]
        [Category("GetPTPFeedbackTests")]
        public async Task GetPTPFeedbackAsync_WithValidInput_ReturnsFeedbackSuccessfully()
        {
            // Arrange: Use valid interview id 101 and user1 who gave feedback.
            int interviewId = 2;
            string userId = "user1";

            // Act
            var result = await _pTPFeedbackService.GetPTPFeedback(interviewId, userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True, "Expected success when valid feedback exists.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null, "Expected a non-null feedback DTO.");
                Assert.That(result.Data.InterviewerPerformance, Is.EqualTo("Good"));
                Assert.That(result.Message, Does.Contain("Feedback retrieved successfully"));
            });
        }

        

        #endregion



        #region Helpers
        private async void CreateSampleData()
        {
            _samplePTPInterviews = new List<PTPInterview>
            {
                new PTPInterview
                {
                    Id = 1,
                    ScheduledDate = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"),
                    ScheduledTime = InterviewTimeSlot.TenPM,
                    Category = InterviewCategory.SQL,
                    QusestionType = InterviewQuestionType.Beginner,
                    SlotState = TimeSlotState.TakenByOne,
                    Status = InterviewStatus.Completed,
                    PeerToPeerInterviewUsers = new List<PTPUsers>(),
                    PTPQuestionInterviews=new List<PTPQuestionInterview>()
                    
                },
                new PTPInterview
                {
                    Id = 2,
                    ScheduledDate = DateTime.UtcNow.AddDays(-2).ToString("yyyy-MM-dd"),
                    ScheduledTime = InterviewTimeSlot.TenPM,
                    Category = InterviewCategory.SQL,
                    QusestionType = InterviewQuestionType.Beginner,
                    SlotState = TimeSlotState.TakenByTwo,
                    Status = InterviewStatus.Completed,
                    PeerToPeerInterviewUsers = new List<PTPUsers>(),
                    PTPQuestionInterviews=new List<PTPQuestionInterview>()

                },
                new PTPInterview
                {
                    Id = 3,
                    ScheduledDate = DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-dd"),
                    ScheduledTime = InterviewTimeSlot.TenPM,
                    Category = InterviewCategory.SQL,
                    QusestionType = InterviewQuestionType.Beginner,
                    SlotState = TimeSlotState.TakenByTwo,
                    Status = InterviewStatus.Completed,
                    PeerToPeerInterviewUsers = new List<PTPUsers>(),
                    PTPQuestionInterviews=new List<PTPQuestionInterview>()

                }
            };
            _sampleFeedbacks = new List<PTPFeedBack>
            {
                new PTPFeedBack
                {
                    Id = 1,
                    PTPInterviewId = 2,
                    GivenByUserId = 2, 
                    ReceivedByUserId = 3,
                    InterviewerPerformance = "Good",
                    Improvement = "More practice"
                },
                new PTPFeedBack
                {
                    Id = 2,
                    PTPInterviewId = 2,
                    GivenByUserId = 3,
                    ReceivedByUserId = 2,
                    InterviewerPerformance = "Good",
                    Improvement = "More practice"
                }

            };
            _samplePTPUsers = new List<PTPUsers>
            {
                new PTPUsers
                {
                    Id=1,
                    PTPIId=1,
                    UserID="user1"
                },
                new PTPUsers
                {
                    Id=2,
                    PTPIId=2,
                    UserID="user1"
                },
                new PTPUsers
                {
                    Id=3,
                    PTPIId=2,
                    UserID="user2"
                },
                new PTPUsers
                {
                    Id=4,
                    PTPIId=3,
                    UserID="user1"
                },new PTPUsers
                {
                    Id=5,
                    PTPIId=3,
                    UserID="user2"
                },
            };
            _context.PTPInterviews.AddRange(_samplePTPInterviews);
            _context.PTPUsers.AddRange(_samplePTPUsers);
            _context.PTPFeedBacks.AddRange(_sampleFeedbacks);
            
            _context.SaveChanges();
        }
        

        private void UOWSetupFeedback()
        {
            _mockUnitOfWork.Setup(uow => uow.PTPFeedBack.AddAsync(It.IsAny<PTPFeedBack>()))
                .ReturnsAsync((PTPFeedBack feedback) =>
                {
                    feedback.Id = 1;
                    _sampleFeedbacks.Add(feedback);
                    return feedback;
                });


            _mockUnitOfWork.Setup(uow => uow.PTPUsers.GetByExpressionAsync(It.IsAny<Expression<Func<PTPUsers, bool>>>()))
                .ReturnsAsync((Expression<Func<PTPUsers, bool>> predicate) =>
                {
                    return _samplePTPUsers.FirstOrDefault(predicate.Compile());
                });

            _mockUnitOfWork.Setup(uow => uow.PTPFeedBack.GetByExpressionAsync(It.IsAny<Expression<Func<PTPFeedBack, bool>>>()))
                .ReturnsAsync((Expression<Func<PTPFeedBack, bool>> predicate) =>
                {
                    return _sampleFeedbacks.FirstOrDefault(predicate.Compile());
                });

            _mockUnitOfWork.Setup(uow => uow.PTPFeedBack.AnyAsync(It.IsAny<Expression<Func<PTPFeedBack, bool>>>()))
                .ReturnsAsync((Expression<Func<PTPFeedBack, bool>> predicate) =>
                {
                    return _sampleFeedbacks.Any(predicate.Compile());
                });

            _mockUnitOfWork.Setup(uow => uow.PTPInterviews.AnyAsync(It.IsAny<Expression<Func<PTPInterview, bool>>>()))
                .ReturnsAsync((Expression<Func<PTPInterview, bool>> predicate) =>
                {
                    return _samplePTPInterviews.Any(predicate.Compile());
                });
        }

        private MapperConfiguration MappingProfiles()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AddPTPFeedbackDTO, PTPFeedBack>()
                    .ForMember(dest => dest.Ratings, opt => opt.MapFrom(src => src.Ratings ?? new List<KeyValuePair<int, string>>()));

                cfg.CreateMap<PTPFeedBack, PTPFeedbackDTO>();

            });
            return mapperConfig;
        }
        #endregion

    }
}

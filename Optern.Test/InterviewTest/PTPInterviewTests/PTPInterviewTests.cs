using NUnit.Framework;
using Moq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Optern.Domain.Entities;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.UnitOfWork;
using Optern.Application.DTOs.PTPInterview;
using Optern.Application.DTOs.Question;
using Optern.Application.Interfaces.IPTPInterviewService;
using Optern.Domain.Extensions;
using Optern.Infrastructure.Services.PTPInterviewService;
using Optern.Application.DTOs.Post;
using Optern.Infrastructure.ExternalInterfaces.ICacheService;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore.Diagnostics;


namespace Optern.Test.InterviewTest.PTPInterviewTests
{
    [TestFixture]
    [Category("PTPInterviewTests")]
    public class PTPInterviewTests
    {
        private OpternDbContext _context;
        private IMapper _mapper;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICacheService> _mockCacheService = null!;
        private IPTPInterviewService _ptpInterviewService;
        private List<PTPQuestions> _samplePTPQuestions = null!;
        private static int _interviewIdCounter = 100; 
        private static int _ptpUserIdCounter = 1;      
        private List<PTPInterview> _samplePTPInterviews = new List<PTPInterview>();

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<OpternDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new OpternDbContext(options);
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCacheService = new Mock<ICacheService>();
            _mapper = MappingProfiles().CreateMapper();

            CreateSampleData();
            UOWSetupPTPInterview();

            _ptpInterviewService = new PTPInterviewService(_mockUnitOfWork.Object, _context, _mapper, _mockCacheService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region Create PTP Interview Tests

        [Test]
        [Category("CreatePTPInterviewTests")]
        public async Task CreatePTPInterviewAsync_WithInvalidEnumValues_ReturnsBadRequest()
        {
            // Arrange:
            var invalidDto = new CreatePTPInterviewDTO
            {
                ScheduledDate = "2025-03-21",
                Category = (InterviewCategory)999,         // Invalid
                QuestionType = (InterviewQuestionType)999,   // Invalid
                ScheduledTime = (InterviewTimeSlot)999         // Invalid
            };

            // Act:
            var result = await _ptpInterviewService.CreatePTPInterviewAsync(invalidDto, 1, "user1");

            // Assert:
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("Invalid"));
            });
        }

        [Test]
        [Category("CreatePTPInterviewTests")]
        public async Task CreatePTPInterviewAsync_WhenUserAlreadyCreatedInterviewInSlot_ReturnsFailure()
        {
            // Arrange:
            var dto = new CreatePTPInterviewDTO
            {
                ScheduledDate = "2025-03-21",
                Category = InterviewCategory.SQL,
                QuestionType = InterviewQuestionType.Beginner,
                ScheduledTime = InterviewTimeSlot.TenPM
            };

            var existingInterview = new PTPInterview
            {
                ScheduledDate = dto.ScheduledDate,
                ScheduledTime = dto.ScheduledTime,
                Category = dto.Category,
                QusestionType = dto.QuestionType,
                SlotState = TimeSlotState.TakenByOne,
                Status = InterviewStatus.Scheduled,
                PTPQuestionInterviews = new List<PTPQuestionInterview>(),
                PeerToPeerInterviewUsers = new List<PTPUsers>
                {
                    new PTPUsers { UserID = "user1" }
                }
            };
            _context.PTPInterviews.Add(existingInterview);
            await _context.SaveChangesAsync();

            // Act:
            var result = await _ptpInterviewService.CreatePTPInterviewAsync(dto, 1, "user1");

            // Assert:
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("already created"));
            });
        }

        [Test]
        [Category("CreatePTPInterviewTests")]
        public async Task CreatePTPInterviewAsync_WhenTimeSlotFullyBooked_ReturnsFailure()
        {
            // Arrange:
            var dto = new CreatePTPInterviewDTO
            {
                ScheduledDate = "2025-03-21",
                Category = InterviewCategory.SQL,
                QuestionType = InterviewQuestionType.Beginner,
                ScheduledTime = InterviewTimeSlot.TenPM
            };

            var fullyBookedInterview = new PTPInterview
            {
                ScheduledDate = dto.ScheduledDate,
                ScheduledTime = dto.ScheduledTime,
                Category = dto.Category,
                QusestionType = dto.QuestionType,
                SlotState = TimeSlotState.TakenByTwo,
                Status = InterviewStatus.Scheduled,
                PTPQuestionInterviews = new List<PTPQuestionInterview>(),
                PeerToPeerInterviewUsers = new List<PTPUsers>()
            };
            _context.PTPInterviews.Add(fullyBookedInterview);
            await _context.SaveChangesAsync();

            // Act:
            var result = await _ptpInterviewService.CreatePTPInterviewAsync(dto, 1, "user2");

            // Assert:
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("fully booked"));
            });
        }

        [Test]
        [Category("CreatePTPInterviewTests")]
        public async Task CreatePTPInterviewAsync_WhenNoExistingInterview_CreatesNewInterviewSuccessfully() //verify that a new interview is created
        {
            // Arrange:
            var dto = new CreatePTPInterviewDTO
            {
                ScheduledDate = "2025-03-21",
                Category = InterviewCategory.SQL,
                QuestionType = InterviewQuestionType.Intermediate,
                ScheduledTime = InterviewTimeSlot.TenPM
            };

            // Act:
            var result = await _ptpInterviewService.CreatePTPInterviewAsync(dto, 1, "user3");

            // Assert:
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.ScheduledDate, Is.EqualTo(dto.ScheduledDate));
                Assert.That(result.Data.ScheduledTimeDisplay, Is.EqualTo(dto.ScheduledTime.GetDisplayName()));
                Assert.That(result.Data.Questions.Count, Is.EqualTo(1));
            });
        }

        [Test]
        [Category("CreatePTPInterviewTests")]
        public async Task CreatePTPInterviewAsync_WhenInterviewExistsWithOneUserJoined_UpdatesSlotStateAndAppendsNewQuestions()
        {

            // Arrange: Create a valid DTO.
            var dto = new CreatePTPInterviewDTO
            {
                ScheduledDate = "2025-03-21",
                Category = InterviewCategory.SQL,
                QuestionType = InterviewQuestionType.Intermediate,
                ScheduledTime = InterviewTimeSlot.TenPM
            };

            var existingInterview = new PTPInterview
            {
                ScheduledDate = dto.ScheduledDate,
                ScheduledTime = dto.ScheduledTime,
                Category = dto.Category,
                QusestionType = dto.QuestionType,
                SlotState = TimeSlotState.TakenByOne,
                Status = InterviewStatus.Scheduled,
                PTPQuestionInterviews = new List<PTPQuestionInterview>
                {
                    new PTPQuestionInterview { PTPQuestionId = 101 } // Assume question with Id 101 is pre-assigned
                },
                PeerToPeerInterviewUsers = new List<PTPUsers>
                {
                    new PTPUsers { UserID = "user1", Id = 1 }
                }
            };
            _context.PTPInterviews.Add(existingInterview);
            await _context.SaveChangesAsync();

            var result = await _ptpInterviewService.CreatePTPInterviewAsync(dto, 2, "user4");

            // Assert:
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.SlotState, Is.EqualTo(TimeSlotState.TakenByTwo));
                Assert.That(result.Data.Questions.Count, Is.EqualTo(2));
            });
        }

        [Test]
        public async Task CreatePTPInterviewAsync_WhenRandomQuestionsRetrievalFails_RollsBackAndReturnsFailure()
        {
            // Arrange: Create a valid DTO.
            var dto = new CreatePTPInterviewDTO
            {
                ScheduledDate = "2025-03-21",
                Category = InterviewCategory.SQL,
                QuestionType = InterviewQuestionType.Intermediate,
                ScheduledTime = InterviewTimeSlot.TenPM
            };

            var allQuestions = _context.PTPQuestions.ToList();
            _context.PTPQuestions.RemoveRange(allQuestions);
            await _context.SaveChangesAsync();

            // Act:
            var result = await _ptpInterviewService.CreatePTPInterviewAsync(dto, 3, "user5");

            // Assert:
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound)
                    .Or.EqualTo(StatusCodes.Status500InternalServerError));
                Assert.That(result.Message, Does.Contain("Not enough questions").Or.Contains("Failed to retrieve"));
            });
        }

        #endregion
        #region Helpers
        private void CreateSampleData()
        {
            _samplePTPQuestions = new List<PTPQuestions>
            {
                new PTPQuestions { Id = 101, Title = "Question 1", Content = "Content 1", Hints = "Hint 1", Answer = "Answer 1", Category = InterviewCategory.SQL, QusestionType = InterviewQuestionType.Intermediate },
                new PTPQuestions { Id = 102, Title = "Question 2", Content = "Content 2", Hints = "Hint 2", Answer = "Answer 2", Category = InterviewCategory.SQL, QusestionType = InterviewQuestionType.Intermediate },
                new PTPQuestions { Id = 103, Title = "Question 3", Content = "Content 3", Hints = "Hint 3", Answer = "Answer 3", Category = InterviewCategory.SQL, QusestionType = InterviewQuestionType.Intermediate },
            };

            //add to the in memory database
            _context.PTPQuestions.AddRange(_samplePTPQuestions);
            _context.SaveChanges();
        }

        private void UOWSetupPTPInterview()
        {
            _mockUnitOfWork.Setup(uow => uow.PTPInterviews.AddAsync(It.IsAny<PTPInterview>()))
                .ReturnsAsync((PTPInterview interview) =>
                {
                    interview.Id = _interviewIdCounter++;
                    _samplePTPInterviews.Add(interview);
                    return interview;
                });

            _mockUnitOfWork.Setup(uow => uow.PTPInterviews.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    return _samplePTPInterviews.FirstOrDefault(i => i.Id == id);
                });

            _mockUnitOfWork.Setup(uow => uow.PTPInterviews.UpdateAsync(It.IsAny<PTPInterview>()))
                .Returns((PTPInterview interview) =>
                {
                    return System.Threading.Tasks.Task.CompletedTask;
                });

             _mockUnitOfWork.Setup(uow => uow.PTPUsers.AddAsync(It.IsAny<PTPUsers>()))
                .ReturnsAsync((PTPUsers ptpUser) =>
                {
                    ptpUser.Id = _ptpUserIdCounter++;
                    return ptpUser;
                });

        }
        private MapperConfiguration MappingProfiles()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PTPInterview, PTPInterviewDTO>()
                    .ForMember(dest => dest.ScheduledTimeDisplay, opt => opt.MapFrom(src => src.ScheduledTime.GetDisplayName()));
                cfg.CreateMap<PTPQuestions, PTPQuestionDTO>();

            });
            return mapperConfig;
        }

        #endregion

    }
}

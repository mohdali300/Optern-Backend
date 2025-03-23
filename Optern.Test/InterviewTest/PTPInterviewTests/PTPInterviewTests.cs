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
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using System.Runtime.Intrinsics.X86;
using System.Linq.Expressions;
using Hangfire.States;


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

        #region Get All Upcoming PTPInterviews Tests
        [Test]
        [Category("GetUpcomingPTPInterviewTests")]
        public async Task GetAllUpcomingPTPInterviews_WithValidUserId_ReturnsUpcomingInterviews()
        {
            // Arrange
            string userId = "user1";
            // Act
            var result = await _ptpInterviewService.GetAllUpcomingPTPInterviews("user1");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True, "The response should indicate success.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.Any(), Is.True, "There should be upcoming interviews returned.");
            });
        }

        [Test]
        [Category("GetUpcomingPTPInterviewTests")]
        public async Task GetAllUpcomingPTPInterviews_WithEmptyUserId_ReturnsBadRequest()
        {
            // Arrange
            string userId = ""; // or null

            // Act
            var result = await _ptpInterviewService.GetAllUpcomingPTPInterviews(userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False, "The response should indicate failure for an empty user ID.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("Invalid parameter"));
            });
        }

        [Test]
        [Category("GetUpcomingPTPInterviewTests")]
        public async Task GetAllUpcomingPTPInterviews_NoUpcomingInterviewsFound_ReturnsNotFound()
        {
            // Arrange
            string userId = "user2";

            // Act
            var result = await _ptpInterviewService.GetAllUpcomingPTPInterviews(userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound)
                     .Or.EqualTo(StatusCodes.Status204NoContent));
                Assert.That(result.Data, Is.Empty);
                Assert.That(result.Message, Does.Contain("No Upcoming Interviews"));
            });
        }

        [Test]
        [Category("GetUpcomingPTPInterviewTests")]
        public async Task GetAllUpcomingPTPInterviews_WithInterviewStatusScheduledOrInProgress_ReturnsFilteredResults()
        {
            // Arrange
            string userId = "user1";

            // Act
            var result = await _ptpInterviewService.GetAllUpcomingPTPInterviews(userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True, "Expected success when upcoming interviews exist.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.Count(), Is.EqualTo(2));
                Assert.That(result.Data.All(i => i.Status == InterviewStatus.Scheduled || i.Status == InterviewStatus.InProgress), Is.True);
            });
        }
        [Test]
        [Category("GetPTPInterviewTimeSlotsTests")]
        public async Task GetPTPInterviewTimeSlotsAsync_WithInvalidEnumValues_ReturnsBadRequest()
        {
            // Arrange
            string scheduledDate = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-dd");
            InterviewCategory category = (InterviewCategory)999;
            InterviewQuestionType questionType = (InterviewQuestionType)999;

            // Act
            var result = await _ptpInterviewService.GetPTPInterviewTimeSlotsAsync(category, questionType, scheduledDate);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("Invalid"));
            });
        }

        //[Test]
        //[Category("GetUpcomingPTPInterviewTests")]
        //public async Task GetAllUpcomingPTPInterviews_CalculatesCorrectTimeRemaining()
        //{
        //    // Arrange
        //    string userId = "user1";

        //    // Capture the current time for test consistency.
        //    DateTime testNow = DateTime.UtcNow;

        //    TimeSpan expectedRemaining = TimeSpan.FromHours(2);

        //    string scheduledDate = testNow.AddHours(3).ToString("yyyy-MM-dd");
        //    InterviewTimeSlot testSlot = InterviewTimeSlot.TenPM; 

        //    // Act
        //    var result = await _ptpInterviewService.GetAllUpcomingPTPInterviews(userId);
        //    var interviewDto = result.Data.FirstOrDefault(i => i.Id == 110);

        //    // Assert
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(result.IsSuccess, Is.True, "Expected a successful response.");
        //        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        //        Assert.That(interviewDto, Is.Not.Null, "Expected to retrieve the seeded interview.");


        //        string expectedTimeRemaining = $"{expectedRemaining.Hours}h {expectedRemaining.Minutes}m"; // Adjust this format to match your FormatTimeRemaining method

        //        Assert.That(interviewDto.ScheduledTimeDisplay, Is.EqualTo(testSlot.GetDisplayName()));
        //        Assert.That(interviewDto.TimeRemaining, Is.EqualTo(expectedTimeRemaining));
        //    });
        //}


        #endregion

        #region Get PTPInterview TimeSlots Tests
        [Test]
        [Category("GetPTPInterviewTimeSlotsTests")]
        public async Task GetPTPInterviewTimeSlotsAsync_WithValidFilters_ReturnsAllTimeSlots()
        {
            // Arrange
            // Use a scheduled date with a valid format that is expected to have some interviews.
            string scheduledDate = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-dd");
            InterviewCategory category = InterviewCategory.SQL;
            InterviewQuestionType questionType = InterviewQuestionType.Beginner;


            // Act
            var result = await _ptpInterviewService.GetPTPInterviewTimeSlotsAsync(category, questionType, scheduledDate);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True, "Expected successful response with valid filters");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);

                var dtoForTenPM = result.Data.FirstOrDefault(d => d.TimeSlot == InterviewTimeSlot.TenPM);
                Assert.That(dtoForTenPM, Is.Not.Null, "Expected a DTO for TenPM slot");
                Assert.That(dtoForTenPM.SlotState, Is.EqualTo(TimeSlotState.TakenByOne));

                var dtoForEightAM = result.Data.FirstOrDefault(d => d.TimeSlot == InterviewTimeSlot.EightAM);
                Assert.That(dtoForEightAM, Is.Not.Null, "Expected a DTO for EightAM slot");
                Assert.That(dtoForEightAM.SlotState, Is.EqualTo(TimeSlotState.TakenByTwo));
            });
        }

        [Test]
        [Category("GetPTPInterviewTimeSlotsTests")]
        public async Task GetPTPInterviewTimeSlotsAsync_WhenNoInterviewsExist_ReturnsAllSlotsAsEmpty()
        {
            // Arrange
            string scheduledDate = DateTime.UtcNow.AddDays(5).ToString("yyyy-MM-dd");
            InterviewCategory category = InterviewCategory.SQL;
            InterviewQuestionType questionType = InterviewQuestionType.Beginner;

            var interviewsForDate = _context.PTPInterviews.Where(i => i.ScheduledDate == scheduledDate);
            _context.PTPInterviews.RemoveRange(interviewsForDate);
            await _context.SaveChangesAsync();

            // Act
            var result = await _ptpInterviewService.GetPTPInterviewTimeSlotsAsync(category, questionType, scheduledDate);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True, "Expected successful response even if no interviews exist.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.All(dto => dto.SlotState == TimeSlotState.Empty), Is.True);
            });
        }

        [Test]
        [Category("GetPTPInterviewTimeSlotsTests")]
        public async Task GetPTPInterviewTimeSlotsAsync_WithInterviewsPresent_ReturnsCorrectSlotState()
        {
            // Arrange
            string scheduledDate = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-dd");
            InterviewCategory category = InterviewCategory.SQL;
            InterviewQuestionType questionType = InterviewQuestionType.Beginner;

            // Act
            var result = await _ptpInterviewService.GetPTPInterviewTimeSlotsAsync(category, questionType, scheduledDate);

            // Assert
            Assert.Multiple(() =>
            {
                var dtoTenPM = result.Data.FirstOrDefault(d => d.TimeSlot == InterviewTimeSlot.TenPM);
                var dtoEightAM = result.Data.FirstOrDefault(d => d.TimeSlot == InterviewTimeSlot.EightAM);
                Assert.That(dtoTenPM, Is.Not.Null, "Expected DTO for TenPM slot.");
                Assert.That(dtoEightAM, Is.Not.Null, "Expected DTO for EightAM slot.");

                Assert.That(dtoTenPM.SlotState, Is.EqualTo(TimeSlotState.TakenByOne));
                Assert.That(dtoEightAM.SlotState, Is.EqualTo(TimeSlotState.TakenByTwo));
            });
        }

        [Test]
        [Category("GetPTPInterviewTimeSlotsTests")]
        public async Task GetPTPInterviewTimeSlotsAsync_WithInvalidDateFormat_ReturnsFailure()
        {
            // Arrange
            string invalidDate = "invalid-date";
            InterviewCategory category = InterviewCategory.SQL;
            InterviewQuestionType questionType = InterviewQuestionType.Beginner;

            // Act
            var result = await _ptpInterviewService.GetPTPInterviewTimeSlotsAsync(category, questionType, invalidDate);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False, "Expected failure due to invalid date format.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("Invalid"));
            });
        }



        #endregion

        #region Cancel PTP Interview Tests
        [Test]
        [Category("CancelPTPInterviewTests")]
        public async Task CancelPTPInterviewAsync_WithNonExistentInterview_ReturnsNotFound()
        {
            // Arrange:
            int nonExistentInterviewId = 999;
            string userId = "user1";

            // Act
            var result = await _ptpInterviewService.CancelPTPInterviewAsync(nonExistentInterviewId, userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False, "Should fail when interview does not exist.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("Interview not found"));
            });
        }

        [Test]
        [Category("CancelPTPInterviewTests")]
        public async Task CancelPTPInterviewAsync_WithUserNotAssociated_ReturnsUnauthorized()
        {
            // Arrange: 
            int interviewId = 110;
            
            // Act: Try to cancel with a user not associated with this interview.
            var result = await _ptpInterviewService.CancelPTPInterviewAsync(interviewId, "user2");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
                Assert.That(result.Message, Does.Contain("Unauthorized"));
            });
        }
        [Test]
        [Category("CancelPTPInterviewTests")]
        public async Task CancelPTPInterviewAsync_WhenUserIsTheOnlyParticipant_DeletesInterview()
        {
            // Arrange: 
            int interviewId = 12; //single participant

            // Act: Cancel by the only participant.
            var result = await _ptpInterviewService.CancelPTPInterviewAsync(interviewId, "user4");

            // Assert: Expect the interview to be deleted.
            Assert.Multiple(async () =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Does.Contain("cancelled successfully"));
                var deletedInterview = await _context.PTPInterviews.FindAsync(interviewId);
                Assert.That(deletedInterview, Is.Null);
            });
        }


        [Test]
        [Category("CancelPTPInterviewTests")]
        public async Task CancelPTPInterviewAsync_WhenMultipleUsersExist_RemovesUserAndUpdatesSlotState()
        {
            // Arrange: 
            int interviewId = 102; //interview with multiple participants

            // Act: 
            var result = await _ptpInterviewService.CancelPTPInterviewAsync(interviewId, "user2");

            // Assert: 
            Assert.Multiple(async () =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Does.Contain("cancelled successfully"));

                var updatedInterview = await _context.PTPInterviews.FindAsync(interviewId);
                Assert.That(updatedInterview, Is.Not.Null);

                Assert.That(updatedInterview.PeerToPeerInterviewUsers.Count, Is.EqualTo(1));

                Assert.That(updatedInterview.SlotState, Is.EqualTo(TimeSlotState.TakenByOne));

                var user2Questions = updatedInterview.PTPQuestionInterviews.Where(q => q.PTPUserId == 22);
                Assert.That(user2Questions.Any(), Is.False);
            });
        }

        #endregion

        #region Get User PTPInterview Questions Tests
        [Test]
        [Category("GetUserPTPInterviewQuestionsTests")]
        public async Task GetUserPTPInterviewQuestionsAsync_WithEmptyUserId_ReturnsBadRequest()
        {
            // Arrange
            int interviewId = 110; 
            string emptyUserId = ""; // invalid

            // Act
            var result = await _ptpInterviewService.GetUserPTPInterviewQuestionsAsync(interviewId, emptyUserId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False, "Expected failure when user ID is empty.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("Invalid")
                    .Or.Contain("required"), "Expected error message about invalid user ID.");
            });
        }

        [Test]
        [Category("GetUserPTPInterviewQuestionsTests")]
        public async Task GetUserPTPInterviewQuestionsAsync_WithInvalidInterviewID_ReturnsNotFound()
        {
            // Arrange
            int invalidInterviewId = 0; 
            string userId = "user1";

            // Act
            var result = await _ptpInterviewService.GetUserPTPInterviewQuestionsAsync(invalidInterviewId, userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False, "Expected failure when interview ID is invalid.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("Interview not found"));
            });
        }

        [Test]
        [Category("GetUserPTPInterviewQuestionsTests")]
        public async Task GetUserPTPInterviewQuestionsAsync_WithNonExistentInterview_ReturnsNotFound()
        {
            // Arrange
            int nonExistentInterviewId = 999; // Interview ID that does not exist
            string userId = "user1";

            // Act
            var result = await _ptpInterviewService.GetUserPTPInterviewQuestionsAsync(nonExistentInterviewId, userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False, "Expected failure when interview is not found.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound), "Expected status 404 for non-existent interview.");
                Assert.That(result.Message, Does.Contain("Interview not found"), "Expected error message to mention 'Interview not found'.");
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.Count, Is.EqualTo(0), "Expected an empty list of questions.");
            });
        }
        [Test]
        [Category("GetUserPTPInterviewQuestionsTests")]
        public async Task GetUserPTPInterviewQuestionsAsync_WithUserNotAssociated_ReturnsUnauthorized()
        {
            // Arrange
            int interviewId = 110;

            string userId = "user2"; // user2 is not associated

            // Act
            var result = await _ptpInterviewService.GetUserPTPInterviewQuestionsAsync(interviewId, userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False, "Expected failure when user is not associated with the interview.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden), "Expected status 403 for unauthorized access.");
                Assert.That(result.Message, Does.Contain("Unauthorized"), "Expected error message to mention unauthorized.");
            });
        }

        [Test]
        [Category("GetUserPTPInterviewQuestionsTests")]
        public async Task GetUserPTPInterviewQuestionsAsync_WhenNoQuestionsAssigned_ReturnsEmptyList()
        {
            // Arrange
            int interviewId = 18;
            var interview = new PTPInterview
            {
                Id = interviewId,
                ScheduledDate = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd"),
                ScheduledTime = InterviewTimeSlot.TenPM,
                Category = InterviewCategory.SQL,
                QusestionType = InterviewQuestionType.Beginner,
                Status = InterviewStatus.Scheduled,
                PeerToPeerInterviewUsers = new List<PTPUsers>
                {
                    new PTPUsers { Id = 27, UserID = "user3" }
                },
                PTPQuestionInterviews = new List<PTPQuestionInterview>() // No questions assigned
            };
            _context.PTPInterviews.Add(interview);
            await _context.SaveChangesAsync();

            // Act
            var result = await _ptpInterviewService.GetUserPTPInterviewQuestionsAsync(interviewId, "user3");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True, "Expected success even if no questions are assigned.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK), "Expected status 200.");
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.Count, Is.EqualTo(0), "Expected an empty list of questions.");
                Assert.That(result.Message, Does.Contain("No questions found"), "Expected message indicating no questions were found.");
            });
        }

        [Test]
        [Category("GetUserPTPInterviewQuestionsTests")]
        public async Task GetUserPTPInterviewQuestionsAsync_WithQuestionsAssigned_ReturnsQuestionList()
        {
            // Arrange
            int interviewId = 110;
           
            // Act
            var result = await _ptpInterviewService.GetUserPTPInterviewQuestionsAsync(interviewId, "user1");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True, "Expected success when questions are assigned.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.Count, Is.EqualTo(1), "Expected 1 assigned questions.");
                // Optionally, check properties of one of the questions
                Assert.That(result.Data.First().Title, Is.EqualTo("Question 1"));
                Assert.That(result.Message, Does.Contain("retrieved successfully"));
            });
        }

        [Test]
        [Category("GetUserPTPInterviewQuestionsTests")]
        public async Task GetUserPTPInterviewQuestionsAsync_MultipleUsersIgnoresOtherUsersQuestions()
        {
            // Arrange
            int interviewId = 102;
            

            // Act
            var result = await _ptpInterviewService.GetUserPTPInterviewQuestionsAsync(interviewId, "user2");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True, "Expected success for a valid interview and user.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                // Only the questions assigned to user2 should be returned
                Assert.That(result.Data.Count, Is.EqualTo(1), "Expected exactly 1 question for user2.");
                // Optionally, assert the content of the question
                Assert.That(result.Data.First().Title, Is.EqualTo("Question 1"));
            });
        }


        #endregion

        #region Start PTP Interview Session Tests
        [Test]
        [Category("StartPTPInterviewSessionTests")]
        public async Task StartPTPInterviewSessionAsync_WithEmptyUserIdOrInvaildInterviewId_ReturnsBadRequest()
        {
            // Arrange
            int invalidInterviewId = 0;
            string emptyUserId = ""; //or null

            // Act
            var result1 = await _ptpInterviewService.StartPTPInterviewSessionAsync(invalidInterviewId, "user1");
            var result2 = await _ptpInterviewService.StartPTPInterviewSessionAsync(100, emptyUserId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result1.IsSuccess, Is.False, "Expected failure when interviewId is Invaild.");
                Assert.That(result1.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result1.Message, Does.Contain("Invalid user or interview id"));

                Assert.That(result2.IsSuccess, Is.False, "Expected failure when userId is empty.");
                Assert.That(result2.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result2.Message, Does.Contain("Invalid user or interview id"));
            });
        }

        [Test]
        [Category("StartPTPInterviewSessionTests")]
        public async Task StartPTPInterviewSessionAsync_WithNonExistentInterview_ReturnsNotFound()
        {
            // Arrange
            int nonExistentInterviewId = 999; // invaild id 
            string userId = "user1";

            // Act
            var result = await _ptpInterviewService.StartPTPInterviewSessionAsync(nonExistentInterviewId, userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False, "Expected failure when interview is not found.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("Interview not found"));
            });
        }

        [Test]
        [Category("StartPTPInterviewSessionTests")]
        public async Task StartPTPInterviewSessionAsync_WithUserNotAssociated_ReturnsUnauthorized()
        {
            // Arrange: 
            int interviewId = 110;

            string userId = "user2"; // not associated with the interview

            // Act
            var result = await _ptpInterviewService.StartPTPInterviewSessionAsync(interviewId, userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False, "Expected failure when user is not associated with the interview.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
                Assert.That(result.Message, Does.Contain("user isn’t a participant"));
            });
        }

        [Test]
        [Category("StartPTPInterviewSessionTests")]
        public async Task StartPTPInterviewSessionAsync_WhenInValidTimeRange_ReturnsBadRequest()
        {
            // Arrange: Seed an interview where the current time is not within the valid window.
            int notComeInterviewId = 110;
            int tooLateInterviewId = 102;

            // Act
            var result1 = await _ptpInterviewService.StartPTPInterviewSessionAsync(notComeInterviewId, "user1");
            var result2 = await _ptpInterviewService.StartPTPInterviewSessionAsync(tooLateInterviewId, "user2");


            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result1.IsSuccess, Is.False);
                Assert.That(result1.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result1.Message, Does.Contain("not come yet"));

                Assert.That(result2.IsSuccess, Is.False);
                Assert.That(result2.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result2.Message, Does.Contain("is too late"));
            });
        }

        [Test]
        [Category("StartPTPInterviewSessionTests")]
        public async Task StartPTPInterviewSessionAsync_WithValidData_StartsSessionSuccessfully() //fine tuning date to match
        {
            // Arrange: 
            int interviewId = 113;
            
            var scheduledDate = DateTime.Now.AddMinutes(23).ToString("yyyy-MM-dd"); 
            TestContext.WriteLine($"ScheduledDate: {scheduledDate}");                      
            var interview = new PTPInterview
            {
                Id = interviewId,
                ScheduledDate = scheduledDate,
                ScheduledTime = InterviewTimeSlot.SixPM,
                Category = InterviewCategory.SQL,
                QusestionType = InterviewQuestionType.Beginner,
                SlotState = TimeSlotState.TakenByOne,
                Status = InterviewStatus.Scheduled,
                PeerToPeerInterviewUsers = new List<PTPUsers>
                {
                    new PTPUsers { Id = 35, UserID = "user2" }
                },
                PTPQuestionInterviews = new List<PTPQuestionInterview>()
            };
            _context.PTPInterviews.Add(interview);
            await _context.SaveChangesAsync();

            // Act
            var result = await _ptpInterviewService.StartPTPInterviewSessionAsync(interviewId, "user2");

            // Assert
            Assert.Multiple(async () =>
            {
                Assert.That(result.IsSuccess, Is.True, "Expected the session to start successfully with valid inputs.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Does.Contain("Interview session strated").Or.Contain("started"));

                // Optionally, verify that the interview's status is updated in the database:
                var updatedInterview = await _context.PTPInterviews.FindAsync(interviewId);
                Assert.That(updatedInterview.Status, Is.EqualTo(InterviewStatus.InProgress));
            });
        }



        #endregion

        #region End PTP Interview Session Tests

        [Test]
        [Category("EndPTPInterviewSessionTests")]
        public async Task EndPTPInterviewSessionAsync_WithNonExistentInterview_ReturnsNotFound()
        {
            // Arrange
            int nonExistentInterviewId = 999; 

            // Act
            var result = await _ptpInterviewService.EndPTPInterviewSessionAsync(nonExistentInterviewId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False, "Expected failure when interview is not found.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("Interview not found"));
            });
        }

        [Test]
        [Category("EndPTPInterviewSessionTests")]
        public async Task EndPTPInterviewSessionAsync_WithInterviewNotInProgress_ReturnsBadRequest()
        {
            // Arrange
            int interviewId = 110;
           
            // Act
            var result = await _ptpInterviewService.EndPTPInterviewSessionAsync(interviewId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("didn’t begin"), "Expected message indicating the interview has not started.");
            });
        }

        [Test]
        [Category("EndPTPInterviewSessionTests")]
        public async Task EndPTPInterviewSessionAsync_WithValidData_StartsSessionSuccessfully()
        {
            // Arrange
            int interviewId = _interviewIdCounter++;
            var interview = new PTPInterview
            {
                Id = interviewId,
                ScheduledDate = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd"),
                ScheduledTime = InterviewTimeSlot.TenPM,
                Category = InterviewCategory.SQL,
                QusestionType = InterviewQuestionType.Beginner,
                Status = InterviewStatus.InProgress, // Valid state to end
                PeerToPeerInterviewUsers = new List<PTPUsers>
                {
                    new PTPUsers { Id = 2, UserID = "user1" }
                },
                PTPQuestionInterviews = new List<PTPQuestionInterview>()
            };
            _context.PTPInterviews.Add(interview);
            await _context.SaveChangesAsync();

            // Act
            var result = await _ptpInterviewService.EndPTPInterviewSessionAsync(interviewId);

            // Assert
            Assert.Multiple(async () =>
            {
                Assert.That(result.IsSuccess, Is.True, "Expected successful response for valid end session.");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Does.Contain("Interview session ended"));
                var updatedInterview = await _context.PTPInterviews.FindAsync(interviewId);
                Assert.That(updatedInterview.Status, Is.EqualTo(InterviewStatus.Completed));
            });
        }
        #endregion

        #region Helpers
        private async void CreateSampleData()
        {

            _samplePTPQuestions = new List<PTPQuestions>
            {
                new PTPQuestions { Id = 101, Title = "Question 1", Content = "Content 1", Hints = "Hint 1", Answer = "Answer 1", Category = InterviewCategory.SQL, QusestionType = InterviewQuestionType.Intermediate },
                new PTPQuestions { Id = 102, Title = "Question 2", Content = "Content 2", Hints = "Hint 2", Answer = "Answer 2", Category = InterviewCategory.SQL, QusestionType = InterviewQuestionType.Intermediate },
                new PTPQuestions { Id = 103, Title = "Question 3", Content = "Content 3", Hints = "Hint 3", Answer = "Answer 3", Category = InterviewCategory.SQL, QusestionType = InterviewQuestionType.Intermediate },
            };
            _samplePTPInterviews = new List<PTPInterview>
            {
               new PTPInterview
               {
                   Id = 110,
                   ScheduledDate = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-dd"),
                   //ScheduledDate="2025-03-23",
                   ScheduledTime = InterviewTimeSlot.TenPM, 
                   Category = InterviewCategory.SQL,
                   QusestionType = InterviewQuestionType.Beginner,
                   SlotState = TimeSlotState.TakenByOne,
                   Status = InterviewStatus.Scheduled,
                   PeerToPeerInterviewUsers = new List<PTPUsers>
                   {
                       new PTPUsers { Id = 150, UserID = "user1" }
                   },
                   PTPQuestionInterviews=new List<PTPQuestionInterview>
                   {
                       new PTPQuestionInterview
                       {
                           Id = 301,
                           PTPInterviewId = 110,
                           PTPQuestionId = 101,
                           PTPUserId = 150
                       }
                   }
               },
               new PTPInterview
               {
                   Id = 111,
                   ScheduledDate = DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-dd"),
                   //ScheduledDate="2025-03-25",
                   ScheduledTime = InterviewTimeSlot.TenAM,
                   Category = InterviewCategory.SQL,
                   QusestionType = InterviewQuestionType.Beginner,
                   SlotState = TimeSlotState.TakenByOne,
                   Status = InterviewStatus.InProgress,
                   PeerToPeerInterviewUsers = new List<PTPUsers>
                   {
                       new PTPUsers { Id = 120, UserID = "user1" }
                   }
               },
               new PTPInterview
               {
                   Id = 106,
                   ScheduledDate = DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-dd"),
                   ScheduledTime = InterviewTimeSlot.EightAM,
                   Category = InterviewCategory.SQL,
                   QusestionType = InterviewQuestionType.Beginner,
                   SlotState = TimeSlotState.TakenByOne,
                   Status = InterviewStatus.Completed, // Should be filtered out
                   PeerToPeerInterviewUsers = new List<PTPUsers>
                   {
                       new PTPUsers { Id = 6, UserID = "user1" }
                   }
               },
               new PTPInterview
               {
                   Id = 103,
                   ScheduledDate = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"),
                   ScheduledTime = InterviewTimeSlot.TenPM,
                   Category = InterviewCategory.SQL,
                   QusestionType = InterviewQuestionType.Beginner,
                   SlotState = TimeSlotState.TakenByOne,
                   Status = InterviewStatus.Scheduled,
                   PeerToPeerInterviewUsers = new List<PTPUsers>
                   {
                    new PTPUsers { Id = 3, UserID = "user2" }
                   }
               },
               new PTPInterview
               {
                Id = 102,
                ScheduledDate = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-dd"),
                ScheduledTime = InterviewTimeSlot.EightAM,
                Category = InterviewCategory.SQL,
                QusestionType = InterviewQuestionType.Beginner,
                SlotState = TimeSlotState.TakenByTwo,
                Status = InterviewStatus.Scheduled,
                PeerToPeerInterviewUsers = new List<PTPUsers>
                {
                    new PTPUsers { Id = 22, UserID = "user2" },
                    new PTPUsers { Id = 33, UserID = "user3" }
                },
                PTPQuestionInterviews=new List<PTPQuestionInterview>
                   {
                       new PTPQuestionInterview
                       {
                           Id = 22,
                           PTPInterviewId = 102,
                           PTPQuestionId = 101,
                           PTPUserId = 22
                       },
                       new PTPQuestionInterview
                       {
                           Id = 33,
                           PTPInterviewId = 102,
                           PTPQuestionId = 102,
                           PTPUserId = 33
                       }
                   }
               },
               new PTPInterview
               {
                   Id = 12,
                   ScheduledDate = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd"),
                   ScheduledTime = InterviewTimeSlot.TenPM,
                   Category = InterviewCategory.SQL,
                   QusestionType = InterviewQuestionType.Beginner,
                   SlotState = TimeSlotState.TakenByOne,
                   Status = InterviewStatus.Scheduled,
                   PeerToPeerInterviewUsers = new List<PTPUsers>
                   {
                       new PTPUsers { Id = 55, UserID = "user4" }
                   },
                   PTPQuestionInterviews = new List<PTPQuestionInterview>()
               }
            };
            //add to the in memory database
            _context.PTPQuestions.AddRange(_samplePTPQuestions);
            _context.PTPInterviews.AddRange(_samplePTPInterviews);
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
            _mockUnitOfWork.Setup(uow => uow.PTPInterviews.GetAllByExpressionAsync(It.IsAny<Expression<Func<PTPInterview, bool>>>()))
                 .ReturnsAsync((Expression<Func<PTPInterview, bool>> predicate) =>
                 {
                     return _samplePTPInterviews.Where(predicate.Compile());
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
                cfg.CreateMap<PTPInterview, UpcomingPTPInterviewDTO>()
                    .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
                    .ForMember(dest => dest.ScheduledTimeDisplay, opt => opt.MapFrom(src => src.ScheduledTime.GetDisplayName()))
                    .ForMember(dest => dest.Questions, opt => opt.Ignore());

            });
            return mapperConfig;
        }

        #endregion

    }
}

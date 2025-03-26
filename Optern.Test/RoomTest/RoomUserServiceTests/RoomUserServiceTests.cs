using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MimeKit;
using Moq;
using NUnit.Framework;
using Optern.Application.DTOs.Room;
using Optern.Application.DTOs.RoomUser;
using Optern.Application.DTOs.UserNotification;
using Optern.Application.Interfaces.IChatService;
using Optern.Application.Interfaces.IRoomUserService;
using Optern.Application.Interfaces.IUserNotificationService;
using Optern.Application.Response;
using Optern.Domain.Entities;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.ExternalInterfaces.ICacheService;
using Optern.Infrastructure.Services.RoomUserService;
using Optern.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Task = System.Threading.Tasks.Task;

namespace Optern.Test.RoomTest.RoomUserServiceTests
{
    [TestFixture]
    public class RoomUserServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private OpternDbContext _context;
        private Mock<IMapper> _mockMapper;
        private Mock<IChatService> _mockChatService;
        private Mock<IUserNotificationService> _mockUserNotificationService;
        private Mock<ICacheService> _mockCacheService; 
        private IRoomUserService _roomUserService;

        private List<Room> _rooms;
        private List<ResponseRoomDTO> _cachedRooms;
        private List<UserRoom> _roomUsers;
        private List<ApplicationUser> _users;
        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockChatService= new Mock<IChatService>();
            _mockUserNotificationService=   new Mock<IUserNotificationService>();
            _mockCacheService= new Mock<ICacheService>();
            var options = new DbContextOptionsBuilder<OpternDbContext>()
               .UseInMemoryDatabase(databaseName: "OpternTestDb")
               .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
               .Options;
            _context = new OpternDbContext(options);
            LoadData();
            _roomUserService = new RoomUserService(
                _mockUnitOfWork.Object,
                _context,
                _mockMapper.Object,
                _mockChatService.Object,
                _mockCacheService.Object,
                _mockUserNotificationService.Object);

        }

        [TearDown]
        public void TearDown() {
            _context.Database.EnsureDeleted();
        }

        #region GetAllCollaboratorsTests
        [Test]
        [Category("GetAllCollaboratorsTests")]
        [TestCase("")]
        public async Task GetAllCollaborators_ShouldReturn400InvalidRoomId_WhenRoomIdIsEmpty(string roomId, bool? isAdmin = null)
        {
            var result = await _roomUserService.GetAllCollaboratorsAsync(roomId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Data, Is.InstanceOf<List<RoomUserDTO>>());
            Assert.That(result.Data.Count, Is.EqualTo(0));
            Assert.That(result.Message, Does.Contain("Room ID cannot be empty."));
        }

        [Test]
        [Category("GetAllCollaboratorsTests")]
        [TestCase("room150")]
        public async Task GetAllCollaborators_ShouldReturn404NotFound_WhenRoomNotFound(string roomId, bool? isAdmin = null)
        {
            MockUniteOfWork();
            var result = await _roomUserService.GetAllCollaboratorsAsync(roomId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Data, Is.InstanceOf<List<RoomUserDTO>>());
            Assert.That(result.Data.Count, Is.EqualTo(0));
            Assert.That(result.Message, Does.Contain("Room not found."));
        }

        [Test]
        [Category("GetAllCollaboratorsTests")]
        [TestCase("room6")]
        public async Task GetAllCollaborators_ShouldReturn404NotFound_WhenRoomDoesNotContainsAnyCollaborators(string roomId, bool? isAdmin = null)
        {
            MockUniteOfWork();
            var result = await _roomUserService.GetAllCollaboratorsAsync(roomId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Data, Is.InstanceOf<List<RoomUserDTO>>());
            Assert.That(result.Data.Count, Is.EqualTo(0));
            Assert.That(result.Message, Does.Contain("No collaborators found."));
        }

        [Test]
        [Category("GetAllCollaboratorsTests")]
        [TestCase("room1")]
        public async Task GetAllCollaborators_ShouldReturnAllCollaborators_WhenRoomContainsCollaborators(string roomId, bool? isAdmin = null)
        {
            MockUniteOfWork();
            MapListOfUserRoomData();
            var result = await _roomUserService.GetAllCollaboratorsAsync(roomId);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Data, Has.Exactly(5).Items);
            Assert.That(result.Data, Is.Ordered.By(nameof(RoomUserDTO.JoinedAt)).Descending);
            Assert.That(!result.Data.Any(s => s.IsAccepted), Is.False);
            Assert.That(result.Message, Does.Contain("Collaborators retrieved successfully."));
        }
        [Test]
        [Category("GetAllCollaboratorsTests")]
        [TestCase("room1", true)]
        public async Task GetAllCollaborators_ShouldReturnAllAdminsForRoom_WhenRoomContainsAdminsCollaborators(string roomId, bool isAdmin)
        {
            MockUniteOfWork();
            MapListOfUserRoomData();
            var result = await _roomUserService.GetAllCollaboratorsAsync(roomId, isAdmin);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Data, Has.Exactly(2).Items);
            Assert.That(result.Data, Is.Ordered.By(nameof(RoomUserDTO.JoinedAt)).Descending);
            Assert.That(!result.Data.Any(s => s.IsAccepted), Is.False);
            Assert.That(result.Data.First().UserName, Is.EqualTo("david.clark"));
            Assert.That(result.Message, Does.Contain("Collaborators retrieved successfully."));
        }
        [Test]
        [Category("GetAllCollaboratorsTests")]
        [TestCase("room1", false)]
        public async Task GetAllCollaborators_ShouldReturnAllNotAdminsCollaboratorsForRoom_WhenRoomContainsCollaborators(string roomId, bool isAdmin)
        {
            MockUniteOfWork();
            MapListOfUserRoomData();
            var result = await _roomUserService.GetAllCollaboratorsAsync(roomId, isAdmin);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Data, Has.Exactly(3).Items);
            Assert.That(result.Data, Is.Ordered.By(nameof(RoomUserDTO.JoinedAt)).Descending);
            Assert.That(!result.Data.Any(s => s.IsAccepted), Is.False);
            Assert.That(result.Data.First().UserName, Is.EqualTo("sara.jones"));
            Assert.That(result.Message, Does.Contain("Collaborators retrieved successfully."));
        }
        [Test]
        [Category("GetAllCollaboratorsTests")]
        [TestCase("room1", false)]
        public async Task GetAllCollaborators_ShouldThrowException_WhenExceptionThrown(string roomId, bool isAdmin)
        {
            _mockUnitOfWork.Setup(uow => uow.Rooms.GetByIdAsync(It.IsAny<string>()))
                   .ThrowsAsync(new Exception("An error occurred while retrieving collaborators"));
            var result = await _roomUserService.GetAllCollaboratorsAsync(roomId, isAdmin);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(result.Message, Does.Contain("An error occurred while retrieving collaborators"));
        }
        #endregion

        #region GetPendingRequestsTests
        [Test]
        [Category("GetPendingRequestsTests")]
        [TestCase("", "user1")]
        [TestCase("user1", "")]
        public async Task GetPendingRequests_ShouldReturn400InvalidIds_WhenIdsEmpty(string roomId, string leaderId)
        {
            var result = await _roomUserService.GetPendingRequestsAsync(roomId, leaderId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Data, Is.InstanceOf<List<RoomUserDTO>>());
            Assert.That(result.Data.Count, Is.EqualTo(0));
            Assert.That(result.Message, Does.Contain("Data cannot be empty"));
        }

        [Test]
        [Category("GetPendingRequestsTests")]
        [TestCase("room48", "user1")]
        public async Task GetPendingRequests_ShouldReturn404NotFound_WhenRoomNotFound(string roomId, string leaderId)
        {
            MockUniteOfWork();
            var result = await _roomUserService.GetPendingRequestsAsync(roomId, leaderId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Data, Is.InstanceOf<List<RoomUserDTO>>());
            Assert.That(result.Data.Count, Is.EqualTo(0));
            Assert.That(result.Message, Does.Contain("Room not found"));
        }

        [Test]
        [Category("GetPendingRequestsTests")]
        [TestCase("room1", "user2")]
        public async Task GetPendingRequests_ShouldReturn403UnauthorizedUser_WhenUserIsNotAuthorized(string roomId, string leaderId)
        {
            MockUniteOfWork();
            var result = await _roomUserService.GetPendingRequestsAsync(roomId, leaderId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
            Assert.That(result.Data, Is.InstanceOf<List<RoomUserDTO>>());
            Assert.That(result.Data.Count, Is.EqualTo(0));
            Assert.That(result.Message, Does.Contain("Unauthorized access"));
        }

        [Test]
        [Category("GetPendingRequestsTests")]
        [TestCase("room2", "user2")]
        public async Task GetPendingRequests_ShouldReturn200Success_WhenUserIsAuthorized(string roomId, string leaderId)
        {
            MockUniteOfWork();
            var result = await _roomUserService.GetPendingRequestsAsync(roomId, leaderId);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Data, Is.InstanceOf<List<RoomUserDTO>>());
            Assert.That(result.Data.Count, Is.EqualTo(3));
            Assert.That(result.Data, Is.Ordered.By(nameof(RoomUserDTO.JoinedAt)).Ascending);
            Assert.That(result.Message, Does.Contain("pending requests"));
        }
        [Test]
        [Category("GetPendingRequestsTests")]
        [TestCase("room2", "user2")]
        public async Task GetPendingRequests_ShouldThrowException_WhenExceptionThrown(string roomId, string leaderId)
        {
            _mockUnitOfWork.Setup(uow => uow.Rooms.GetByIdAsync(It.IsAny<string>()))
                   .ThrowsAsync(new Exception("An error occurred while retrieving requests"));
            var result = await _roomUserService.GetPendingRequestsAsync(roomId, leaderId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(result.Message, Does.Contain("An error occurred while retrieving requests"));
        }
        #endregion

        #region DeleteCollaboratorTests

        [Test]
        [Category("DeleteCollaboratorTests")]
        [TestCase("room1", "", "user1")]
        public async Task DeleteCollaborator_ShouldReturn400InvalidData_WhenDataIsEmpty(string roomId, string TargetUserId, string leaderId)
        {
            var result = await _roomUserService.DeleteCollaboratorAsync(roomId, TargetUserId, leaderId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Data, Is.InstanceOf<RoomUserDTO>());
            Assert.That(result.Message, Does.Contain("Data cannot be empty"));
        }

        [Test]
        [Category("DeleteCollaboratorTests")]
        [TestCase("room190", "user5", "user1")]
        public async Task DeleteCollaborator_ShouldReturn404NotFound_WhenRoomNotFound(string roomId, string TargetUserId, string leaderId)
        {
            MockUniteOfWork();
            var result = await _roomUserService.DeleteCollaboratorAsync(roomId, TargetUserId, leaderId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Data, Is.InstanceOf<RoomUserDTO>());
            Assert.That(result.Message, Does.Contain("Room not found"));
        }

        [Test]
        [Category("DeleteCollaboratorTests")]
        [TestCase("room3", "user4", "user5")]
        public async Task DeleteCollaborator_ShouldReturn403UnAuthorized_WhenUserIsUnAuthorizedToDeleteCollaborator(string roomId, string TargetUserId, string leaderId)
        {
            MockUniteOfWork();
            var result = await _roomUserService.DeleteCollaboratorAsync(roomId, TargetUserId, leaderId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
            Assert.That(result.Data, Is.InstanceOf<RoomUserDTO>());
            Assert.That(result.Message, Does.Contain("Unauthorized: Only leaders can remove collaborators"));
        }
        [Test]
        [Category("DeleteCollaboratorTests")]
        [TestCase("room3", "user12", "user1")]
        public async Task DeleteCollaborator_ShouldReturn404NotFound_WhenTargetUserToDeleteNotFoundInRoom(string roomId, string TargetUserId, string leaderId)
        {
            MockUniteOfWork();
            var result = await _roomUserService.DeleteCollaboratorAsync(roomId, TargetUserId, leaderId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Data, Is.InstanceOf<RoomUserDTO>());
            Assert.That(result.Message, Does.Contain("User not found in this room"));
        }

        //[Test]
        //[Category("DeleteCollaboratorTests")]
        //[TestCase("room4", "user5", "user2")]
        //public async Task DeleteCollaborator_ShouldReturn400Failed_WhenRoomContainsOnlyOneAdmin(string roomId, string TargetUserId, string leaderId)
        //{
        //    MockUniteOfWork();
        //    var result = await _roomUserService.DeleteCollaboratorAsync(roomId, TargetUserId, leaderId);
        //    Assert.That(result.IsSuccess, Is.False);
        //    Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        //    Assert.That(result.Data, Is.InstanceOf<RoomUserDTO>());
        //    Assert.That(result.Message, Does.Contain("Cannot remove the last leader"));
        //}

        [Test]
        [Category("DeleteCollaboratorTests")]
        [TestCase("room2", "user2", "user2")]
        public async Task DeleteCollaborator_ShouldReturn400Failed_WhenAdminTryToDeleteHimSelf(string roomId, string TargetUserId, string leaderId)
        {
            MockUniteOfWork();
            var result = await _roomUserService.DeleteCollaboratorAsync(roomId, TargetUserId, leaderId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Data, Is.InstanceOf<RoomUserDTO>());
            Assert.That(result.Message, Does.Contain("Cannot remove yourself as the last leader"));
        }

        [Test]
        [Category("DeleteCollaboratorTests")]
        [TestCase("room1", "user1", "user5")]
        public async Task DeleteCollaborator_ShouldReturn400Failed_WhenAnotherAdminTryToDeleteTheCreatorForRoom(string roomId, string TargetUserId, string leaderId)
        {
            MockUniteOfWork();
            var result = await _roomUserService.DeleteCollaboratorAsync(roomId, TargetUserId, leaderId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Data, Is.InstanceOf<RoomUserDTO>());
            Assert.That(result.Message, Does.Contain("Cannot remove The Creator For This Room"));
        }
        [Test]
        [Category("DeleteCollaboratorTests")]
        [TestCase("room1", "user5", "user1")]
        public async Task DeleteCollaborator_ShouldReturn200Success_WhenAdminIsAuthorizedToDelete(string roomId, string TargetUserId, string leaderId)
        {
            MockUniteOfWork();
            MapRoomDataItem();
            var result = await _roomUserService.DeleteCollaboratorAsync(roomId, TargetUserId, leaderId);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Data.UserName, Is.EqualTo("david.clark"));
            Assert.That(result.Message, Does.Contain("Collaborator removed successfully"));
        }

        [Test]
        [Category("DeleteCollaboratorTests")]
        [TestCase("room1", "user5", "user1")]
        public async Task DeleteCollaborator_ShouldThrowException_WhenExceptionThrown(string roomId, string TargetUserId, string leaderId)
        {

            _mockUnitOfWork.Setup(uow => uow.Rooms.GetByIdAsync(It.IsAny<string>()))
                    .ThrowsAsync(new Exception("An error occurred while removing collaborator:"));
            var result = await _roomUserService.DeleteCollaboratorAsync(roomId, TargetUserId, leaderId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(result.Message, Does.Contain("An error occurred while removing collaborator:"));
        }
        #endregion

        #region ToggleLeadershipTests
        [Test]
        [Category("ToggleLeadershipTests")]
        [TestCase("room1", "", "")]
        public async Task ToggleLeadership_ShouldReturn400InvalidData_WhenDataIsEmpty(string roomId, string targetUserId, string leaderId)
        {
            var result = await _roomUserService.ToggleLeadershipAsync(roomId, targetUserId, leaderId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Data, Is.InstanceOf<RoomUserDTO>());
            Assert.That(result.Message, Does.Contain("Data cannot be empty"));
        }

        [Test]
        [Category("ToggleLeadershipTests")]
        [TestCase("room150", "user1", "user5")]
        public async Task ToggleLeadership_ShouldReturn404NotFound_WhenRoomNotFound(string roomId, string targetUserId, string leaderId)
        {
            MockUniteOfWork();
            var result = await _roomUserService.ToggleLeadershipAsync(roomId, targetUserId, leaderId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Data, Is.InstanceOf<RoomUserDTO>());
            Assert.That(result.Message, Does.Contain("Room not found"));
        }

        [Test]
        [Category("ToggleLeadershipTests")]
        [TestCase("room1", "user4", "user3")]
        public async Task ToggleLeadership_ShouldReturn403UnAuthorized_WhenUserIsUnAuthorized(string roomId, string targetUserId, string leaderId)
        {
            MockUniteOfWork();
            var result = await _roomUserService.ToggleLeadershipAsync(roomId, targetUserId, leaderId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
            Assert.That(result.Data, Is.InstanceOf<RoomUserDTO>());
            Assert.That(result.Message, Does.Contain("Only existing leaders can modify leadership status"));
        }
        [Test]
        [Category("ToggleLeadershipTests")]
        [TestCase("room1", "user150", "user1")]
        public async Task ToggleLeadership_ShouldReturn404NotFound_WhenTargetUserNotFound(string roomId, string targetUserId, string leaderId)
        {
            MockUniteOfWork();
            var result = await _roomUserService.ToggleLeadershipAsync(roomId, targetUserId, leaderId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Data, Is.InstanceOf<RoomUserDTO>());
            Assert.That(result.Message, Does.Contain("Target user is not a room member"));
        }

        [Test]
        [Category("ToggleLeadershipTests")]
        [TestCase("room5", "user1", "user1")]
        public async Task ToggleLeadership_ShouldReturn400Failed_WhenRoomContainsOnlyOneAdmin(string roomId, string targetUserId, string leaderId)
        {
            MockUniteOfWork();
            var result = await _roomUserService.ToggleLeadershipAsync(roomId, targetUserId, leaderId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Data, Is.InstanceOf<RoomUserDTO>());
            Assert.That(result.Message, Does.Contain("Cannot remove yourself as the last leader"));
        }
        [Test]
        [Category("ToggleLeadershipTests")]
        [TestCase("room1", "user4", "user1")]
        public async Task ToggleLeadership_ShouldReturn200Success_WhenUserIsAuthorized(string roomId, string targetUserId, string leaderId)
        {
            MockUniteOfWork();
            MapRoomDataItem();
            var result = await _roomUserService.ToggleLeadershipAsync(roomId, targetUserId, leaderId);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Data.IsAdmin, Is.True);
            Assert.That(result.Message, Does.Contain("User Assigned to leader"));
        }
        [Test]
        [Category("ToggleLeadershipTests")]
        [TestCase("room1", "user4", "user1")]
        public async Task ToggleLeadership_ShouldThrowException_WhenExceptionThrown(string roomId, string targetUserId, string leaderId)
        {

            _mockUnitOfWork.Setup(uow => uow.Rooms.GetByIdAsync(It.IsAny<string>()))
                 .ThrowsAsync(new Exception("An error occurred while updating leadership status"));
            var result = await _roomUserService.ToggleLeadershipAsync(roomId, targetUserId, leaderId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(result.Message, Does.Contain("An error occurred while updating leadership status"));
        }
        #endregion

        #region AcceptRequestsTests
        [Test]
        [Category("AcceptRequestsTests")]
        public async Task AcceptRequests_ShouldReturn400_WhenNoUserRoomIdAndApproveAllFalse()
        {
            var result = await _roomUserService.AcceptRequestsAsync("room1", "user1", null, false);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Message, Is.EqualTo("Specify either UserRoomId or set ApproveAll to true"));
        }

        [Test]
        [Category("AcceptRequestsTests")]
        public async Task AcceptRequests_ShouldReturn403_WhenLeaderIsNotAuthorized()
        {

            var result = await _roomUserService.AcceptRequestsAsync("room1", "user4", 1, null);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
            Assert.That(result.Message, Is.EqualTo("Unauthorized: Only room leaders can process requests"));
        }
        [Test]
        [Category("AcceptRequestsTests")]
        public async Task AcceptRequests_ShouldReturn404_WhenRequestNotFound()
        {

            var result = await _roomUserService.AcceptRequestsAsync("room1", "user1", 999, null);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Message, Is.EqualTo("Request not found"));
        }
        [Test]
        [Category("AcceptRequestsTests")]
        public async Task AcceptRequests_ShouldApproveRequest_WhenValidAcceptAllProvided()
        {
            MapListOfUserRoomData();
            var result = await _roomUserService.AcceptRequestsAsync("room5", "user1", null, true);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Data.First().UserName, Is.EqualTo("David Clark"));
            Assert.That(result.Data.Count, Is.EqualTo(1));
        }
        #endregion

        #region Load Data
        private void LoadData()
        {
            _rooms = new List<Room>()
            {
                new Room { Id="room1",Name="C#",Description="Description for room 1",RoomType=RoomType.Public,CreatedAt=DateTime.Now.AddMinutes(-15), CreatorId="user1"},
                  new Room { Id="room2",Name=".Net",Description="Description for room 2",RoomType=RoomType.Public,CreatedAt=DateTime.Now.AddMinutes(-15), CreatorId="user1"},
                    new Room { Id="room3",Name="Java",Description="Description for room 3",RoomType=RoomType.Public,CreatedAt=DateTime.Now.AddMinutes(-15), CreatorId="user2"},
                    new Room { Id="room4",Name="Python",Description="Description for room 4",RoomType=RoomType.Public,CreatedAt=DateTime.Now.AddMinutes(-15), CreatorId="user2"},
                    new Room { Id="room5",Name="JavaScript" ,Description="Description for room 5",RoomType=RoomType.Public,CreatedAt=DateTime.Now.AddMinutes(-15), CreatorId="user2"},
                    new Room { Id="room6",Name="Go",Description="Description for room 6",RoomType=RoomType.Public,CreatedAt=DateTime.Now.AddMinutes(-15), CreatorId="user1"}
            };
            _cachedRooms = new List<ResponseRoomDTO>
                {
                    new ResponseRoomDTO { Id = "room1", Name = "C#" },
                    new ResponseRoomDTO { Id = "room2", Name = ".Net" },
                     new ResponseRoomDTO { Id = "room3", Name = ".Java" }
                };
            _roomUsers = new List<UserRoom>
{
    // room1 users
    new UserRoom { Id = 1, UserId = "user1", RoomId = "room1", IsAccepted = true, IsAdmin = true, JoinedAt = DateTime.UtcNow.AddDays(-5) },
    new UserRoom { Id = 2, UserId = "user2", RoomId = "room1", IsAccepted = true, IsAdmin = false, JoinedAt = DateTime.UtcNow.AddDays(-4) },
    new UserRoom { Id = 3, UserId = "user3", RoomId = "room1", IsAccepted = true, IsAdmin = false, JoinedAt = DateTime.UtcNow.AddDays(-3) },
    new UserRoom { Id = 4, UserId = "user4", RoomId = "room1", IsAccepted = true, IsAdmin = false, JoinedAt = DateTime.UtcNow.AddDays(-2) },
    new UserRoom { Id = 5, UserId = "user5", RoomId = "room1", IsAccepted = true, IsAdmin = true, JoinedAt = DateTime.UtcNow.AddDays(-1) },

    // room2 users
    new UserRoom { Id = 6, UserId = "user1", RoomId = "room2", IsAccepted = false, IsAdmin = false, JoinedAt = DateTime.UtcNow.AddDays(-10) },
    new UserRoom { Id = 7, UserId = "user2", RoomId = "room2", IsAccepted = true, IsAdmin = true, JoinedAt = DateTime.UtcNow.AddDays(-9) },
    new UserRoom { Id = 8, UserId = "user3", RoomId = "room2", IsAccepted = false, IsAdmin = false, JoinedAt = DateTime.UtcNow.AddDays(-8) },
    new UserRoom { Id = 9, UserId = "user4", RoomId = "room2", IsAccepted = true, IsAdmin = false, JoinedAt = DateTime.UtcNow.AddDays(-7) },
    new UserRoom { Id = 10, UserId = "user5", RoomId = "room2", IsAccepted = false, IsAdmin = false, JoinedAt = DateTime.UtcNow.AddDays(-6) },

    // room3 users
    new UserRoom { Id = 11, UserId = "user1", RoomId = "room3", IsAccepted = true, IsAdmin = true, JoinedAt = DateTime.UtcNow.AddDays(-15) },
    new UserRoom { Id = 12, UserId = "user2", RoomId = "room3", IsAccepted = true, IsAdmin = false, JoinedAt = DateTime.UtcNow.AddDays(-14) },
    new UserRoom { Id = 13, UserId = "user3", RoomId = "room3", IsAccepted = true, IsAdmin = false, JoinedAt = DateTime.UtcNow.AddDays(-13) },
    new UserRoom { Id = 14, UserId = "user4", RoomId = "room3", IsAccepted = true, IsAdmin = false, JoinedAt = DateTime.UtcNow.AddDays(-12) },
    new UserRoom { Id = 15, UserId = "user5", RoomId = "room3", IsAccepted = true, IsAdmin = false, JoinedAt = DateTime.UtcNow.AddDays(-11) },

    // room4 users
    new UserRoom { Id = 16, UserId = "user1", RoomId = "room4", IsAccepted = true, IsAdmin = false, JoinedAt = DateTime.UtcNow.AddDays(-20) },
    new UserRoom { Id = 17, UserId = "user2", RoomId = "room4", IsAccepted = true, IsAdmin = true, JoinedAt = DateTime.UtcNow.AddDays(-19) },
    new UserRoom { Id = 18, UserId = "user3", RoomId = "room4", IsAccepted = true, IsAdmin = false, JoinedAt = DateTime.UtcNow.AddDays(-18) },
    new UserRoom { Id = 19, UserId = "user4", RoomId = "room4", IsAccepted = true, IsAdmin = false, JoinedAt = DateTime.UtcNow.AddDays(-17) },
    new UserRoom { Id = 20, UserId = "user5", RoomId = "room4", IsAccepted = false, IsAdmin = true, JoinedAt = DateTime.UtcNow.AddDays(-16) },

    // room5 users
    new UserRoom { Id = 21, UserId = "user1", RoomId = "room5", IsAccepted = true, IsAdmin = true, JoinedAt = DateTime.UtcNow.AddDays(-25) },
    new UserRoom { Id = 22, UserId = "user2", RoomId = "room5", IsAccepted = true, IsAdmin = false, JoinedAt = DateTime.UtcNow.AddDays(-24) },
    new UserRoom { Id = 25, UserId = "user5", RoomId = "room5", IsAccepted = false, IsAdmin = false, JoinedAt = DateTime.UtcNow.AddDays(-23) },
};



            _users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = "user1",
                UserName = "john.doe",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                ProfilePicture = "profile1.jpg",
                JobTitle = "Developer",
                Gender = "Male",
                Country = "USA",
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new ApplicationUser
            {
                Id = "user2",
                UserName = "alice.smith",
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice.smith@example.com",
                ProfilePicture = "profile2.jpg",
                JobTitle = "Designer",
                Gender = "Female",
                Country = "UK",
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new ApplicationUser
            {
                Id = "user3",
                UserName = "michael.brown",
                FirstName = "Michael",
                LastName = "Brown",
                Email = "michael.brown@example.com",
                ProfilePicture = "profile3.jpg",
                JobTitle = "Project Manager",
                Gender = "Male",
                Country = "Canada",
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new ApplicationUser
            {
                Id = "user4",
                UserName = "sara.jones",
                FirstName = "Sara",
                LastName = "Jones",
                Email = "sara.jones@example.com",
                ProfilePicture = "profile4.jpg",
                JobTitle = "QA Engineer",
                Gender = "Female",
                Country = "Australia",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new ApplicationUser
            {
                Id = "user5",
                UserName = "david.clark",
                FirstName = "David",
                LastName = "Clark",
                Email = "david.clark@example.com",
                ProfilePicture = "profile5.jpg",
                JobTitle = "DevOps Engineer",
                Gender = "Male",
                Country = "Germany",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            }, new ApplicationUser
            {
                Id = "user6",
                UserName = "david2.clark2",
                FirstName = "David2",
                LastName = "Clark2",
                Email = "david2.clark2@example.com",
                ProfilePicture = "profile55.jpg",
                JobTitle = "DevOps Engineer | Cloud Engineer",
                Gender = "Male",
                Country = "Germany",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };
 
            _context.Rooms.AddRange(_rooms);
            _context.Users.AddRange(_users);
            _context.UserRooms.AddRange(_roomUsers);
            _context.SaveChanges();
        }
        #endregion

        private void MapListOfUserRoomData()
        {
            _mockMapper.Setup(m => m.Map<List<RoomUserDTO>>(It.IsAny<List<UserRoom>>()))
          .Returns((List<UserRoom> source) => source.Select(ur => new RoomUserDTO
          {
              Id = ur.Id,
              UserId = ur.UserId,
              RoomId = ur.RoomId,
              UserName = ur.User.UserName,
              ProfilePicture = ur.User.ProfilePicture,
              IsAdmin = ur.IsAdmin,
              JoinedAt = ur.JoinedAt,
              IsAccepted = ur.IsAccepted,
          }).ToList());
        }

        private void MapRoomDataItem()
        {
            _mockMapper.Setup(m => m.Map<RoomUserDTO>(It.IsAny<UserRoom>()))
                .Returns((UserRoom ur) => new RoomUserDTO
                {
                    Id = ur.Id,
                    UserId = ur.UserId,
                    RoomId = ur.RoomId,
                    UserName = ur.User.UserName,
                    ProfilePicture = ur.User.ProfilePicture,
                    IsAdmin = ur.IsAdmin,
                    JoinedAt = ur.JoinedAt,
                    IsAccepted = ur.IsAccepted,
                });
        }

        private void MockUniteOfWork()
        {
            _mockUnitOfWork.Setup(uow => uow.Rooms.GetByIdAsync(It.IsAny<string>()))
           .ReturnsAsync((string id) =>
           {
               var room = _rooms.FirstOrDefault(x => x.Id == id);
               return room!;
           });
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using Optern.Application.DTOs.Chat;
using Optern.Application.DTOs.Room;
using Optern.Application.Interfaces.IChatService;
using Optern.Application.Interfaces.IRepositoryService;
using Optern.Application.Interfaces.IRoomService;
using Optern.Application.Interfaces.IRoomSkillService;
using Optern.Application.Interfaces.IRoomTrackService;
using Optern.Application.Interfaces.ISkillService;
using Optern.Application.Interfaces.IUserService;
using Optern.Application.Interfaces.IWorkSpaceService;
using Optern.Application.Response;
using Optern.Domain.Entities;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.ExternalInterfaces.ICacheService;
using Optern.Infrastructure.ExternalInterfaces.IFileService;
using Optern.Infrastructure.Services.RoomService;
using Optern.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Task = System.Threading.Tasks.Task;

namespace Optern.Test.RoomTest.RoomServiceTests
{
    [TestFixture]
    public class RoomServiceTests
    {
        private IRoomService _roomService;
        private OpternDbContext _context;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IMapper> _mockMapper;
        private Mock<IUserService> _mockUserService;
        private Mock<ICloudinaryService> _mockCloudinaryService;
        private Mock<IRoomPositionService> _mockRoomPositionService;
        private Mock<IRoomTrackService> _mockRoomTrackService;
        private Mock<ISkillService> _mockSkillService;
        private Mock<IRoomSkillService> _mockRoomSkillService;
        private Mock<IRepositoryService> _mockRepositoryService;
        private Mock<IChatService> _mockChatService;
        private Mock<ICacheService> _mockCacheService;
        private List<Room> _rooms;
        private List<ResponseRoomDTO> _cachedRooms;
        private List<UserRoom> _roomUsers;
        private List<ApplicationUser> _users;
        private List<Skills> _skills;
        private List<RoomSkills> _roomSkills;
        private List<RoomTrack> _roomTracks;
        private List<Track> _tracks;

        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockUserService= new Mock<IUserService>();
            _mockCloudinaryService= new Mock<ICloudinaryService>();
            _mockRoomPositionService= new Mock<IRoomPositionService>();
            _mockRoomTrackService= new Mock<IRoomTrackService>();
            _mockSkillService= new Mock<ISkillService>();
            _mockRoomSkillService = new Mock<IRoomSkillService>();
            _mockRepositoryService= new Mock<IRepositoryService>();
            _mockChatService= new Mock<IChatService>();
            _mockCacheService= new Mock<ICacheService>();

            var options = new DbContextOptionsBuilder<OpternDbContext>()
                .UseInMemoryDatabase(databaseName: $"OpternTestDB_{Guid.NewGuid()}")
                .ConfigureWarnings(warnings =>
                    warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .EnableSensitiveDataLogging()
                .Options;

            _context = new OpternDbContext(options);
            LoadData();
            _roomService = new RoomService(_mockUnitOfWork.Object, _context, _mockMapper.Object, _mockUserService.Object, _mockCloudinaryService.Object,
                _mockRoomPositionService.Object, _mockRoomTrackService.Object, _mockSkillService.Object, _mockRoomSkillService.Object, _mockRepositoryService.Object,
                _mockChatService.Object, _mockCacheService.Object);
        }
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region GetAllRoomsTests
        [Test]
        [Category("GetAllRoomsTests")]
        public async Task GetALlRooms_ShouldReturnCachedData_WhenCacheExists()
        {
          
            _mockCacheService.Setup(c => c.GetData<IEnumerable<ResponseRoomDTO>>("Rooms"))
                     .Returns(_cachedRooms);

            var result = await _roomService.GetAllAsync();
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.EqualTo(_cachedRooms));
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Message, Does.Contain("Cached Data Retrieved Successfully"));
            _mockUnitOfWork.Verify(u => u.Rooms.GetAllAsync(), Times.Never);
            _mockCacheService.Verify(c => c.GetData<IEnumerable<ResponseRoomDTO>>("Rooms"), Times.Once);
        }

        [Test]
        [Category("GetAllRoomsTests")]
        public async Task GetALlRooms_ShouldReturn404RoomsNotFound_WhenRoomsNotExist()
        {
            _mockCacheService.Setup(c => c.GetData<IEnumerable<ResponseRoomDTO>>("Rooms"))
               .Returns((IEnumerable<ResponseRoomDTO>)null);

            _mockUnitOfWork.Setup(uow => uow.Rooms.GetAllAsync())
                 .ReturnsAsync(new List<Room>());


            var result = await _roomService.GetAllAsync();
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Message, Does.Contain("No Rooms Found"));
            Assert.That(result.Data, Is.Null);
            _mockUnitOfWork.Verify(u => u.Rooms.GetAllAsync(), Times.Once);
        }
        [Test]
        [Category("GetAllRoomsTests")]
        public async Task GetALlRooms_ShouldReturn200Success_WhenRoomsExist()
        {

            // Arrange:
            _mockCacheService.Setup(c => c.GetData<IEnumerable<ResponseRoomDTO>>("Rooms"))
               .Returns((IEnumerable<ResponseRoomDTO>)null);

            _mockUnitOfWork.Setup(uow => uow.Rooms.GetAllAsync())
                 .ReturnsAsync(_rooms);
            _mockMapper.Setup(m => m.Map<IEnumerable<ResponseRoomDTO>>(It.IsAny<IEnumerable<Room>>()))
                .Returns((IEnumerable<Room> sourceRooms) =>
                {
                    return sourceRooms.Select(r => new ResponseRoomDTO
                    {
                        Id = r.Id,
                        Name = r.Name,
                    });
                });
            // Act:
            var result = await _roomService.GetAllAsync();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Data, Has.Exactly(6).Items);
            Assert.That(result.Data.First().Name, Does.Match(_rooms.First().Name));
            _mockUnitOfWork.Verify(u => u.Rooms.GetAllAsync(), Times.Once);
        }
        [Test]
        [Category("GetAllRoomsTests")]
        public async Task GetAllAsync_ShouldReturn500_OnException()
        {
            _mockCacheService.Setup(c => c.GetData<IEnumerable<ResponseRoomDTO>>("Rooms"))
                .Returns((IEnumerable<ResponseRoomDTO>)null);

            _mockUnitOfWork.Setup(u => u.Rooms.GetAllAsync())
                .ThrowsAsync(new Exception("DB down"));

            var result = await _roomService.GetAllAsync();

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(result.Message, Does.Contain("DB down"));
        }
        #endregion

        #region GetPopularRooms
        [Test]
        [Category("GetPopularRoomsTests")]
        public async Task GetPopularRooms_ShouldReturnCachedData_WhenDataCached()
        {
            _mockCacheService.Setup(c => c.GetData<IEnumerable<ResponseRoomDTO>>("PopulerRooms"))
                      .Returns(_cachedRooms);

            var result = await _roomService.GetPopularRooms();
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.EqualTo(_cachedRooms));
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Message, Does.Contain("Cached Data Retrieved Successfully"));
            _mockUnitOfWork.Verify(u => u.Rooms.GetAllAsync(), Times.Never);
            _mockCacheService.Verify(c => c.GetData<IEnumerable<ResponseRoomDTO>>("PopulerRooms"), Times.Once);
        }

        [Test]
        [Category("GetPopularRoomsTests")]
        public async Task GetPopularRooms_ShouldReturn200Success_WhenRoomsExist()
        {
            _mockCacheService.Setup(c => c.GetData<IEnumerable<ResponseRoomDTO>>("PopulerRooms"))
               .Returns((IEnumerable<ResponseRoomDTO>)null);
            var result = await _roomService.GetPopularRooms();
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Data, Has.Exactly(4).Items);
        }

        [Test]
        [Category("GetPopularRoomsTests")]
        public async Task GetPopularRooms_ShouldReturnEnptyList_WhenRoomsNotExist()
        {
            _mockCacheService.Setup(c => c.GetData<IEnumerable<ResponseRoomDTO>>("PopulerRooms"))
                 .Returns((IEnumerable<ResponseRoomDTO>)null);

            _context.RemoveRange(_rooms);
            _context.RemoveRange(_roomUsers);
            _context.SaveChanges();

            var result = await _roomService.GetPopularRooms();
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
            Assert.That(result.Data, Has.Exactly(0).Items);
            Assert.That(result.Data, Is.InstanceOf<IEnumerable<ResponseRoomDTO>>());
            Assert.That(result.Message, Does.Contain("There Are No Created Rooms Until Now"));
        }

        #endregion 

        #region Get Created Rooms
        [Test]
        [Category("GetCreatedRooms")]
        [TestCase("")]
        public async Task GetCreatedRooms_ShouldReturn400_WhenIdIsInvalid(string id)
        {
            var result = await _roomService.GetCreatedRooms(id);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Data, Is.InstanceOf<IEnumerable<ResponseRoomDTO>>());
            Assert.That(result.Data, Is.Empty);
            Assert.That(result.Message, Does.Contain("Invalid CreatorId"));
        }

        [Test]
        [Category("GetCreatedRooms")]
        [TestCase("user1")]
        [TestCase("user2")]
        public async Task GetCreatedRooms_ShouldReturn200Success_WhenIdIsValid(string id)
        {
            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(It.IsAny<string>()))
              .ReturnsAsync((string id) =>
              {
                  var user = _users.FirstOrDefault(x => x.Id == id);
                  return user!;
              });
            var result = await _roomService.GetCreatedRooms(id);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Data, Is.Not.Empty);
            Assert.That(result.Message, Does.Contain("Created Rooms Fetched Successfully"));
        }

        [Test]
        [Category("GetCreatedRooms")]
        [TestCase("user5")]
        [TestCase("user6")]
        public async Task GetCreatedRooms_ShouldReturnEmptyList_WhenUserDoesNotCreateRooms(string id)
        {
            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(It.IsAny<string>()))
              .ReturnsAsync((string id) =>
              {
                  var user = _users.FirstOrDefault(x => x.Id == id);
                  return user!;
              });
            var result = await _roomService.GetCreatedRooms(id);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
            Assert.That(result.Data, Is.Empty);
            Assert.That(result.Data, Has.Exactly(0).Items);
            Assert.That(result.Message, Does.Contain("There Are no Created Rooms Until Now"));
        }


        [Test]
        [Category("GetCreatedRooms")]
        [TestCase("user10")]
        [TestCase("user12")]
        public async Task GetCreatedRooms_ShouldReturnUserNotFound_WhenUserDoesNotExist(string id)
        {
            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(It.IsAny<string>()))
                 .ReturnsAsync((string id) =>
                 {
                     var user = _users.FirstOrDefault(x => x.Id == id);
                     return user!;
                 });

            var result = await _roomService.GetCreatedRooms(id);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Data, Is.Empty);
            Assert.That(result.Data, Has.Exactly(0).Items);
            Assert.That(result.Message, Does.Contain("User Not Found"));
        }

        [Test]
        [Category("GetCreatedRooms")]
        [TestCase("user1")]
        public async Task GetCreatedRooms_ShouldReturn500_WhenExceptionThrown(string id)
        {

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(id))
                .ThrowsAsync(new Exception("Database error"));
     
            var result = await _roomService.GetCreatedRooms(id);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(result.Message, Does.Contain("There is a server error"));
            Assert.That(result.Message, Does.Contain("Database error"));
        }

        #endregion

        #region  GetJoinedRoomsTests
       
        [Test]
        [Category("GetJoinedRoomsTests")]
        [TestCase("")]
        public async Task GetJoinedRooms_ShouldReturn400Failed_WhenIdIsInValid(string userId)
        {
            var result = await _roomService.GetJoinedRooms(userId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Data, Is.Empty);
            Assert.That(result.Message, Does.Contain("Invalid UserId"));
        }
        [Test]
        [Category("GetJoinedRoomsTests")]
        [TestCase("user12")]
        public async Task GetJoinedRooms_ShouldReturn404UserNotFound_WhenUserNotExist(string userId)
        {
            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync((string id) =>
                    {
                        var user = _users.FirstOrDefault(x => x.Id == id);
                        return user!;
                    });

            var result = await _roomService.GetJoinedRooms(userId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Data, Is.Empty);
            Assert.That(result.Data, Has.Exactly(0).Items);
            Assert.That(result.Message, Does.Contain("User Not Found"));
        }
        [Test]
        [Category("GetJoinedRoomsTests")]
        [TestCase("user1")]
        public async Task GetJoinedRooms_ShouldReturn200Success_WhenUserJoinedToRooms(string userId)
        {
            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(It.IsAny<string>()))
                               .ReturnsAsync((string id) =>
                               {
                                   var user = _users.FirstOrDefault(x => x.Id == id);
                                   return user!;
                               });

            var result = await _roomService.GetJoinedRooms(userId);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Data, Has.Exactly(5).Items);
            Assert.That(result.Data.Any(r => r.Name == "C#"), Is.True);
        }

        [Test]
        [Category("GetJoinedRoomsTests")]
        [TestCase("user6")]
        public async Task GetJoinedRooms_ShouldReturnEmptyList_WhenUserNotJoinedToRooms(string userId)
        {
            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(It.IsAny<string>()))
                               .ReturnsAsync((string id) =>
                               {
                                   var user = _users.FirstOrDefault(x => x.Id == id);
                                   return user!;
                               });

            var result = await _roomService.GetJoinedRooms(userId);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
            Assert.That(result.Data, Has.Exactly(0).Items);
            Assert.That(result.Data, Is.Empty);
            Assert.That(result.Message, Does.Contain("You have not joined any room yet."));
        }


        [Test]
        [Category("GetJoinedRoomsTests")]
        [TestCase("user6")]
        public async Task GetJoinedRooms_ShouldThrowException_WhenExceptionThrown(string userId)
        {
            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(It.IsAny<string>()))
                             .ThrowsAsync(new Exception("Connection Failed To DB"));

            var result = await _roomService.GetJoinedRooms(userId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Does.Contain("There is a server error. Please try again later."));
        }
        #endregion

        #region JoinToRoomTests
        [Test]
        [Category("JoinToRoomTests")]
        public async Task JoinToRoom_ShouldReturn400Failed_WhenDataIsInvalid()
        {
            var model = new JoinRoomDTO
            {
                RoomId = "",
                UserId = ""
            };
            var result = await _roomService.JoinToRoom(model);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Message, Does.Contain("Invalid Data"));
        }

        [Test]
        [Category("JoinToRoomTests")]
        public async Task JoinToRoom_ShouldReturn404NotFound_WhenUserOrRoomNotExist()
        {
            var model = new JoinRoomDTO
            {
                RoomId = "room15",
                UserId = "user1"
            };
            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(It.IsAny<string>()))
                   .ReturnsAsync((string id) =>
                   {
                       var user = _users.FirstOrDefault(x => x.Id == id);
                       return user!;
                   });
            _mockUnitOfWork.Setup(uow => uow.Rooms.GetByIdAsync(It.IsAny<string>()))
                   .ReturnsAsync((string id) =>
                   {
                       var room = _rooms.FirstOrDefault(x => x.Id == id);
                       return room!;
                   });
            var result = await _roomService.JoinToRoom(model);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Message, Does.Contain("Either Room or User is not found!"));
            _mockUnitOfWork.Verify(u => u.Rooms.GetByIdAsync(model.RoomId), Times.Exactly(1));
            _mockUnitOfWork.Verify(u => u.Users.GetByIdAsync(model.UserId), Times.Exactly(1));
        }

        [Test]
        [Category("JoinToRoomTests")]
        public async Task JoinToRoom_ShouldReturn400AlreadyJoined_WhenUserAlreadyJoinedToRoom()
        {
            var model = new JoinRoomDTO
            {
                RoomId = "room6",
                UserId = "user1"
            };
            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(It.IsAny<string>()))
                   .ReturnsAsync((string id) =>
                   {
                       var user = _users.FirstOrDefault(x => x.Id == id);
                       return user!;
                   });
            _mockUnitOfWork.Setup(uow => uow.Rooms.GetByIdAsync(It.IsAny<string>()))
                   .ReturnsAsync((string id) =>
                   {
                       var room = _rooms.FirstOrDefault(x => x.Id == id);
                       return room!;
                   });
            _mockUnitOfWork.Setup(u => u.UserRoom.GetByExpressionAsync(
                     It.Is<Expression<Func<UserRoom, bool>>>(expr =>
                  expr.Compile().Invoke(new UserRoom { UserId = "user1", RoomId = "room6" }))))
                .ReturnsAsync(new UserRoom
                {
                    Id = 1,
                    UserId = "user1",
                    RoomId = "room6",
                    IsAccepted = false,
                    IsAdmin = false
                });

            var result = await _roomService.JoinToRoom(model);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Message, Does.Contain("You have already requested to join this room!"));
            Assert.That(_roomUsers.Any(u => u.UserId == model.UserId && u.RoomId == model.RoomId && !u.IsAccepted), Is.True);

        }

        [Test]
        [Category("JoinToRoomTests")]
        public async Task JoinToRoom_ShouldReturn201JoinedSuccessfully_WhenUserRequestToJoinToRoom()
        {
            var model = new JoinRoomDTO
            {
                RoomId = "room6",
                UserId = "user6"
            };
            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(It.IsAny<string>()))
                   .ReturnsAsync((string id) =>
                   {
                       var user = _users.FirstOrDefault(x => x.Id == id);
                       return user!;
                   });
            _mockUnitOfWork.Setup(uow => uow.Rooms.GetByIdAsync(It.IsAny<string>()))
                   .ReturnsAsync((string id) =>
                   {
                       var room = _rooms.FirstOrDefault(x => x.Id == id);
                       return room!;
                   });
            _mockUnitOfWork.Setup(u => u.UserRoom.GetByExpressionAsync(
                  It.IsAny<Expression<Func<UserRoom, bool>>>())).ReturnsAsync((UserRoom)null!);

            var result = await _roomService.JoinToRoom(model);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
            Assert.That(result.Message, Does.Contain("Waiting for approval"));
            Assert.That(!_roomUsers.Any(u => u.UserId == model.UserId && u.RoomId == model.RoomId), Is.True);
        }

        [Test]
        [Category("JoinToRoomTests")]
        public async Task JoinToRoom_ShouldThrowException_WhenExceptionThrown()
        {
            var model = new JoinRoomDTO
            {
                RoomId = "room6",
                UserId = "user6"
            };
            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(It.IsAny<string>()))
                   .ThrowsAsync(new Exception("Connection Failed To Db"));

            var result = await _roomService.JoinToRoom(model);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(result.Message, Does.Contain("There is a server error. Please try again later"));
        }
        #endregion

        #region GetRoomByIdTests
        [Test]
        [Category("GetRoomByIdTests")]
        [TestCase("", "user1")]
        public async Task GetRoomById_ShouldReturn400Failed_WhenIdIsInValid(string roomId, string userId)
        {
            var result = await _roomService.GetRoomById(roomId, userId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Data, Is.InstanceOf<ResponseRoomDTO>());
            Assert.That(result.Message, Does.Contain("Invalid Room Id"));
        }

        [Test]
        [Category("GetRoomByIdTests")]
        [TestCase("room1", "user1")]
        public async Task GetRoomById_ShouldReturn200Success_WhenRoomExist(string roomId, string userId)
        {
            var result = await _roomService.GetRoomById(roomId, userId);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Data.Name, Is.EqualTo("C#"));
            Assert.That(result.Data.Members, Is.EqualTo(5));
            Assert.That(result.Data.Skills.Count, Is.EqualTo(2));
        }
        [Test]
        [Category("GetRoomByIdTests")]
        [TestCase("room105", "user1")]
        public async Task GetRoomById_ShouldReturn404NotFound_WhenRoomNotExist(string roomId, string userId)
        {
            var result = await _roomService.GetRoomById(roomId, userId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Data, Is.InstanceOf<ResponseRoomDTO>());
            Assert.That(result.Message, Does.Contain("This Room Not Found"));
        }
        #endregion
        
        #region GetRoomsByTrackTests
        [Test]
        [Category("GetRoomsByTrackTests")]
        [TestCase(0)]
        public async Task GetRoomsByTrack_ShouldReturn400Failed_WhenIdIsInValid(int trackId)
        {

            var result = await _roomService.GetRoomsByTrack(trackId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Message, Does.Contain("Invalid Room Id"));
            Assert.That(result.Data, Is.InstanceOf<IEnumerable<ResponseRoomDTO>>());
        }

        [Test]
        [Category("GetRoomsByTrackTests")]
        [TestCase(20)]
        public async Task GetRoomsByTrack_ShouldReturn404NotFound_WhenTrackNotExist(int trackId)
        {
            _mockUnitOfWork.Setup(uow => uow.Tracks.GetByIdAsync(It.IsAny<int>()))
                 .ReturnsAsync((int id) =>
                 {
                     var track = _tracks.FirstOrDefault(x => x.Id == id);
                     return track!;
                 });
            var result = await _roomService.GetRoomsByTrack(trackId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Message, Does.Contain("Track Not Found"));
            Assert.That(result.Data, Is.InstanceOf<IEnumerable<ResponseRoomDTO>>());
        }

        [Test]
        [Category("GetRoomsByTrackTests")]
        [TestCase(1)]
        public async Task GetRoomsByTrack_ShouldReturn200Success_WhenTrackExist(int trackId)
        {
            _mockUnitOfWork.Setup(uow => uow.Tracks.GetByIdAsync(It.IsAny<int>()))
                 .ReturnsAsync((int id) =>
                 {
                     var track = _tracks.FirstOrDefault(x => x.Id == id);
                     return track!;
                 });
            var result = await _roomService.GetRoomsByTrack(trackId);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Message, Does.Contain("Rooms retrieved successfully."));
            Assert.That(result.Data, Has.Exactly(2).Items);
        }

        [Test]
        [Category("GetRoomsByTrackTests")]
        [TestCase(3)]
        public async Task GetRoomsByTrack_ShouldReturn404Failed_WhenTrackDoesNotContainsRooms(int trackId)
        {
            _mockUnitOfWork.Setup(uow => uow.Tracks.GetByIdAsync(It.IsAny<int>()))
                 .ReturnsAsync((int id) =>
                 {
                     var track = _tracks.FirstOrDefault(x => x.Id == id);
                     return track!;
                 });
            var result = await _roomService.GetRoomsByTrack(trackId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Message, Does.Contain("No rooms found for the given track."));
            Assert.That(result.Data, Has.Exactly(0).Items);
        }

        [Test]
        [Category("GetRoomsByTrackTests")]
        [TestCase(3)]
        public async Task GetRoomsByTrack_ShouldThrowException_WhenExceptionThrown(int trackId)
        {
            _mockUnitOfWork.Setup(uow => uow.Tracks.GetByIdAsync(It.IsAny<int>()))
                 .ThrowsAsync(new Exception("Failed Connection To Db"));
            var result = await _roomService.GetRoomsByTrack(trackId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(result.Data, Is.InstanceOf<IEnumerable<ResponseRoomDTO>>());
            Assert.That(result.Message, Does.Contain("Server error"));

        }
        #endregion

        #region CreateRoomTests
        [Test]
        [Category("CreateRoomTests")]
        public async Task CreateRoom_ShouldReturn400InvalidModel_WhenModelIsNull()
        {
            CreateRoomDTO room = null;
            var result = await _roomService.CreateRoom(room);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Does.Contain("Invalid Data Model"));
        }

        [Test]
        [Category("CreateRoomTests")]
        public async Task CreateRoom_ShouldReturn400InvalidData_WhenCreatorIdIsNull()
        {

            CreateRoomDTO room = new CreateRoomDTO()
            {
                Name = "room7",
                Description = "New Room",
                RoomType = RoomType.Private,
                CreatorId = null,
            };

            _mockChatService.Setup(c => c.CreateRoomChatAsync(It.IsAny<string>(), It.IsAny<ChatType>()))
                      .ReturnsAsync(Response<ChatDTO>.Failure(new ChatDTO(), "Invalid creator Id.", 400));

            _mockUnitOfWork.Setup(u => u.SaveAsync())
                  .Returns(Task.FromResult(1));

            var result = await _roomService.CreateRoom(room);


            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Does.Contain("Invalid creator Id."));
        }

        [Test]
        [Category("CreateRoomTests")]
        public async Task CreateRoom_ShouldReturn400InvalidData_WhenModelIsNotValid()
        {

            CreateRoomDTO room = new CreateRoomDTO()
            {
                Name = "room7",
                Description = "",
                RoomType = RoomType.Private,
                CreatorId = "user1",
            };

            _mockChatService.Setup(c => c.CreateRoomChatAsync(It.IsAny<string>(), It.IsAny<ChatType>()))
             .ReturnsAsync(Response<ChatDTO>.Success(new ChatDTO { Id = 1, Type = ChatType.Group }, "Chat Created Successfully", 200));
            _mockUnitOfWork.Setup(u => u.SaveAsync())
                  .Returns(Task.FromResult(1));

            var result = await _roomService.CreateRoom(room);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Data, Is.InstanceOf<ResponseRoomDTO>());
            Assert.That(result.Message, Does.Contain("Room description is required."));

        }

        [Test]
        [Category("CreateRoomTests")]
        public async Task CreateRoom_ShouldReturn200Success_WhenModelIsValid()
        {
            CreateRoomDTO room = new CreateRoomDTO()
            {
                Name = "room7",
                Description = "New Room",
                RoomType = RoomType.Public,
                CreatorId = "user1",
                Tracks = _tracks.Select(t => t.Id).ToList(),
                Skills = _roomSkills.Select(s => new Application.DTOs.Skills.SkillDTO { Id = s.SkillId, Name = s.Skill.Name }).ToList()
            };

            _mockChatService.Setup(c => c.CreateRoomChatAsync(It.IsAny<string>(), It.IsAny<ChatType>()))
         .ReturnsAsync(Response<ChatDTO>.Success(new ChatDTO { Id = 1, Type = ChatType.Group }, "Chat Created Successfully", 200));

            _mockUnitOfWork.Setup(uow => uow.Rooms.AddAsync(It.IsAny<Room>()))
            .ReturnsAsync((Room room) =>
            {
                room.Id = "room7";
                room.Name = "room7";
                room.CreatorId = "user1";
                return room;
            });

            _mockUnitOfWork.Setup(uow => uow.UserRoom.AddAsync(It.IsAny<UserRoom>()))
              .ReturnsAsync((UserRoom room) =>
              {
                  room.UserId = "user1";
                  room.RoomId = "room7";
                  room.IsAdmin = true;
                  room.IsAccepted = true;
                  return room;
              });
            MapCreateRoom();
            var result = await _roomService.CreateRoom(room);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
            Assert.That(result.Data.Name, Is.SameAs("room7"));
            Assert.That(result.Data.CreatorId, Is.EqualTo("user1"));
            Assert.That(result.Data.Description, Is.EqualTo(room.Description));
            Assert.That(result.Message, Does.Contain("Room Added Successfully"));
        }
        [Test]
        [Category("CreateRoomTests")]
        public async Task CreateRoom_ShouldReturn500_WhenExceptionOccurs()
        {
            CreateRoomDTO room = new CreateRoomDTO()
            {
                Name = "room7",
                Description = "New Room",
                RoomType = RoomType.Private,
                CreatorId = "user123",
            };

            _mockChatService.Setup(c => c.CreateRoomChatAsync(It.IsAny<string>(), It.IsAny<ChatType>()))
                .ThrowsAsync(new Exception("Database connection failed"));

            _mockUnitOfWork.Setup(u => u.SaveAsync())
                .ThrowsAsync(new Exception("Unexpected database error"));

            var result = await _roomService.CreateRoom(room);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(result.Message, Does.Contain("There is a server error"));
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
    new UserRoom { Id = 1, UserId = "user1", RoomId = "room1", IsAccepted = true, IsAdmin = true },
    new UserRoom { Id = 2, UserId = "user2", RoomId = "room1", IsAccepted = true, IsAdmin = false },
    new UserRoom { Id = 3, UserId = "user3", RoomId = "room1", IsAccepted = true, IsAdmin = false },
    new UserRoom { Id = 4, UserId = "user4", RoomId = "room1", IsAccepted = true, IsAdmin = false },
    new UserRoom { Id = 5, UserId = "user5", RoomId = "room1", IsAccepted = true, IsAdmin = true },

    // room2 users
    new UserRoom { Id = 6, UserId = "user1", RoomId = "room2", IsAccepted = true, IsAdmin = false },
    new UserRoom { Id = 7, UserId = "user2", RoomId = "room2", IsAccepted = true, IsAdmin = true },
    new UserRoom { Id = 8, UserId = "user3", RoomId = "room2", IsAccepted = true, IsAdmin = false },
    new UserRoom { Id = 9, UserId = "user4", RoomId = "room2", IsAccepted = true, IsAdmin = false },
    new UserRoom { Id = 10, UserId = "user5", RoomId = "room2", IsAccepted = true, IsAdmin = false },

    // room3 users
    new UserRoom { Id = 11, UserId = "user1", RoomId = "room3", IsAccepted = true, IsAdmin = true },
    new UserRoom { Id = 12, UserId = "user2", RoomId = "room3", IsAccepted = true, IsAdmin = false },
    new UserRoom { Id = 13, UserId = "user3", RoomId = "room3", IsAccepted = true, IsAdmin = false },
    new UserRoom { Id = 14, UserId = "user4", RoomId = "room3", IsAccepted = true, IsAdmin = false },
    new UserRoom { Id = 15, UserId = "user5", RoomId = "room3", IsAccepted = true, IsAdmin = false },

    // room4 users
    new UserRoom { Id = 16, UserId = "user1", RoomId = "room4", IsAccepted = true, IsAdmin = false },
    new UserRoom { Id = 17, UserId = "user2", RoomId = "room4", IsAccepted = true, IsAdmin = true },
    new UserRoom { Id = 18, UserId = "user3", RoomId = "room4", IsAccepted = true, IsAdmin = false },
    new UserRoom { Id = 19, UserId = "user4", RoomId = "room4", IsAccepted = true, IsAdmin = false },
    new UserRoom { Id = 20, UserId = "user5", RoomId = "room4", IsAccepted = false, IsAdmin = false },

    // room5 users
    new UserRoom { Id = 21, UserId = "user1", RoomId = "room5", IsAccepted = true, IsAdmin = true },
    new UserRoom { Id = 22, UserId = "user2", RoomId = "room5", IsAccepted = true, IsAdmin = false },
    new UserRoom { Id = 25, UserId = "user5", RoomId = "room5", IsAccepted = false, IsAdmin = false },

    // room6 users
    new UserRoom { Id = 26, UserId = "user1", RoomId = "room6", IsAccepted = false, IsAdmin = false },
    new UserRoom { Id = 27, UserId = "user2", RoomId = "room6", IsAccepted = true, IsAdmin = true },
    new UserRoom { Id = 28, UserId = "user3", RoomId = "room6", IsAccepted = true, IsAdmin = false },

};

            _skills = new List<Skills>
            {
                new Skills { Id = 1, Name = "C#" },
                new Skills { Id = 2, Name = "SQL" },
                new Skills { Id = 3, Name = "Azure" },
            };
            _roomSkills = new List<RoomSkills>
            {
                new RoomSkills { Id = 1, RoomId = "room1", SkillId = 1 },
                new RoomSkills { Id = 2, RoomId = "room1", SkillId = 2 },
                 new RoomSkills { Id = 3, RoomId = "room2", SkillId = 3 }
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
            _tracks = new List<Track>
            {
                new Track {Id=1,Name="DotNet"},
                new Track {Id=2,Name="NodeJs"},
                new Track {Id=3,Name="Angular"},
            };
            _roomTracks = new List<RoomTrack>
            {
                new RoomTrack {Id=1, RoomId="room1", TrackId=1},
                new RoomTrack {Id=2, RoomId="room3", TrackId=1},
                new RoomTrack {Id=3, RoomId="room2", TrackId=2},
            };
            _context.Rooms.AddRange(_rooms);
            _context.Users.AddRange(_users);
            _context.UserRooms.AddRange(_roomUsers);
            _context.Skills.AddRange(_skills);
            _context.RoomSkills.AddRange(_roomSkills);
            _context.Tracks.AddRange(_tracks);
            _context.RoomTracks.AddRange(_roomTracks);
            _context.SaveChanges();
        }
        #endregion

        #region Mapping
        private void MapCreateRoom()
        {
            _mockMapper.Setup(m => m.Map<ResponseRoomDTO>(It.IsAny<Room>()))
                .Returns((Room src) => new ResponseRoomDTO
                {
                    Id = src.Id,
                    Name = src.Name,
                    Description = src.Description,
                    RoomType = src.RoomType,
                    CreatorId = src.CreatorId,
                    CoverPicture = src.CoverPicture,
                    CreatedAt = src.CreatedAt,
                    chatId = src.ChatId
                });
        } 
        #endregion
    }
}

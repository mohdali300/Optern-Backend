using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using Optern.Application.Interfaces.IRoomTrackService;
using Optern.Application.Interfaces.ITrackService;
using Optern.Domain.Entities;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Services.RoomTracksService;
using Optern.Infrastructure.Services.TrackService;
using Optern.Infrastructure.UnitOfWork;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

namespace Optern.Test.RoomTest.RoomTrackTests
{
    [TestFixture]
    public class RoomTrackTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private OpternDbContext _context;
        private IRoomTrackService _roomTrackService;
        private List<Track> _tracks;
        private List<RoomTrack> _roomTracks;
        private List<Room> _rooms;
        private List<ApplicationUser> _users;
        private List<UserRoom> _roomUsers;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<OpternDbContext>()
                .UseInMemoryDatabase(databaseName: $"OpternTestDB_{Guid.NewGuid()}")
                .ConfigureWarnings(warnings =>
                    warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .EnableSensitiveDataLogging()
                .Options;

            _context = new OpternDbContext(options);
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            CreateSampleData();
            UOWSetup();

            _roomTrackService = new RoomTrackService(
                _mockUnitOfWork.Object
                );
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region AddRoomTrack

        [Test]
        [Category("AddRoomTrack")]
        public async Task AddRoomTrack_WithValidData_ReturnsSuccess()
        {
            var data = new List<int> { 3 };
            var result = await _roomTrackService.AddRoomTrack("room2", data);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
                Assert.That(result.Message, Does.Contain("Successfully"));
            });
        }

        [Test]
        [Category("AddRoomTrack")]
        public async Task AddRoomTrack_WithInValidData_ReturnsBadRequest()
        {
            var data = new List<int>();
            var result = await _roomTrackService.AddRoomTrack("room2", data);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("Invalid"));
            });
        }

        [Test]
        [Category("AddRoomTrack")]
        public async Task AddRoomTrack_WithNonExistentRoom_ReturnsNotFound()
        {
            var data = new List<int> { 3 };
            var result = await _roomTrackService.AddRoomTrack("room3", data);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("not found"));
            });
        }

        #endregion AddRoomTrack

        #region DeleteRoomTrack

        [Test]
        [Category("DeleteRoomTrack")]
        [TestCase("room1", 1)]
        [TestCase("room1", 3)]
        public async Task DeleteRoomTrack_WithValidData_ShouldDeleteRoomTrack(string roomId, int trackId)
        {
            var result = await _roomTrackService.DeleteRoomTrack(roomId, trackId);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Does.Contain("Deleted"));
            });
        }

        [Test]
        [Category("DeleteRoomTrack")]
        [TestCase("", 1)]
        [TestCase("room1", 0)]
        public async Task DeleteRoomTrack_WithInValidData_ReturnsBadRequest(string roomId, int trackId)
        {
            var result = await _roomTrackService.DeleteRoomTrack(roomId, trackId);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("Invalid"));
            });
        }

        [Test]
        [Category("DeleteRoomTrack")]
        [TestCase("room3", 1)]
        [TestCase("room1", 5)]
        public async Task DeleteRoomTrack_WithNonExistentRoomOrTrack_ReturnsNotFound(string roomId, int trackId)
        {
            var result = await _roomTrackService.DeleteRoomTrack(roomId, trackId);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("Not Found"));
            });
        }

        #endregion DeleteRoomTrack

        #region Helpers

        private void CreateSampleData()
        {
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
                    }
            };

            _rooms = new List<Room>
            {
                new Room
                {
                    Id="room1",
                    Name="Room 1",
                    Description="Description for room 1",
                    RoomType=RoomType.Public,
                    CreatedAt=DateTime.Now.AddMinutes(-10),
                    CreatorId="user1"
                },
                new Room
                {
                    Id="room2",
                    Name="Room 2",
                    Description="Description for room 2",
                    RoomType=RoomType.Public,
                    CreatedAt=DateTime.Now.AddMinutes(-15),
                    CreatorId="user1"
                },
            };

            _roomUsers = new List<UserRoom>
            {
                new UserRoom
                {
                    Id=1,
                    UserId="user1",
                    RoomId="room1",
                    IsAccepted=true,
                    IsAdmin=true,
                },
                new UserRoom
                {
                    Id=2,
                    UserId="user2",
                    RoomId="room1",
                    IsAccepted=true,
                    IsAdmin=false,
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
                new RoomTrack {Id=2, RoomId="room1", TrackId=3},
                new RoomTrack {Id=3, RoomId="room2", TrackId=2},
            };

            _context.Tracks.AddRange(_tracks);
            _context.Users.AddRange(_users);
            _context.Rooms.AddRange(_rooms);
            _context.RoomTracks.AddRange(_roomTracks);
            _context.UserRooms.AddRange(_roomUsers);
            _context.SaveChanges();
        }

        private void UOWSetup()
        {
            _mockUnitOfWork.Setup(uow => uow.Tracks.GetAllAsync())
                .ReturnsAsync(_context.Tracks);

            _mockUnitOfWork.Setup(uow => uow.Tracks.AddAsync(It.IsAny<Track>()))
                .ReturnsAsync((Track entity) =>
                {
                    entity.Id = 4;
                    return entity;
                });

            _mockUnitOfWork.Setup(uow => uow.RoomTracks.AddRangeAsync(It.IsAny<List<RoomTrack>>()))
                .Returns((List<RoomTrack> entities) =>
                {
                    var id = 4;
                    foreach (var entity in entities)
                    {
                        entity.Id = id++;
                    }
                    return Task.CompletedTask;
                });

            _mockUnitOfWork.Setup(uow => uow.RoomTracks.DeleteAsync(It.IsAny<RoomTrack>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(uow => uow.Rooms.AnyAsync(It.IsAny<Expression<Func<Room, bool>>>()))
                .ReturnsAsync((Expression<Func<Room, bool>> expr) =>
                {
                    var func = expr.Compile();
                    return _rooms.Any(room => func(room));
                });

            _mockUnitOfWork.Setup(uow => uow.RoomTracks.GetByExpressionAsync(It.IsAny<Expression<Func<RoomTrack, bool>>>()))!
                .ReturnsAsync((Expression<Func<RoomTrack, bool>> expr) =>
                {
                    var func = expr.Compile();
                    return _roomTracks.FirstOrDefault(rt => func(rt));
                });
        }

        #endregion Helpers
    }
}
#region usings

using HotChocolate.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using Optern.Application.DTOs.Room;
using Optern.Application.DTOs.Skills;
using Optern.Application.Interfaces.IChatService;
using Optern.Application.Interfaces.IRoomSettingService;
using Optern.Application.Interfaces.IRoomSkillService;
using Optern.Application.Interfaces.IRoomTrackService;
using Optern.Application.Interfaces.ISkillService;
using Optern.Domain.Entities;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.ExternalInterfaces.IFileService;
using Optern.Infrastructure.ExternalServices.FileService;
using Optern.Infrastructure.Repositories;
using Optern.Infrastructure.Services.RoomSettings;
using Optern.Infrastructure.UnitOfWork;
using Task = System.Threading.Tasks.Task;

#endregion

namespace Optern.Test.RoomTest.RoomSettingsTests
{
    [TestFixture]
    public class RoomSettingsTests
    {
        #region Mocks

        private Mock<IUnitOfWork> _mockUnitOfWork;
        private OpternDbContext _context;
        private IRoomSettingService _roomSettingService;
        private Mock<ICloudinaryService> _mockCloudinaryService;
        private Mock<ISkillService> _mockSkillService;
        private Mock<IRoomSkillService> _mockRoomSkillService;
        private Mock<IRoomPositionService> _mockRoomPositionService;
        private Mock<IRoomTrackService> _mockRoomTrackService;
        private Mock<IChatService> _mockChatService;
        private List<Room> _rooms;
        private List<Skills> _skills;
        private List<Position> _positions;
        private List<Track> _tracks;
        private List<RoomTrack> _roomTracks;
        private List<RoomSkills> _roomSkills;
        private List<RoomPosition> _roomPositions;
        private List<ApplicationUser> _users;
        private List<UserRoom> _roomUsers;

        #endregion Mocks

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
            _mockChatService = new Mock<IChatService>();
            _mockCloudinaryService = new Mock<ICloudinaryService>();
            _mockRoomPositionService = new Mock<IRoomPositionService>();
            _mockRoomSkillService = new Mock<IRoomSkillService>();
            _mockRoomTrackService = new Mock<IRoomTrackService>();
            _mockSkillService = new Mock<ISkillService>();

            CreateSampleData();
            UOWSetup();

            _roomSettingService = new RoomSettingService(
                _mockUnitOfWork.Object,
                _context,
                _mockCloudinaryService.Object,
                _mockSkillService.Object,
                _mockRoomSkillService.Object,
                _mockRoomPositionService.Object,
                _mockRoomTrackService.Object,
                _mockChatService.Object
                );
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region EditRoom

        [Test]
        [Category("EditRoom")]
        public async Task EditRoom_WithValidData_ShouldUpdateRoom()
        {
            var model = new EditRoomDTO
            {
                Name = "Updeted name",
                Description = "Updeted description",
                RoomType = RoomType.Private,
                Positions = new List<int> { 1 },
                //Skills=new List<SkillDTO> { new SkillDTO { Id=4,Name="MicroServices"} },
                Tracks=new List<int> { 3}
            };

            var result = await _roomSettingService.EditRoom("room2", model);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Is.EqualTo("Updated Successfully"));
            });
        }

        [Test]
        [Category("EditRoom")]
        public async Task EditRoom_WithNonExistentRoom_ReturnsNotFound()
        {
            var model = new EditRoomDTO
            {
                Name = "Updeted name",
                Description = "Updeted description",
                RoomType = RoomType.Private,
                Positions = new List<int> { 1 }
            };

            var result = await _roomSettingService.EditRoom("room3", model);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Is.EqualTo("Room Not Found!"));
            });
        }

        [Test]
        [Category("EditRoom")]
        public async Task EditRoom_WithInValidData_ReturnsBadRequest()
        {
            var result = await _roomSettingService.EditRoom("", null!);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Is.EqualTo("Invalid Data Model"));
            });
        }

        #endregion

        #region DeleteRoom

        [Test]
        [Category("DeleteRoom")]
        [TestCase("room1")]
        [TestCase("room2")]
        public async Task DeleteRoom_WithValidId_ShouldDeleteRoom(string id)
        {
            var result = await _roomSettingService.DeleteRoom(id);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Is.EqualTo("Room Deleted Successfully"));
            });
        }

        [Test]
        [Category("DeleteRoom")]
        [TestCase("")]
        [TestCase("room3")]
        public async Task DeleteRoom_WithInValidId_ReturnsNotFound(string id)
        {
            var result = await _roomSettingService.DeleteRoom(id);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Is.EqualTo("Room not Found!"));
            });
        }

        #endregion

        #region LeaveRoom

        [Test]
        [Category("LeaveRoom")]
        public async Task LeaveRoom_WithNotAdminMember_ShouldUserLeaveRoom()
        {
            var result = await _roomSettingService.LeaveRoomAsync("room1", "user2");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Is.EqualTo("Successfully left the room"));
            });
        }

        [Test]
        [Category("LeaveRoom")]
        public async Task LeaveRoom_WithLastAdmin_ReturnsBadRequest()
        {
            var result = await _roomSettingService.LeaveRoomAsync("room1", "user1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Is.EqualTo("You must specify a new admin before leaving as the last admin"));
            });
        }

        [Test]
        [Category("LeaveRoom")]
        public async Task LeaveRoom_WithNotMemberUser_ReturnsNotFound()
        {
            var result = await _roomSettingService.LeaveRoomAsync("room1", "user3");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Is.EqualTo("User is not in this room"));
            });
        }

        [Test]
        [Category("LeaveRoom")]
        public async Task LeaveRoom_WithLastMember_ReturnsLeaveAndDeleteRoom()
        {
            await _roomSettingService.LeaveRoomAsync("room1", "user2");
            var result = await _roomSettingService.LeaveRoomAsync("room1", "user1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Is.EqualTo("You left the room and the room was deleted because it became empty"));
            });
        }

        #endregion

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

            _positions = new List<Position>
            {
                new Position { Id=1,Name="Backend Dev"},
                new Position { Id=2,Name="Frontend Dev"},
            };
            _roomPositions = new List<RoomPosition>
            {
                new RoomPosition { Id=1,RoomId="room1",PositionId=1},
                new RoomPosition { Id=2,RoomId="room1",PositionId=2},
            };

            _context.Users.AddRange(_users);
            _context.Rooms.AddRange(_rooms);
            _context.UserRooms.AddRange(_roomUsers);
            _context.Positions.AddRange(_positions);
            _context.RoomPositions.AddRange(_roomPositions);
            _context.Tracks.AddRange(_tracks);
            _context.RoomTracks.AddRange(_roomTracks);
            _context.Skills.AddRange(_skills);
            _context.RoomSkills.AddRange(_roomSkills);
            _context.SaveChanges();
        }

        private void UOWSetup()
        {
            var mockRoomRepository = new Mock<IGenericRepository<Room>>();

            _mockUnitOfWork.Setup(uow => uow.Rooms).Returns(mockRoomRepository.Object);

            mockRoomRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Room>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(uow => uow.Rooms.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) =>
                {
                    var room = _rooms.FirstOrDefault(x => x.Id == id);
                    return room!;
                });

            _mockUnitOfWork.Setup(uow => uow.Skills.AddRangeAsync(It.IsAny<List<Skills>>()))
                .Returns((List<Skills> entities) =>
                {
                    var id = 4;
                    foreach (var entity in entities)
                    {
                        entity.Id = id++;
                    }
                    return Task.CompletedTask;
                });

                _mockUnitOfWork.Setup(uow => uow.RoomSkills.AddRangeAsync(It.IsAny<List<RoomSkills>>()))
                .Returns((List<RoomSkills> entities) =>
                {
                    var id = 4;
                    foreach (var entity in entities)
                    {
                        entity.Id = id++;
                    }
                    return Task.CompletedTask;
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

            _mockUnitOfWork.Setup(uow => uow.Skills.DeleteAsync(It.IsAny<Skills>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(uow => uow.RoomSkills.DeleteAsync(It.IsAny<RoomSkills>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(uow => uow.Tracks.DeleteAsync(It.IsAny<Track>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.RoomTracks.DeleteAsync(It.IsAny<RoomTrack>()))
                .Returns(Task.CompletedTask);
        }

        #endregion Helpers
    }
}
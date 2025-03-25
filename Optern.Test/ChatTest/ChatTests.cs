using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using Optern.Application.Interfaces.IChatService;
using Optern.Domain.Entities;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Services.ChatService;
using Optern.Infrastructure.UnitOfWork;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

namespace Optern.Test.ChatTest
{
    [TestFixture]
    public class ChatTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private OpternDbContext _context;
        private IChatService _chatService;
        private List<Room> _rooms;
        private List<ApplicationUser> _users;
        private List<UserRoom> _roomUsers;
        private List<Chat> _chats;
        private List<ChatParticipants> _chatParticipants;

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

            _chatService = new ChatService(
                _mockUnitOfWork.Object,
                _context
                );
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region CreatePrivateChat

        [Test]
        [Category("CreatePrivateChat")]
        public async Task CreatePrivateChat_WithValidData_ReturnsCreatedChat()
        {
            var result = await _chatService.CreatePrivateChatAsync("user1", "user2");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
                Assert.That(result.Message, Does.Contain("created"));
                Assert.That(result.Data, Is.Not.Null);
            });
        }

        [Test]
        [Category("CreatePrivateChat")]
        [TestCase("", "user2")]
        [TestCase("user1", "")]
        public async Task CreatePrivateChat_WithInValidData_ReturnsBadRequest(string user1, string user2)
        {
            var result = await _chatService.CreatePrivateChatAsync(user1, user2);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("Invalid"));
            });
        }

        [Test]
        [Category("CreatePrivateChat")]
        public async Task CreatePrivateChat_WithNonExistentUser_ReturnsBadRequest()
        {
            var result = await _chatService.CreatePrivateChatAsync("user1", "user4");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("not found"));
            });
        }

        [Test]
        [Category("CreatePrivateChat")]
        public async Task CreatePrivateChat_WithExistedChat_ReturnsBadRequest()
        {
            var result = await _chatService.CreatePrivateChatAsync("user1", "user3");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("Already there is a chat"));
            });
        }

        #endregion CreatePrivateChat

        #region CreateRoomChat

        [Test]
        [Category("CreateRoomChat")]
        public async Task CreateRoomChat_WithValidData_ReturnsCreatedChat()
        {
            var result = await _chatService.CreateRoomChatAsync("user2", ChatType.Group);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
                Assert.That(result.Data.Type, Is.EqualTo(ChatType.Group));
            });
        }

        [Test]
        [Category("CreateRoomChat")]
        public async Task CreateRoomChat_WithCreatorId_ReturnsBadRequest()
        {
            var result = await _chatService.CreateRoomChatAsync("", ChatType.Group);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("Invalid"));
            });
        }

        #endregion CreateRoomChat

        #region JoinToRoomChat

        [Test]
        [Category("JoinToRoomChat")]
        public async Task JoinToRoomChat_WithValidData_ReturnsSuccess()
        {
            var result = await _chatService.JoinToRoomChatAsync("room2", "user3");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
                Assert.That(result.Message, Does.Contain("joined to room"));
            });
        }

        [Test]
        [Category("JoinToRoomChat")]
        [TestCase("room2", "user4")]
        [TestCase("room5", "user3")]
        public async Task JoinToRoomChat_WithNonExistentRoomOrUser_ReturnsNotFound(string roomId, string userId)
        {
            var result = await _chatService.JoinToRoomChatAsync(roomId, userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("not existed"));
            });
        }

        [Test]
        [Category("JoinToRoomChat")]
        [TestCase("", "user3")]
        [TestCase("room2", "")]
        public async Task JoinToRoomChat_WithInValidData_ReturnsBadRequest(string roomId, string userId)
        {
            var result = await _chatService.JoinToRoomChatAsync(roomId, userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("Invalid"));
            });
        }

        #endregion JoinToRoomChat

        #region JoinAllToRoomChat

        [Test]
        [Category("JoinAllToRoomChat")]
        public async Task JoinAllToRoomChat_WithValidData_ReturnsSuccess()
        {
            var result = await _chatService.JoinAllToRoomChatAsync("room2", ["user1", "user3"]);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
                Assert.That(result.Message, Does.Contain("joined to room"));
            });
        }

        [Test]
        [Category("JoinAllToRoomChat")]
        public async Task JoinAllToRoomChat_WithNonExistentRoom_ReturnsNotFound()
        {
            var result = await _chatService.JoinAllToRoomChatAsync("room4", ["user1", "user3"]);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("not existed"));
            });
        }

        [Test]
        [Category("JoinAllToRoomChat")]
        public async Task JoinAllToRoomChat_WithInValidData_ReturnsBadRequest()
        {
            var result = await _chatService.JoinAllToRoomChatAsync("", ["user1", "user3"]);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("Invalid"));
            });
        }

        #endregion

        #region RemoveFromRoomChat

        [Test]
        [Category("RemoveFromRoomChat")]
        public async Task RemoveFromRoomChat_WithValidData_ReturnsSuccess()
        {
            var result = await _chatService.RemoveFromRoomChatAsync(1, "user3");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Does.Contain("User left"));
            });
        }

        [Test]
        [Category("RemoveFromRoomChat")]
        public async Task RemoveFromRoomChat_WithLeftUser_ReturnsBadRequest()
        {
            var result = await _chatService.RemoveFromRoomChatAsync(1, "user2");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("already left"));
            });
        }

        #endregion

        #region GetChatParticipants

        [Test]
        [Category("GetChatParticipants")]
        public async Task GetChatParticipants_WithChatId_ReturnsAllForChat()
        {
            var result = await _chatService.GetChatParticipantsAsync(1);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data.Count, Is.EqualTo(2));
            });
        }

        [Test]
        [Category("GetChatParticipants")]
        public async Task GetChatParticipants_WithUserId_ReturnsAllForUser()
        {
            var result = await _chatService.GetChatParticipantsAsync(null, "user1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data.Count, Is.EqualTo(1));
            });
        }

        [Test]
        [Category("GetChatParticipants")]
        public async Task GetChatParticipants_WithEmptyChat_ReturnsNoContent()
        {
            var result = await _chatService.GetChatParticipantsAsync(null,"user2");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
                Assert.That(result.Data.Count, Is.EqualTo(0));
            });
        }

        [Test]
        [Category("GetChatParticipants")]
        public async Task GetChatParticipants_WithNonExistentUser_ReturnsNotFound()
        {
            var result = await _chatService.GetChatParticipantsAsync(null, "user4");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("not existed"));
                Assert.That(result.Data.Count, Is.EqualTo(0));
            });
        }

        [Test]
        [Category("GetChatParticipants")]
        public async Task GetChatParticipants_WithNonExistentChat_ReturnsNotFound()
        {
            var result = await _chatService.GetChatParticipantsAsync(5);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("not existed"));
                Assert.That(result.Data.Count, Is.EqualTo(0));
            });
        }

        [Test]
        [Category("GetChatParticipants")]
        [TestCase(1,"user1")]
        [TestCase(null,"")]
        [TestCase(0,null)]
        public async Task GetChatParticipants_WithInValidData_ReturnsBadRequest(int chatId, string userId)
        {
            var result = await _chatService.GetChatParticipantsAsync(chatId,userId);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("only one from them"));
                Assert.That(result.Data.Count, Is.EqualTo(0));
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
                    },
                new ApplicationUser
                    {
                        Id = "user3",
                        UserName = "emma.watson",
                        FirstName = "Emma",
                        LastName = "Watson",
                        Email = "emma.watson@example.com",
                        ProfilePicture = "profile3.jpg",
                        JobTitle = "Developer",
                        Gender = "Female",
                        Country = "USA",
                        CreatedAt = DateTime.UtcNow.AddDays(-25)
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
                    CreatorId="user2",
                    ChatId=2
                },
            };

            _chats = new List<Chat>
            {
                new Chat
                {
                    Id=1,
                    CreatedDate=DateTime.UtcNow,
                    Type=ChatType.Private,
                    CreatorId="user1"
                },
                new Chat
                {
                    Id=2,
                    CreatedDate=DateTime.Now.AddMinutes(-10),
                    Type=ChatType.Group,
                    CreatorId="user2"
                }
            };

            _chatParticipants = new List<ChatParticipants>
            {
                new ChatParticipants
                {
                    Id=1,
                    JoinedAt=DateTime.UtcNow,
                    UserId="user1",
                    ChatId=1
                },
                new ChatParticipants
                {
                    Id=2,
                    JoinedAt=DateTime.UtcNow,
                    UserId="user3",
                    ChatId=1
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

            _context.Users.AddRange(_users);
            _context.Rooms.AddRange(_rooms);
            _context.UserRooms.AddRange(_roomUsers);
            _context.Chats.AddRange(_chats);
            _context.ChatParticipants.AddRange(_chatParticipants);
            _context.SaveChanges();
        }

        private void UOWSetup()
        {
            _mockUnitOfWork.Setup(uow => uow.Chats.AddAsync(It.IsAny<Chat>()))
                .ReturnsAsync((Chat entity) =>
                {
                    entity.Id = _context.Chats.Count() + 1;
                    _context.Chats.Add(entity);
                    return entity;
                });

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

            _mockUnitOfWork.Setup(uow => uow.SaveAsync())
                .ReturnsAsync(() =>
                {
                    return _context.SaveChangesAsync().Result;
                });

            _mockUnitOfWork.Setup(uow => uow.ChatParticipants.AddRangeAsync(It.IsAny<List<ChatParticipants>>()))
                .Returns((List<ChatParticipants> entities) =>
                {
                    var id = _context.ChatParticipants.Count() + 1;
                    foreach (var entity in entities)
                    {
                        entity.Id = id++;
                    }
                    return Task.CompletedTask;
                });

            _mockUnitOfWork.Setup(uow => uow.ChatParticipants.DeleteAsync(It.IsAny<ChatParticipants>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(uow => uow.ChatParticipants.GetByExpressionAsync(It.IsAny<Expression<Func<ChatParticipants, bool>>>()))!
                .ReturnsAsync((Expression<Func<ChatParticipants, bool>> expr) =>
                {
                    var func = expr.Compile();
                    return _chatParticipants.FirstOrDefault(rt => func(rt));
                });
        }

        #endregion Helpers
    }
}
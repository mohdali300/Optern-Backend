using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.UnitOfWork;
using Optern.Application.Interfaces.IBookMarkedTaskService;
using Optern.Infrastructure.Services.BookMarkedTaskService;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ThreadingTask = System.Threading.Tasks.Task;
using Task = Optern.Domain.Entities.Task;
using Optern.Domain.Enums;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;

namespace Optern.Test.RoomTest.BookMarkedTaskTests
{
    [TestFixture]
    public class BookMarkedTaskTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private OpternDbContext _context;
        private IBookMarkedTaskService _bookMarkedTaskService;
        private List<Task> _tasks;
        private List<Room> _rooms;
        private List<ApplicationUser> _users;
        private List<UserRoom> _roomUsers;
        private List<Sprint> _sprints;
        private List<WorkSpace> _workspaces;
        private List<BookMarkedTask> _bookMarkedTasks;

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

            _bookMarkedTaskService = new BookMarkedTaskService(
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

        #region AddBookMarkedTask

        [Test]
        [Category("AddBookMarkedTask")]
        public async ThreadingTask AddBookMarkedTask_WithValidData_ReturnsSuccess()
        {
            var result = await _bookMarkedTaskService.Add("user2", 1);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Does.Contain("successfully"));
            });
        }

        [Test]
        [Category("AddBookMarkedTask")]
        public async ThreadingTask AddBookMarkedTask_WithMarkedTask_ReturnsBadRequest()
        {
            var result = await _bookMarkedTaskService.Add("user1", 1);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("already in your BookMarks"));
            });
        }

        #endregion AddBookMarkedTask

        #region DeleteBookMarkedTask

        [Test]
        [Category("DeleteBookMarkedTask")]
        public async ThreadingTask DeleteBookMarkedTask_WithValidData_ReturnsSuccess()
        {
            var result = await _bookMarkedTaskService.Delete("user1", 1);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Does.Contain("Removed"));
            });
        }

        [Test]
        [Category("DeleteBookMarkedTask")]
        public async ThreadingTask DeleteBookMarkedTask_WithNotMarkedTask_ReturnsBadRequest()
        {
            var result = await _bookMarkedTaskService.Delete("user2", 1);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("already is not in your BookMarks"));
            });
        }

        #endregion DeleteBookMarkedTask

        #region GetAllBookMarkedTask

        [Test]
        [Category("GetAllBookMarkedTask")]
        public async ThreadingTask GetAllBookMarkedTask_WithValidData_ReturnsAllInRoom()
        {
            var result = await _bookMarkedTaskService.GetAll("user1", "room1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data.Count, Is.EqualTo(2));
            });
        }

        [Test]
        [Category("GetAllBookMarkedTask")]
        public async ThreadingTask GetAllBookMarkedTask_WithNoMarkedTasks_ReturnsNoContent()
        {
            var result = await _bookMarkedTaskService.GetAll("user2", "room1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
                Assert.That(result.Data.Count, Is.EqualTo(0));
            });
        }

        #endregion GetAllBookMarkedTask

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

            _workspaces = new List<WorkSpace>
            {
                new WorkSpace
                {
                    Id=1,
                    Title="Workspace 1",
                    CreatedDate=DateTime.Now,
                    RoomId="room1",
                },
                new WorkSpace
                {
                    Id=2,
                    Title="Workspace 2",
                    CreatedDate=DateTime.Now,
                    RoomId="room2",
                },
            };

            _sprints = new List<Sprint>
            {
                new Sprint
                {
                    Id=1,
                    Title="Sprint 1",
                    StartDate=DateTime.Now,
                    EndDate=DateTime.Now.AddDays(7),
                    Goal="goal1",
                    WorkSpaceId=1
                },
                new Sprint
                {
                    Id=2,
                    Title="Sprint 2",
                    StartDate=DateTime.Now,
                    EndDate=DateTime.Now.AddDays(7),
                    Goal="goal2",
                    WorkSpaceId=1
                }
            };

            _tasks = new List<Task>
            {
                new Task
                {
                    Id = 1,
                    Title="Task 1",
                    Description="description task 1",
                    StartDate="20-10-2025",
                    EndDate="25-10-2025",
                    DueDate="25-10-2025",
                    Status=TaskState.ToDo,
                    SprintId=1,
                },
                new Task
                {
                    Id = 2,
                    Title="Task 2",
                    Description="description task 2",
                    StartDate="20-10-2025",
                    EndDate="25-10-2025",
                    DueDate="25-10-2025",
                    Status=TaskState.InProgress,
                    SprintId=1,
                },
            };

            _bookMarkedTasks = new List<BookMarkedTask>
            {
                new BookMarkedTask
                {
                    Id=1,
                    UserId="user1",
                    TaskId=1
                },
                new BookMarkedTask
                {
                    Id=2,
                    UserId="user1",
                    TaskId=2
                },
            };

            _context.Users.AddRange(_users);
            _context.Rooms.AddRange(_rooms);
            _context.UserRooms.AddRange(_roomUsers);
            _context.WorkSpaces.AddRange(_workspaces);
            _context.Sprints.AddRange(_sprints);
            _context.Tasks.AddRange(_tasks);
            _context.BookMarkedTasks.AddRange(_bookMarkedTasks);
            _context.SaveChanges();
        }

        private void UOWSetup()
        {
            _mockUnitOfWork.Setup(uow => uow.BookMarkedTask.AddAsync(It.IsAny<BookMarkedTask>()))
                .ReturnsAsync((BookMarkedTask entity) =>
                {
                    entity.Id = _context.BookMarkedTasks.Count() + 1;
                    _context.BookMarkedTasks.Add(entity);
                    return entity;
                });

            _mockUnitOfWork.Setup(uow => uow.SaveAsync())
                .ReturnsAsync(() =>
                {
                    return _context.SaveChangesAsync().Result;
                });
        }

        #endregion Helpers
    }
}
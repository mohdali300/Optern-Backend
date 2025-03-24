using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using Optern.Application.DTOs.Post;
using Optern.Application.Interfaces.IRoomTrackService;
using Optern.Application.Interfaces.ITaskActivityService;
using Optern.Domain.Entities;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Services.RoomTracksService;
using Optern.Infrastructure.Services.TaskActivityService;
using Optern.Infrastructure.UnitOfWork;
using System.Linq.Expressions;
using Optern.Application.DTOs.TaskActivity;
using Microsoft.AspNetCore.Http;
using ThreadingTask = System.Threading.Tasks.Task;
using Task = Optern.Domain.Entities.Task;
using Google.Apis.Util;

namespace Optern.Test.RoomTest.TaskActivityTests
{
    [TestFixture]
    public class TaskActivityTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private OpternDbContext _context;
        private IMapper _mapper;
        private ITaskActivityService _taskActivityService;
        private List<Task> _tasks;
        private List<TaskActivity> _taskActivities;
        private List<UserTasks> _userTasks;
        private List<Room> _rooms;
        private List<ApplicationUser> _users;
        private List<UserRoom> _roomUsers;
        private List<Sprint> _sprints;
        private List<WorkSpace> _workspaces;

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
            _mapper = MappingProfiles().CreateMapper();

            CreateSampleData();
            UOWSetup();

            _taskActivityService = new TaskActivityService(
                _mockUnitOfWork.Object,
                _context,
                _mapper
                );
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region AddTaskActivity

        [Test]
        [Category("AddTaskActivity")]
        public async ThreadingTask AddTaskActivity_WithValidData_ReturnsSuccess()
        {
            var model = new AddTaskCommentDTO
            {
                TaskId = 1,
                Content = "content 1",
                RoomId = "room1"
            };
            var result = await _taskActivityService.AddTaskActivityAsync(model, "user1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Message, Does.Contain("successfully"));
            });
        }

        [Test]
        [Category("AddTaskActivity")]
        public async ThreadingTask AddTaskActivity_WithEmptyContent_ReturnsBadRequest()
        {
            var model = new AddTaskCommentDTO
            {
                TaskId = 1,
                Content = "",
                RoomId = "room1"
            };
            var result = await _taskActivityService.AddTaskActivityAsync(model, "user1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("cannot be empty"));
            });
        }

        [Test]
        [Category("AddTaskActivity")]
        public async ThreadingTask AddTaskActivity_WithNonExistentRoomOrTask_ReturnsNotFound()
        {
            var model = new AddTaskCommentDTO
            {
                TaskId = 1,
                Content = "content",
                RoomId = "room3"
            };
            var result = await _taskActivityService.AddTaskActivityAsync(model, "user1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("not found"));
            });
        }

        [Test]
        [Category("AddTaskActivity")]
        public async ThreadingTask AddTaskActivity_WithNotAssignedUser_ReturnsForbidden()
        {
            var model = new AddTaskCommentDTO
            {
                TaskId = 1,
                Content = "content",
                RoomId = "room1"
            };
            var result = await _taskActivityService.AddTaskActivityAsync(model, "user2");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
                Assert.That(result.Message, Does.Contain("not have permission"));
            });
        }

        #endregion AddTaskActivity

        #region EditTaskActivity

        [Test]
        [Category("EditTaskActivity")]
        public async ThreadingTask EditTaskActivity_WithValidData_ReturnsUpdatedEntity()
        {
            var result = await _taskActivityService.EditTaskActivityAsync(1, "new content", "user1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Does.Contain("updated successfully"));
                Assert.That(result.Data, Is.Not.Null);
            });
        }

        [Test]
        [Category("EditTaskActivity")]
        public async ThreadingTask EditTaskActivity_WithEmptyContent_ReturnsBadRequest()
        {
            var result = await _taskActivityService.EditTaskActivityAsync(1, "", "user1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("cannot be empty"));
                Assert.That(result.Data, Is.Not.Null);
            });
        }

        [Test]
        [Category("EditTaskActivity")]
        public async ThreadingTask EditTaskActivity_WithNonExistentActivity_ReturnsNotFound()
        {
            var result = await _taskActivityService.EditTaskActivityAsync(3, "new content", "user1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("not found"));
                Assert.That(result.Data, Is.Not.Null);
            });
        }

        [Test]
        [Category("EditTaskActivity")]
        public async ThreadingTask EditTaskActivity_WithNotAssignedMember_ReturnsForbidden()
        {
            var result = await _taskActivityService.EditTaskActivityAsync(1, "new content", "user2");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
                Assert.That(result.Message, Does.Contain("not have permission"));
                Assert.That(result.Data, Is.Not.Null);
            });
        }

        [Test]
        [Category("EditTaskActivity")]
        public async ThreadingTask EditTaskActivity_WithCouldNotDelete_ReturnsBadRequest()
        {
            var result = await _taskActivityService.EditTaskActivityAsync(2, "new content", "user1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("cannot"));
                Assert.That(result.Data, Is.Not.Null);
            });
        }

        #endregion EditTaskActivity

        #region DeleteTaskActivity

        [Test]
        [Category("DeleteTaskActivity")]
        public async ThreadingTask DeleteTaskActivity_WithValidData_ReturnsSuccess()
        {
            var result = await _taskActivityService.DeleteTaskActivityAsync(1, "user1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Does.Contain("successfully"));
            });
        }

        [Test]
        [Category("DeleteTaskActivity")]
        public async ThreadingTask DeleteTaskActivity_WithNotAssignedMember_ReturnsForbidden()
        {
            var result = await _taskActivityService.DeleteTaskActivityAsync(1, "user2");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
                Assert.That(result.Message, Does.Contain("not have permission"));
                Assert.That(result.Data, Is.Not.Null);
            });
        }

        [Test]
        [Category("DeleteTaskActivity")]
        public async ThreadingTask DeleteTaskActivity_WithNonExistentActivity_ReturnsNotFound()
        {
            var result = await _taskActivityService.DeleteTaskActivityAsync(3, "user1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("not found"));
            });
        }

        [Test]
        [Category("DeleteTaskActivity")]
        public async ThreadingTask DeleteTaskActivity_WithCouldNotDelete_ReturnsBadRequest()
        {
            var result = await _taskActivityService.DeleteTaskActivityAsync(2, "user1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("cannot"));
            });
        }

        #endregion DeleteTaskActivity

        #region GetAllTaskActivities

        [Test]
        [Category("GetAllTaskActivities")]
        public async ThreadingTask GetAllTaskActivities_WithTaskId_ReturnsAll()
        {
            var result = await _taskActivityService.GetAllTaskActivitiesAsync(1);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data.Count, Is.EqualTo(2));
            });
        }

        [Test]
        [Category("GetAllTaskActivities")]
        public async ThreadingTask GetAllTaskActivities_WithNonExistentTask_ReturnsNotFound()
        {
            var result = await _taskActivityService.GetAllTaskActivitiesAsync(3);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("not found."));
                Assert.That(result.Data.Count, Is.EqualTo(0));
            });
        }

        [Test]
        [Category("GetAllTaskActivities")]
        public async ThreadingTask GetAllTaskActivities_WithNoTaskActivities_ReturnsNoContent()
        {
            var result = await _taskActivityService.GetAllTaskActivitiesAsync(2);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
                Assert.That(result.Message, Does.Contain("No task activities"));
                Assert.That(result.Data.Count, Is.EqualTo(0));
            });
        }

        #endregion GetAllTaskActivities

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

            _taskActivities = new List<TaskActivity>
            {
                new TaskActivity
                {
                    Id=1,
                    Content="content 1",
                    CreatedAt=DateTime.Now,
                    CouldDelete=true,
                    TaskId=1,
                    CreatorId="user1"
                },
                new TaskActivity
                {
                    Id=2,
                    Content="content 1",
                    CreatedAt=DateTime.Now,
                    CouldDelete=false,
                    TaskId=1,
                    CreatorId="user1"
                }
            };

            _userTasks = new List<UserTasks>
            {
                new UserTasks
                {
                    Id=1,
                    Attachmentdate=DateTime.Now,
                    UserId="user1",
                    TaskId=1
                },
                new UserTasks
                {
                    Id=2,
                    Attachmentdate=DateTime.Now,
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
            _context.UserTasks.AddRange(_userTasks);
            _context.TaskActivities.AddRange(_taskActivities);
            _context.SaveChanges();
        }

        private void UOWSetup()
        {
            _mockUnitOfWork.Setup(uow => uow.TaskActivites.GetAllAsync())
                .ReturnsAsync(_context.TaskActivities);

            _mockUnitOfWork.Setup(uow => uow.SaveAsync())
                .ReturnsAsync(() =>
                {
                    return _context.SaveChangesAsync().Result;
                });

            _mockUnitOfWork.Setup(uow => uow.TaskActivites.AddAsync(It.IsAny<TaskActivity>()))
                .ReturnsAsync((TaskActivity entity) =>
                {
                    entity.Id = _context.TaskActivities.Count() + 1;
                    _context.TaskActivities.Add(entity);
                    return entity;
                });

            _mockUnitOfWork.Setup(uow => uow.Rooms.AnyAsync(It.IsAny<Expression<Func<Room, bool>>>()))
                .ReturnsAsync((Expression<Func<Room, bool>> expr) =>
                {
                    var func = expr.Compile();
                    return _rooms.Any(room => func(room));
                });

            _mockUnitOfWork.Setup(uow => uow.Tasks.AnyAsync(It.IsAny<Expression<Func<Task, bool>>>()))
                .ReturnsAsync((Expression<Func<Task, bool>> expr) =>
                {
                    var func = expr.Compile();
                    return _tasks.Any(t => func(t));
                });

            _mockUnitOfWork.Setup(uow => uow.Rooms.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) =>
                {
                    var room = _rooms.FirstOrDefault(x => x.Id == id);
                    return room!;
                });

            _mockUnitOfWork.Setup(uow => uow.Tasks.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var task = _tasks.FirstOrDefault(x => x.Id == id);
                    return task!;
                });

            _mockUnitOfWork.Setup(uow => uow.TaskActivites.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var task = _context.TaskActivities.FirstOrDefault(x => x.Id == id);
                    return task!;
                });

            _mockUnitOfWork.Setup(uow => uow.UserRoom.AnyAsync(It.IsAny<Expression<Func<UserRoom, bool>>>()))
                .ReturnsAsync((Expression<Func<UserRoom, bool>> expr) =>
                {
                    var func = expr.Compile();
                    return _roomUsers.Any(ru => func(ru));
                });

            _mockUnitOfWork.Setup(uow => uow.UserTasks.AnyAsync(It.IsAny<Expression<Func<UserTasks, bool>>>()))
                .ReturnsAsync((Expression<Func<UserTasks, bool>> expr) =>
                {
                    var func = expr.Compile();
                    return _userTasks.Any(ut => func(ut));
                });
        }

        private MapperConfiguration MappingProfiles()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TaskActivity, TaskActivityDTO>()
                     .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.Creator != null ? src.Creator.FirstName + " " + src.Creator.LastName : string.Empty))
                     .ForMember(dest => dest.CreatorProfilePicture, opt => opt.MapFrom(src => src.Creator != null && src.Creator.ProfilePicture != null ? src.Creator.ProfilePicture : string.Empty));
            });
            return mapperConfig;
        }

        #endregion Helpers
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.UnitOfWork;
using Task = System.Threading.Tasks.Task;
using Optern.Domain.Enums;
using Optern.Application.Interfaces.ISprintService;
using Optern.Infrastructure.Services.SprintService;
using AutoMapper;
using Optern.Application.Interfaces.IRoomUserService;
using Optern.Infrastructure.ExternalInterfaces.ICacheService;
using Microsoft.AspNetCore.Http;
using Optern.Application.DTOs.Sprint;

namespace Optern.Test.RoomTest.SprintTests
{
    public class SprintTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private OpternDbContext _context;
        private IMapper _mapper;
        private ISprintService _sprintService;
        private Mock<IRoomUserService> _roomUserService;
        private Mock<ICacheService> _cacheService;
        private List<Room> _rooms;
        private List<ApplicationUser> _users;
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
            _roomUserService = new Mock<IRoomUserService>();
            _cacheService = new Mock<ICacheService>();
            _mapper = MappingProfiles().CreateMapper();

            CreateSampleData();
            UOWSetup();

            _sprintService = new SprintService(
                _mockUnitOfWork.Object,
                _context,
                _mapper,
                _cacheService.Object,
                _roomUserService.Object
                );
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region GetWorkSpaceSprints

        [Test]
        [Category("GetWorkSpaceSprints")]
        public async Task GetWorkSpaceSprints_WithExistedWorkspace_ReturnsAllSprints()
        {
            var result = await _sprintService.GetWorkSpaceSprints(1);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Does.Contain("Fetched"));
                Assert.That(result.Data.Count, Is.EqualTo(2));
            });
        }

        #endregion GetWorkSpaceSprints

        #region GetSprint

        [Test]
        [Category("GetSprint")]
        public async Task GetSprint_WithValidData_ReturnsSprint()
        {
            var result = await _sprintService.GetSprint("user1", "room1", 1);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Does.Contain("Fetched"));
                Assert.That(result.Data, Is.Not.Null);
            });
        }

        [Test]
        [Category("GetSprint")]
        public async Task GetSprint_WithNonExistentSprint_ReturnsNotFound()
        {
            var result = await _sprintService.GetSprint("user1", "room1", 5);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("No Sprints Found"));
            });
        }

        #endregion GetSprint

        #region AddSprint

        [Test]
        [Category("AddSprint")]
        public async Task AddSprint_WithValidData_ReturnsAddedSprint()
        {
            var model = new AddSprintDTO
            {
                Title = "new sprint",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                Goal = "new goal",
                WorkSpaceId = 2
            };

            var result = await _sprintService.AddSprint(model);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
                Assert.That(result.Message, Does.Contain("Added"));
                Assert.That(result.Data, Is.Not.Null);
            });
        }

        [Test]
        [Category("AddSprint")]
        public async Task AddSprint_WithNonExistentWorkSpace_ReturnsNotFound()
        {
            var model = new AddSprintDTO
            {
                Title = "new sprint",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                Goal = "new goal",
                WorkSpaceId = 4
            };

            var result = await _sprintService.AddSprint(model);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("not Found"));
            });
        }

        [Test]
        [Category("AddSprint")]
        public async Task AddSprint_WithInValidData_ReturnsBadRequest()
        {
            var model = new AddSprintDTO
            {
                Title = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                Goal = "new goal",
                WorkSpaceId = 2
            };

            var result = await _sprintService.AddSprint(model);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("Invalid"));
            });
        }

        #endregion AddSprint

        #region EditSprint

        [Test]
        [Category("EditSprint")]
        public async Task EditSprint_WithValidData_ReturnsUpdatedSprint()
        {
            var model = new EditSprintDTO
            {
                Title = "new sprint",
                StartDate = DateTime.Now.AddDays(-3),
                EndDate = DateTime.Now.AddDays(5),
                Goal = "new goal",
            };

            var result = await _sprintService.EditSprint(1, model);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Does.Contain("Updated"));
            });
        }

        [Test]
        [Category("EditSprint")]
        public async Task EditSprint_WithNonExistentSprint_ReturnsNotFound()
        {
            var model = new EditSprintDTO
            {
                Title = "new sprint",
                StartDate = DateTime.Now.AddDays(-3),
                EndDate = DateTime.Now.AddDays(5),
                Goal = "new goal",
            };

            var result = await _sprintService.EditSprint(4, model);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("not Found"));
            });
        }

        [Test]
        [Category("EditSprint")]
        public async Task EditSprint_WithInValidData_ReturnsBadRequest()
        {
            var model = new EditSprintDTO
            {
                Title = "",
                StartDate = DateTime.Now.AddDays(-3),
                EndDate = DateTime.Now.AddDays(5),
                Goal = "new goal",
            };

            var result = await _sprintService.EditSprint(1, model);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("Invalid"));
            });
        }

        #endregion EditSprint

        #region DeleteSprint

        [Test]
        [Category("DeleteSprint")]
        public async Task DeleteSprint_WithValidId_ReturnsSuccess()
        {
            var result = await _sprintService.DeleteSprint(1);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Does.Contain("Deleted"));
            });
        }

        [Test]
        [Category("DeleteSprint")]
        public async Task DeleteSprint_WithNonExistentSprint_ReturnsNotFound()
        {
            var result = await _sprintService.DeleteSprint(4);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("not Found"));
            });
        }

        #endregion DeleteSprint

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

            _context.Users.AddRange(_users);
            _context.Rooms.AddRange(_rooms);
            _context.WorkSpaces.AddRange(_workspaces);
            _context.Sprints.AddRange(_sprints);
            _context.SaveChanges();
        }

        private void UOWSetup()
        {
            _mockUnitOfWork.Setup(uow => uow.Sprints.AddAsync(It.IsAny<Sprint>()))
                .ReturnsAsync((Sprint entity) =>
                {
                    entity.Id = _context.Sprints.Count() + 1;
                    _context.Sprints.Add(entity);
                    return entity;
                });

            _mockUnitOfWork.Setup(uow => uow.Sprints.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var sprint = _sprints.FirstOrDefault(x => x.Id == id);
                    return sprint!;
                });

            _mockUnitOfWork.Setup(uow => uow.WorkSpace.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var space = _workspaces.FirstOrDefault(x => x.Id == id);
                    return space!;
                });

            _mockUnitOfWork.Setup(uow => uow.SaveAsync())
                .ReturnsAsync(() =>
                {
                    return _context.SaveChangesAsync().Result;
                });
        }

        private MapperConfiguration MappingProfiles()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Sprint, SprintResponseDTO>();
            });
            return mapperConfig;
        }

        #endregion Helpers
    }
}
using AutoMapper;
using Hangfire.PostgreSql.Properties;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using Optern.Application.DTOs.Tags;
using Optern.Application.DTOs.WorkSpace;
using Optern.Application.Interfaces.IRoomSkillService;
using Optern.Application.Interfaces.IWorkSpaceService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Services.WorkSpaceService;
using Optern.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Optern.Test.RoomTest.WorkSpaceServiceTests
{
    [TestFixture]
    public class WorkSpaceServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IMapper> _mockMapper;
        private IWorkSpaceService _workSpaceService;
        private OpternDbContext _context;
        private List<WorkSpace> _workSpaces;
        private List<Room> _rooms;
        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            var options = new DbContextOptionsBuilder<OpternDbContext>()
               .UseInMemoryDatabase(databaseName: "OpternTestDb")
               .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
               .Options;
            _context = new OpternDbContext(options);
            LoadData();
            _workSpaceService = new WorkSpaceService(_mockUnitOfWork.Object, _context, _mockMapper.Object);
        }
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }

        #region GetWorkSpaceTests

        [Test]
        [Category("GetWorkSpaceTests")]
        public async Task GetWorkSpace_ShouldReturn400Failed_WhenWorkSpaceIdEqualToZero()
        {
            int workSpaceId = 0;
            var result = await _workSpaceService.GetWorkSpace(workSpaceId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Data, Is.InstanceOf<WorkSpace>());
            Assert.That(result.Message, Does.Contain("Invalid Id"));
        }

        [Test]
        [Category("GetWorkSpaceTests")]
        [Sequential]
        public async Task GetWorkSpace_ShouldReturn404NotFound_WhenWorkSpaceNotExist([Values(10, 20, 30)] int workSpaceId)
        {
            MockSetup();
            var result = await _workSpaceService.GetWorkSpace(workSpaceId);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Data, Is.InstanceOf<WorkSpace>());
            Assert.That(result.Message, Does.Contain("WorkSpace Not Found!"));
        }
        [Test]
        [Category("GetWorkSpaceTests")]
        [Sequential]
        public async Task GetWorkSpace_ShouldReturn200NSuccess_WhenWorkSpaceExist([Values(1, 2, 3)] int workSpaceId)
        {
            MockSetup();
            var result = await _workSpaceService.GetWorkSpace(workSpaceId);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Message, Does.Contain("Workspace Fetched Successfully"));
            Assert.That(result.Data, Is.Not.Null);
        }

        [Test]
        [Category("GetWorkSpaceTests")]
        public async Task GetWorkSpace_ShouldReturn500ThrowException_WhenExceptionThrown()
        {
            _mockUnitOfWork.Setup(uow => uow.WorkSpace.GetByIdAsync(It.IsAny<int>()))
                     .Throws(new Exception("Connection Failed To Database"));
            var workSpaceId = 1;
            var result = await _workSpaceService.GetWorkSpace(workSpaceId);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(result.Message, Does.Contain("There is a server error. Please try again later."));
            Assert.That(result.Data, Is.Null);
        }
        #endregion

        #region Create WorkSpace Tests
        [Test]
        [Category("CreateWorkSpaceTests")]
        public async Task CreateWorkSpace_ShouldReturn404NotFoundRoom_WhenModelRoomNotFound()
        {
            var WorkSpaceDTO = new WorkSpaceDTO() { RoomId = "room10", Title = "newWorkSpace" };
            MockSetup();
            var result = await _workSpaceService.CreateWorkSpace(WorkSpaceDTO);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Message, Does.Contain("Room Not Found!"));
        }

        [Test]
        [Category("CreateWorkSpaceTests")]
        public async Task CreateWorkSpace_ShouldReturn400Failed_WhenModelTitleLengthLessThan5()
        {
            var WorkSpaceDTO = new WorkSpaceDTO() { RoomId = "room1", Title = "new" };
            MockSetup();
            var result = await _workSpaceService.CreateWorkSpace(WorkSpaceDTO);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Message, Does.Contain("Min Length for Title 5 characters"));
        }
        [Test]
        [Category("CreateWorkSpaceTests")]
        public async Task CreateWorkSpace_ShouldReturn201Success_WhenModelIsValid()
        {
            var WorkSpaceDTO = new WorkSpaceDTO() { RoomId = "room1", Title = "new WorkSpace" };
            MockSetup();
            _mockMapper.Setup(m => m.Map<WorkSpaceDTO>(It.IsAny<WorkSpace>()))
              .Returns((WorkSpace ws) => new WorkSpaceDTO
              {
                  Id = ws.Id,
                  Title = ws.Title,
                  RoomId = ws.RoomId,
                  CreatedDate = ws.CreatedDate
              });

            var result = await _workSpaceService.CreateWorkSpace(WorkSpaceDTO);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
            Assert.That(result.Message, Does.Contain("Workspace Created Successfully"));
        }

        [Test]
        [Category("CreateWorkSpaceTests")]
        public async Task CreateWorkSpace_ShouldReturn500ThrowException_WhenExceptionThrown()
        {
            _mockUnitOfWork.Setup(uow => uow.WorkSpace.GetByIdAsync(It.IsAny<int>()))
                     .Throws(new Exception("Connection Failed To Database"));
            var WorkSpaceDTO = new WorkSpaceDTO() { RoomId = "room1", Title = "new WorkSpace" };
            var result = await _workSpaceService.CreateWorkSpace(WorkSpaceDTO);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(result.Message, Does.Contain("There is a server error. Please try again later."));
            Assert.That(result.Data, Is.Null);
        }
        #endregion

        #region GetAllWorkSpaces
        [Test]
        [Category("GetAllWorkSpaceTests")]
        [TestCase("room3")]
        public async Task GetAllWorkSpace_ShouldReturnEmptyList_WhenWorkSpacesNotExistForRoom(string roomId)
        {
            MockSetup();
            var result = await _workSpaceService.GetAllWorkSpace(roomId);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data, Has.Exactly(0).Items);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
            Assert.That(result.Message, Does.Contain("Workspace Fetched Successfully"));
        }

        [Test]
        [Category("GetAllWorkSpaceTests")]
        [TestCase("room1")]
        public async Task GetAllWorkSpace_ShouldReturnWorkSpaceList_WhenWorkSpacesExistForRoom(string roomId)
        {
            MockSetup();
            var result = await _workSpaceService.GetAllWorkSpace(roomId);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data, Has.Exactly(2).Items);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Message, Does.Contain("Workspace Fetched Successfully"));
        }
        [Test]
        [Category("GetAllWorkSpaceTests")]
        [TestCase("room18")]
        public async Task GetAllWorkSpace_ShouldReturnWork404RoomNotFound_WhenWorkRoomNotExist(string roomId)
        {
            MockSetup();
            var result = await _workSpaceService.GetAllWorkSpace(roomId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Data, Is.InstanceOf<List<WorkSpace>>());
            Assert.That(result.Message, Does.Contain("Room Not Found!"));
        }
        [Test]
        [Category("GetAllWorkSpaceTests")]
        [TestCase("room18")]
        public async Task GetAllWorkSpace_ShouldReturn500ThrowException_WhenExceptionThrown(string roomId)
        {
            _mockUnitOfWork.Setup(uow => uow.WorkSpace.GetByIdAsync(It.IsAny<int>()))
                     .Throws(new Exception("Connection Failed To Database"));
            var result = await _workSpaceService.GetAllWorkSpace(roomId);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Data, Is.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(result.Message, Does.Contain("There is a server error. Please try again later."));
        }
        #endregion

        #region UpdateWorkSpaceTests
        [Test]
        [Category("UpdateWorkSpaceTests")]
        [TestCase(1, "new")]
        [TestCase(1, "")]
        public async Task UpdateWorkSpace_ShouldReturn400Failed_WhenDataInvalid(int id, string title)
        {
            var result = await _workSpaceService.UpdateWorkSpace(id, title);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Data, Is.InstanceOf<WorkSpaceDTO>());
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Message, Does.Contain("Title field is required and must be more than 5 characters to update!"));

        }

        [Test]
        [Category("UpdateWorkSpaceTests")]
        [TestCase(5, "new WorkSpace")]
        [TestCase(6, "new WorkSpace")]
        public async Task UpdateWorkSpace_ShouldReturn404NotFound_WhenWorkSpaceNotFound(int id, string title)
        {
            MockSetup();
            var result = await _workSpaceService.UpdateWorkSpace(id, title);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Data, Is.InstanceOf<WorkSpaceDTO>());
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Message, Does.Contain("WorkSpace Not Found!"));

        }

        [Test]
        [Category("UpdateWorkSpaceTests")]
        [TestCase(2, "Updated WorkSpace")]
        public async Task UpdateWorkSpace_ShouldReturn200Success_WhenWorkSpaceTitleIsValid(int id, string title)
        {
            MockSetup();
            _mockMapper.Setup(m => m.Map<WorkSpaceDTO>(It.IsAny<WorkSpace>()))
          .Returns((WorkSpace ws) => new WorkSpaceDTO
          {
              Id = ws.Id,
              Title = ws.Title,
              RoomId = ws.RoomId,
              CreatedDate = ws.CreatedDate
          });
            var result = await _workSpaceService.UpdateWorkSpace(id, title);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Message, Does.Contain("Workspace Updated Successfully"));
            Assert.That(result.Data.Title, Is.EqualTo(title));
            Assert.That(result.Data.Id, Is.EqualTo(id));
        }

        [Test]
        [Category("UpdateWorkSpaceTests")]
        [TestCase(2, "Updated WorkSpace")]
        public async Task UpdateWorkSpace_ShouldReturn500ThrowException_WhenExceptionThrown(int id, string title)
        {
            MockSetup();
            _mockUnitOfWork.Setup(u => u.WorkSpace.GetByIdAsync(id))
               .ThrowsAsync(new Exception("Connection Failed To Database"));


            var result = await _workSpaceService.UpdateWorkSpace(id, title);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(result.Message, Does.Contain("There is a server error. Please try again later"));
            Assert.That(result.Data, Is.Null);
        }
        #endregion

        #region DeleteWorkSpaceTests
        [Test]
        [Category("DeleteWorkSpaceTests")]
        [TestCase(100)]
        [TestCase(110)]
        public async Task DeleteWorkSpace_ShouldReturn404NotFound_WhenWorkSpaceNotExist(int id)
        {
            MockSetup();
            var result = await _workSpaceService.DeleteWorkSpace(id);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Data, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Message, Does.Contain("WorkSpace Not Found!"));
        }

        [Test]
        [Category("DeleteWorkSpaceTests")]
        [TestCase(1)]
        [TestCase(2)]
        public async Task DeleteWorkSpace_ShouldReturn200Success_WhenWorkSpaceExist(int id)
        {
            MockSetup();
            var result = await _workSpaceService.DeleteWorkSpace(id);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Message, Does.Contain("Workspace Deleted Successfully"));
        }
        [Test]
        [Category("DeleteWorkSpaceTests")]
        [TestCase(1)]
        public async Task DeleteWorkSpace_ShouldReturn500ThrowException_WhenExceptionThrown(int id)
        {
            MockSetup();
            _mockUnitOfWork.Setup(u => u.WorkSpace.GetByIdAsync(id))
                  .ThrowsAsync(new Exception("Connection Failed To Database"));


            var result = await _workSpaceService.DeleteWorkSpace(id);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(result.Message, Does.Contain("There is a server error. Please try again later"));
            Assert.That(result.Data, Is.False);
        } 
        #endregion

        #region Load Data
        private void LoadData()
        {
            _workSpaces = new List<WorkSpace>()
            {
                 new WorkSpace(){ Id=1,Title="workSpace1",RoomId="room1"},
                 new WorkSpace(){ Id=2,Title="workSpace2",RoomId="room1"},
                 new WorkSpace(){ Id=3,Title="workSpace1",RoomId="room2"},
            };
            _rooms = new List<Room>()
            {
                new Room { Id="room1",Name="C#"},
                  new Room { Id="room2",Name=".Net"},
                    new Room { Id="room3",Name="Java"}
            };
        }
        #endregion

        #region MockSetup
        private void MockSetup()
        {
            _mockUnitOfWork.Setup(uow => uow.WorkSpace.GetByIdAsync(It.IsAny<int>()))
           .ReturnsAsync((int id) =>
           {
               var workSpace = _workSpaces.FirstOrDefault(x => x.Id == id);
               return workSpace!;
           });

            _mockUnitOfWork.Setup(uow => uow.Rooms.GetByIdAsync(It.IsAny<string>()))
           .ReturnsAsync((string id) =>
           {
               var room = _rooms.FirstOrDefault(x => x.Id == id);
               return room!;
           });

            _mockUnitOfWork.Setup(uow => uow.WorkSpace.AddAsync(It.IsAny<WorkSpace>()))
              .ReturnsAsync((WorkSpace ws) =>
              {
                  ws.Title = "new WorkSpace ";
                  return ws;
              });


            _mockUnitOfWork.Setup(uow => uow.WorkSpace.GetAllByExpressionAsync(It.IsAny<Expression<Func<WorkSpace, bool>>>()))
               .ReturnsAsync((Expression<Func<WorkSpace, bool>> predicate) =>
                       _workSpaces.AsQueryable().Where(predicate.Compile()).ToList());

        }
        #endregion

    }
}

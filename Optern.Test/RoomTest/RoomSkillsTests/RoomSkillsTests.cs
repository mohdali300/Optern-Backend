using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using Optern.Application.Interfaces.IRoomSkillService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Services.RoomSkillService;
using Optern.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Optern.Test.RoomTest.RoomSkillsTests
{
    [TestFixture]
    public class RoomSkillsTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private IRoomSkillService _roomSkillService;
        private  OpternDbContext _context;
        private  List<Room> _rooms;
        private  List<Skills> _skills;
        private  List<RoomSkills> _roomSkills;


        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            var options = new DbContextOptionsBuilder<OpternDbContext>()
               .UseInMemoryDatabase(databaseName: "OpternTestDb")
               .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
               .Options;
            _context = new OpternDbContext(options);
            LoadData();
            _roomSkillService = new RoomSkillService(_mockUnitOfWork.Object, _context);
        }
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }

        #region Add RoomSkills
        [Test]
        [Category("AddRoomSkillsTest")]
        public async Task AddRoomSkills_ShouldReturn400Failed_WhenDataIsInvalid()
        {

            string roomID = "room1";
            IEnumerable<int> data = new List<int>();

            var result = await _roomSkillService.AddRoomSkills(roomID, data);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Does.Contain("Invalid Data Model"));
        }


        [Test]
        [Category("AddRoomSkillsTest")]
        public async Task AddRoomSkills_ShouldReturn404NotFound_WhenRoomNotExist()
        {

            string roomID = "room10";
            IEnumerable<int> data = new List<int>() { 1, 2 };
            _mockUnitOfWork.Setup(r => r.Rooms.GetByIdAsync(It.IsAny<int>()))
                 .ReturnsAsync(new Room { });

            var result = await _roomSkillService.AddRoomSkills(roomID, data);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Does.Contain("Room Not Found"));
        }
         
        [Test]
        [Category("AddRoomSkillsTest")]
        public async Task AddRoomSkills_ShouldReturn201Success_WhenDataIsValid()
        {

            string roomID = "room1";
            IEnumerable<int> data = new List<int>() { 1, 2, 3 };
            _mockUnitOfWork.Setup(r => r.Rooms.GetByIdAsync(It.IsAny<string>()))
                 .ReturnsAsync(new Room { Id = "room1", Name = "C#" });
            _mockUnitOfWork.Setup(r => r.RoomSkills.AddRangeAsync(It.IsAny<IEnumerable<RoomSkills>>()))
                           .Returns(Task.CompletedTask);

            var result = await _roomSkillService.AddRoomSkills(roomID, data);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Does.Contain("RoomSkills Added Successfully"));
        }
        [Test]
        [Category("AddRoomSkillsTest")]
        public async Task AddRoomSkills_ShouldReturn500Failed_WhenSkillsIdsNotExist()
        {

            string roomID = "room1";
            IEnumerable<int> data = new List<int>() { 4, 5 };
            _mockUnitOfWork.Setup(r => r.Rooms.GetByIdAsync(It.IsAny<string>()))
              .ReturnsAsync(new Room { Id = "room1", Name = "C#" });

            var result = await _roomSkillService.AddRoomSkills(roomID, data);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Does.Contain("There is a server error. Please try again later."));
        }
        #endregion

        #region DeleteRoomSkillsTest
        [Test]
        [Category("DeleteRoomSkills")]
        public async Task DeleteRoomSkills_ShouldReturn400Failed_WhenDataisInvalid()
        {
            string roomId = string.Empty;
            List<int> data = new List<int>();
            var result = await _roomSkillService.DeleteRoomSkills(roomId, data);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Data, Is.EqualTo(false));
            Assert.That(result.Message, Does.Contain("Invalid Data Model"));
        }

        [Test]
        [Category("DeleteRoomSkills")]
        public async Task DeleteRoomSkills_ShouldReturn404NotFound_WhenSkillsNotFound()
        {
            string roomId = "room1";
            List<int> data = new List<int>() {6,7,8};

            _mockUnitOfWork.Setup(r => r.RoomSkills.DeleteRangeAsync(It.IsAny<IEnumerable<RoomSkills>>()))
                         .Returns(Task.CompletedTask);

            var result = await _roomSkillService.DeleteRoomSkills(roomId, data);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.Data, Is.EqualTo(false));
            Assert.That(result.Message, Does.Contain("No matching Room Skills found to delete"));
        }

        [Test]
        [Category("DeleteRoomSkills")]
        public async Task DeleteRoomSkills_ShouldReturn404NotFound_WhenRoomNotExist()
        {

            string roomID = "room1";
            IEnumerable<int> data = new List<int>() { 1, 2 };
            _mockUnitOfWork.Setup(r => r.Rooms.GetByIdAsync(It.IsAny<int>()))
                 .ReturnsAsync(new Room { });
            _mockUnitOfWork.Setup(r => r.RoomSkills.DeleteRangeAsync(It.IsAny<IEnumerable<RoomSkills>>()))
                      .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(r => r.RoomSkills.GetAllByExpressionAsync(It.IsAny<Expression<Func<RoomSkills, bool>>>()))
                .ReturnsAsync((Expression<Func<RoomSkills, bool>> expr) =>
                {
                    return _roomSkills.AsQueryable().Where(expr).ToList();
                });

            var result = await _roomSkillService.AddRoomSkills(roomID, data);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Does.Contain("Room Not Found"));
        }

        [Test]
        [Category("DeleteRoomSkills")]
        public async Task DeleteRoomSkills_ShouldReturn200Success_WhenDataIsValid()
        {
            string roomId = "room1";
            List<int> data = new List<int>() { 1,2 };
            _mockUnitOfWork.Setup(r => r.Rooms.GetByIdAsync(It.IsAny<string>()))
               .ReturnsAsync(new Room { Id = "room1", Name = "C#" });
            _mockUnitOfWork.Setup(r => r.RoomSkills.DeleteRangeAsync(It.IsAny<IEnumerable<RoomSkills>>()))
                         .Returns(Task.CompletedTask);
                _mockUnitOfWork.Setup(r => r.RoomSkills.GetAllByExpressionAsync(It.IsAny<Expression<Func<RoomSkills, bool>>>()))
                    .ReturnsAsync((Expression<Func<RoomSkills, bool>> expr) =>
                    {
                        return _roomSkills.AsQueryable().Where(expr).ToList();
                    });

            var result = await _roomSkillService.DeleteRoomSkills(roomId, data);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(result.Data, Is.EqualTo(true));
            Assert.That(result.Message, Does.Contain("Room Skills Deleted Successfully"));
        }

        #endregion

        #region Load Data
        private void LoadData()
        {
            _skills = new List<Skills>
            {
                new Skills { Id = 1, Name = "C#" },
                new Skills { Id = 2, Name = "SQL" },
                new Skills { Id = 3, Name = "Azure" },
            };

            _rooms = new List<Room>()
            {
                new Room { Id="room1",Name="C#"},
                  new Room { Id="room2",Name=".Net"},
                    new Room { Id="room3",Name="Java"}
            };

            _roomSkills = new List<RoomSkills>
            {
                new RoomSkills { Id = 1, RoomId = "room1", SkillId = 1 },
                new RoomSkills { Id = 2, RoomId = "room1", SkillId = 2 },
                 new RoomSkills { Id = 3, RoomId = "room1", SkillId = 3 }
            };

        } 
        #endregion

    }
}

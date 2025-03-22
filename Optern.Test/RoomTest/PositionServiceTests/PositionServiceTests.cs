using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using Optern.Application.DTOs.Position;
using Optern.Application.Interfaces.IPositionService;
using Optern.Application.Interfaces.IRoomSkillService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Services.PositionService;
using Optern.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Optern.Test.RoomTest.PositionServiceTests
{
    [TestFixture]
    public class PositionServiceTests
    {

        private Mock<IUnitOfWork> _mockUnitOfWork;
        private IPositionService _positionService;
        private OpternDbContext _context;
        private List<Position> _positions;
        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            var options = new DbContextOptionsBuilder<OpternDbContext>()
              .UseInMemoryDatabase(databaseName: "OpternTestDb")
              .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
              .Options;
            _context = new OpternDbContext(options);
            LoadData();
            _positionService = new PositionService(_mockUnitOfWork.Object, _context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }

        #region Add Position TestCases
        [Test]
        [Category("AddPositionTests")]
        public async Task AddPosition_ShouldReturn400Failed_WhenNameIsEmpty()
        {
            string name = string.Empty;
            var result = await _positionService.AddAsync(name);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data, Is.InstanceOf<PositionDTO>());

        }
        [Test]
        [Category("AddPositionTests")]
        public async Task AddPosition_ShouldReturn400Failed_WhenNameLengthMoreThan100()
        {
            string name = "asdfghjkl;asdfghjkl;asdfghjkl;asdfghjkl;asdfghjkl;asdfghjkl;asdfghjkl;asdfghjkl;asdfghjkl;asdfghjkl;a";

            var result = await _positionService.AddAsync(name);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data, Is.InstanceOf<PositionDTO>());
            Assert.That(result.Message, Does.Contain("Invalid Data Model:"));

        }
        [Test]
        [Category("AddPositionTests")]
        public async Task AddPosition_ShouldReturn201Success_WhenNameIsValid()
        {
            string name = "Python";
            _mockUnitOfWork.Setup(uow => uow.Positions.AddAsync(It.IsAny<Position>()))
                 .ReturnsAsync((Position position) =>
                 {
                     position.Id = 5;
                     position.Name = "Python";
                     return position;
                 });
            var result = await _positionService.AddAsync(name);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Message, Does.Contain("Position added successfully."));

        }

        [Test]
        [Category("AddPositionTests")]
        public async Task AddAsync_ShouldReturn500_WhenExceptionIsThrown()
        {
            string positionName = "Software Engineer";

            _mockUnitOfWork.Setup(u => u.Positions.AddAsync(It.IsAny<Position>()))
                           .ThrowsAsync(new Exception("Database connection failed"));

            var result = await _positionService.AddAsync(positionName);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(result.Message, Is.EqualTo("Unexpected error occured!"));
            Assert.That(result.Errors, Has.One.Items);
            Assert.That(result.Errors.First(), Does.Contain("Database connection failed"));
        }
        #endregion


        [Test]
        [Category("GetAllPositionsTests")]
        public async Task GetAllPositions_ShouldReturnEmptyList_WhenNoPositionExist()
        {
            var emptyPositionList= new List<Position>();
            _mockUnitOfWork.Setup(uow => uow.Positions.GetAllAsync())
                     .ReturnsAsync(emptyPositionList); 

            var result= await _positionService.GetAllAsync();
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.InstanceOf<List<PositionDTO>>());
            Assert.That(result.Message, Does.Contain("No positions found!"));
            Assert.That(result.Data, Has.Exactly(0).Items);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
        }

        [Test]
        [Category("GetAllPositionsTests")]
        public async Task GetAllPositions_ShouldReturnPositionsList_WhenPositionExist()
        {
            _mockUnitOfWork.Setup(uow => uow.Positions.GetAllAsync())
                     .ReturnsAsync(_positions);

            var result = await _positionService.GetAllAsync();
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Does.Contain("Positions Fetched Successfully"));
            Assert.That(result.Data, Has.Exactly(3).Items);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }


        [Test]
        [Category("GetAllPositionsTests")]
        public async Task GetAllPositions_ShouldThrowException_WhenThereIsExceptionThrown()
        {
            _mockUnitOfWork.Setup(uow => uow.Positions.GetAllAsync())
                   .ThrowsAsync(new Exception("Database connection failed"));

            var result = await _positionService.GetAllAsync();
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Does.Contain("Unexpected error occured"));
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        }
        private void LoadData()
        {
            _positions = new List<Position> {
                new Position { Id = 1, Name=".Net" },
                new Position { Id = 2, Name="VB" },
                new Position { Id =2, Name="Java" },
            };
        }

    }
}

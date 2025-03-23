using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using Optern.Application.Interfaces.ITrackService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Services.TrackService;
using Optern.Infrastructure.UnitOfWork;
using Task = System.Threading.Tasks.Task;

namespace Optern.Test.RoomTest.TrackServiceTests
{
    [TestFixture]
    public class TrackServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private OpternDbContext _context;
        private ITrackService _trackService;
        private List<Track> _tracks;

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

            _trackService = new TrackService(
                _mockUnitOfWork.Object
                );
        }

        #region GetAll

        [Test]
        [Category("AllTracks")]
        public async Task GetAllTracks_ReturnsAllTracks()
        {
            var result = await _trackService.GetAll();

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.Count(), Is.EqualTo(3));
            });
        }

        [Test]
        [Category("AllTracks")]
        public async Task GetAllTracks_WithEmptyTracks_ReturnsNoContent()
        {
            _context.Tracks.RemoveRange(_tracks);
            _context.SaveChanges();

            var result = await _trackService.GetAll();

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Message, Is.EqualTo("No tracks found!"));
            });
        }

        #endregion GetAll

        #region AddTrack

        [Test]
        [Category("AddTrack")]
        [TestCase("Track1")]
        public async Task AddTrack_WithValidData_ReturnsAddedTrack(string name)
        {
            var result = await _trackService.Add(name);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Message, Does.Contain("successfully"));
                Assert.That(result.Data.Name, Is.EqualTo("Track1"));
            });
        }

        [Test]
        [Category("AddTrack")]
        [TestCase("")]
        public async Task AddTrack_WithInValidData_ReturnsFailure(string name)
        {
            var result = await _trackService.Add(name);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Does.Contain("Invalid"));
            });
        }

        #endregion AddTrack


        #region Helpers

        private void CreateSampleData()
        {
            _tracks = new List<Track>
            {
                new Track {Id=1,Name="DotNet"},
                new Track {Id=2,Name="NodeJs"},
                new Track {Id=3,Name="Angular"},
            };

            _context.Tracks.AddRange(_tracks);
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
        }

        #endregion Helpers
    }
}
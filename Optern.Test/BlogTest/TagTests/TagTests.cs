using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Task= System.Threading.Tasks;
using Optern.Infrastructure.Services.TagService;
using Optern.Domain.Entities;
using Optern.Application.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs.Tags;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Optern.Application.Interfaces.ITagService;

namespace Optern.Tests.Services
{
    [TestFixture]
    public class TagsServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private OpternDbContext _context;
        private Mock<IMapper> _mockMapper;
        private ITagsService _tagsService;
        private List<Tags> _tags;

        [SetUp]
        public async Task.Task Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();

            var options = new DbContextOptionsBuilder<OpternDbContext>()
                .UseInMemoryDatabase(databaseName: "OpternTestDb")
                .Options;

            _context = new OpternDbContext(options);
            _tags= new List<Tags>()
            {
           new Tags { Id = 1, Name = "C#" },
           new Tags { Id = 2, Name = "ASP.NET" },
           new Tags { Id = 3, Name = "Entity FrameWork" },
          };
           await _context.Tags.AddRangeAsync(_tags);
           await _context.SaveChangesAsync();
            _mockUnitOfWork.Setup(uow => uow.Tags.GetAllAsync()).ReturnsAsync(_tags);

            _mockMapper.Setup(m => m.Map<IEnumerable<TagDTO>>(_tags))
                .Returns(_tags.Select(t => new TagDTO { Id = t.Id, Name = t.Name }));


            _tagsService = new TagsService(_mockUnitOfWork.Object, _context, _mockMapper.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }


        [Test]
        [Category("TagTests")]
        [TestCase(0)]
        [TestCase(null)]
        public async Task.Task GetTopTagsAsync_WhenTopNIsNullOrEqualToZero_ShouldReturnAllTags(int? num=null)
        {

            var result = await _tagsService.GetTopTagsAsync(num);

            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result, Is.Not.Null);
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Data.Count(), Is.EqualTo(3));
            });

        }

        [Test]
        [Category("TagTests")]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task.Task GetTopTagsAsync_WhenTopNIsProvided_ShouldReturnTopTags(int num)
        {
            var postTags = new List<PostTags>
            {
                new PostTags { PostId = 1, TagId = 1, Tag = _tags.FirstOrDefault(t=>t.Id==1) },
                new PostTags { PostId = 2, TagId = 1, Tag = _tags.FirstOrDefault(t=>t.Id==2) },
                new PostTags { PostId = 3, TagId = 2, Tag =  _tags.FirstOrDefault(t=>t.Id==3)  }
            };

            await _context.PostTags.AddRangeAsync(postTags);
            await _context.SaveChangesAsync();

            _mockMapper.Setup(m => m.Map<IEnumerable<TagDTO>>(It.IsAny<IEnumerable<Tags>>()))
                .Returns((IEnumerable<Tags> tags) => tags.Select(t => new TagDTO { Id = t.Id, Name = t.Name }));

            var result = await _tagsService.GetTopTagsAsync(num);
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result, Is.Not.Null);
                Assert.That(result.IsSuccess, Is.True); 
                Assert.That(result.Data.Count(), Is.EqualTo(num));
            });
        }

        [Test]
        [Category("TagTests")]
        public async Task.Task GetTopTagsAsync_WhenNoTagsExist_ShouldReturnEmptyList()
        {
            _context.Tags.RemoveRange(_context.Tags);  
            _context.PostTags.RemoveRange(_context.PostTags); 
            await _context.SaveChangesAsync();

            var result = await _tagsService.GetTopTagsAsync(3);

            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Data.Count(), Is.EqualTo(0));
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Data, Is.Empty);
            });
        }




    }
}

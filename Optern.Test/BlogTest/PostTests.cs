using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Optern.Application.Interfaces.IPostService;
using Optern.Application.Response;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.ExternalInterfaces.ICacheService;
using Optern.Infrastructure.Services.PostService;
using Optern.Infrastructure.UnitOfWork;
using Task = System.Threading.Tasks.Task;

namespace Optern.Test.BlogTest
{
    [TestFixture]
    public class PostTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork = null!;
        private OpternDbContext _context = null!;
        private Mock<IMapper> _mockMapper = null!;
        private Mock<ICacheService> _mockCacheService = null!;
        private IPostService _postService = null!;
        private List<Post> _samplePosts = null!;

        [SetUp]
        public void SetUp()
        {
            // Create in memory database with a unique name for each test
            var options = new DbContextOptionsBuilder<OpternDbContext>()
                .UseInMemoryDatabase(databaseName: "OpternTestDB")
                .EnableSensitiveDataLogging()
                .Options;

            _context = new OpternDbContext(options);
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockCacheService = new Mock<ICacheService>();

            // sample data
            _samplePosts = new List<Post>
            {
                new Post
                {
                    Id = 1,
                    Title = "Post 1",
                    Content = "Content 1",
                    Creator = new ApplicationUser
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
                    CreatedDate = DateTime.UtcNow.AddMinutes(-10),
                    PostTags = new List<PostTags>(),
                    Reacts = new List<Reacts>(),
                    Comments = new List<Comment>()
                },
                new Post
                {
                    Id = 2,
                    Title = "Post 2",
                    Content = "Content 2",
                    Creator = new ApplicationUser
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
                    CreatedDate = DateTime.UtcNow.AddMinutes(-15),
                    PostTags = new List<PostTags>(),
                    Reacts = new List<Reacts>(),
                    Comments = new List<Comment>()
                }
            };

            // add to the in memory database
            _context.Posts.AddRange(_samplePosts);
            _context.SaveChanges();

            _postService = new PostService(
                _mockUnitOfWork.Object,
                _context,
                _mockMapper.Object,
                _mockCacheService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region GetLatestPosts
        
        [Test, Category("GetLatestPosts")]
        public async Task GetLatestPosts_ReturnsAllPosts()
        {
            // Act
            var result = await _postService.GetLatestPostsAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.Count(), Is.EqualTo(2));
            });
        }

        [Test, Category("GetLatestPosts")]
        [TestCase("user1")]
        [TestCase("user2")]
        public async Task GetLatestPosts_WithUserId_ReturnsUserPosts(string userId)
        {
            // Act
            var result = await _postService.GetLatestPostsAsync(userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.Count(), Is.EqualTo(1));
            });
        }

        [Test, Category("GetLatestPosts")]
        [TestCase(0, 2)]
        [TestCase(1, 2)]
        [TestCase(2, 2)]
        public async Task GetLatestPosts_WithPagination_ReturnsPostsAfterSkip(int skip, int take)
        {
            // Act
            var result = await _postService.GetLatestPostsAsync(null, skip, take);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.Count(), Is.EqualTo(take - skip));
            });
        }

        [Test, Category("GetLatestPosts")]
        public async Task GetLatestPosts_EmptyPosts_ReturnsEmptyList()
        {
            // Arrange
            var emptyList = new List<Post>();
            _context.RemoveRange(_samplePosts);
            _context.SaveChanges();

            // Act
            var result = await _postService.GetLatestPostsAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.Count(), Is.EqualTo(0));
                Assert.That(result.Message, Is.EqualTo("No posts found."));
            });
        }

        [Test, Category("GetLatestPosts")]
        public async Task GetLatestPosts_WithExceptions_ShouldHandleException()
        {
            // Arrange
            OpternDbContext dbContext = null!;
            _postService = new PostService(_mockUnitOfWork.Object, dbContext, _mockMapper.Object, _mockCacheService.Object);
            // Act
            var result = await _postService.GetLatestPostsAsync();
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
                Assert.That(result.Data, Is.Not.Null);
            });
        } 

        #endregion

    }
}

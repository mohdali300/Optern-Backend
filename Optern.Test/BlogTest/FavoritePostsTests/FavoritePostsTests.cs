using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using Optern.Application.DTOs.FavoritePosts;
using Optern.Application.DTOs.Post;
using Optern.Application.Interfaces.IFavoritePostsService;
using Optern.Domain.Entities;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Services.FavoritePostsService;
using Optern.Infrastructure.UnitOfWork;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

namespace Optern.Test.BlogTest.FavoritePostsTests
{
    [TestFixture,Category("FavoritePostsTests")]
    public class FavoritePostsTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork = null!;
        private OpternDbContext _context = null!;
        private IMapper _mapper = null!;
        private List<Post> _samplePosts = null!;
        private List<ApplicationUser> _sampleUsers = null!;
        private List<FavoritePosts> _sampleFavoritePosts = null!;
        private List<Reacts> _sampleReacts = null!;
        private List<Tags> _sampleTags = null!;
        private List<PostTags> _samplePostTags = null!;
        private FavoritePostsService _favoritePostsService = null!;

        [SetUp]
        public void SetUp()
        {
            // Create in memory database with a unique name for each test
            var options = new DbContextOptionsBuilder<OpternDbContext>()
                .UseInMemoryDatabase(databaseName: $"OpternTestDB_{Guid.NewGuid()}")
                .ConfigureWarnings(warnings =>
                    warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .EnableSensitiveDataLogging()
                .Options;

            _context = new OpternDbContext(options);
            _mapper = MappingProfiles().CreateMapper();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            // create sample data and add to in memory DB
            CreateSampleData();
            UOWSetup();

            _favoritePostsService = new FavoritePostsService(
                _mockUnitOfWork.Object,
                _context,
                _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region GetFavoritePostsByUser
        [Test]
        public async Task GetFavoritePostsByUser_WithValidUserId_ReturnsUserFavPosts()
        {

            var result = await _favoritePostsService.GetFavoritePostsByUserAsync("user1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.Count, Is.EqualTo(2));
                Assert.That(result.Data.Where(p => p.Id == 1),
                    Is.EqualTo(result.Data.Where(p => p.CreatorName == "John Doe")));
                Assert.That(result.Data.Where(p => p.Id == 2),
                    Is.EqualTo(result.Data.Where(p => p.ReactsCount == 1)));
            });
        }

        [Test]
        public async Task GetFavoritePostsByUser_WithInValidUserId_ReturnsNoFavPostsFound()
        {

            var result = await _favoritePostsService.GetFavoritePostsByUserAsync("user4");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data, Is.Empty);
                Assert.That(result.Message, Is.EqualTo("No favorite posts found for the user."));
            });
        }

        [Test]
        [TestCase("user1", 0, 2)]
        [TestCase("user1", 1, 2)]
        [TestCase("user2", 1, 1)]
        public async Task GetFavoritePostsByUser_WithPagination_ReturnsCorrectPostsNumber(string userId, int skip, int take)
        {

            var result = await _favoritePostsService.GetFavoritePostsByUserAsync(userId, skip, take);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.Count, Is.EqualTo(take - skip));
            });
        }
        #endregion

        #region AddToFavorite
        [Test]
        public async Task AddToFavorite_WithValidData_ShouldAddedSuccessfully()
        {
            var favPost = new AddToFavoriteDTO
            {
                PostId = 2,
                UserId = "user2"
            };

            var result = await _favoritePostsService.AddToFavoriteAsync(favPost);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data, Is.EqualTo("Post added to favorites successfully"));
                Assert.That(result.Message, Is.EqualTo("Post added to favorites successfully."));
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            });
        }

        [Test]
        [TestCase("user1", 0)]
        [TestCase(null, 1)]
        public async Task AddToFavorite_WithInValidData_ReturnsInvalidDataModel(string userId, int postId)
        {
            var favPost = new AddToFavoriteDTO
            {
                PostId = postId,
                UserId = userId
            };

            var result = await _favoritePostsService.AddToFavoriteAsync(favPost);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Message, Is.EqualTo("Invalid Data Model"));
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            });
        }

        [Test]
        [TestCase("user4", 1)]
        [TestCase("user1", 3)]
        [TestCase("user4", 3)]
        public async Task AddToFavorite_WithNonExistentUserOrPost_ReturnsNotFound(string userId, int postId)
        {
            var favPost = new AddToFavoriteDTO
            {
                PostId = postId,
                UserId = userId
            };

            var result = await _favoritePostsService.AddToFavoriteAsync(favPost);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Message, Is.EqualTo("User or Post not found"));
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            });
        }

        [Test]
        [TestCase("user1", 2)]
        [TestCase("user2", 1)]
        public async Task AddToFavorite_WithExistentFavPost_ReturnsIsExisted(string userId, int postId)
        {
            var favPost = new AddToFavoriteDTO
            {
                PostId = postId,
                UserId = userId
            };

            var result = await _favoritePostsService.AddToFavoriteAsync(favPost);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Message, Is.EqualTo("This post is already in your favorites."));
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            });
        }
        #endregion

        #region DeleteFavoritePost
        [Test]
        public async Task DeleteFavoritePost_WithValidUserId_ShouldRemoveAllUserFavPosts()
        {
            var result = await _favoritePostsService.DeleteFavoritePostAsync("user1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Is.EqualTo("All favorite posts deleted successfully."));
            });
        }

        [Test]
        public async Task DeleteFavoritePost_WithValidPostId_ShouldRemoveFavPost()
        {
            var result = await _favoritePostsService.DeleteFavoritePostAsync("user1", 1);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Is.EqualTo("Favorite post deleted successfully."));
            });
        }

        [Test]
        [TestCase("user4", 1)]
        [TestCase("user1", 3)]
        [TestCase("user4", 3)]
        [TestCase("user4", 3)]
        public async Task DeleteFavoritePost_WithNonExistentUserOrPost_ReturnsNotFound(string userId, int postId)
        {
            var result = await _favoritePostsService.DeleteFavoritePostAsync(userId, postId);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Is.EqualTo("User Not Found").Or.EqualTo("Favorite post not found."));
            });
        }

        [Test]
        [TestCase(null, 1)]
        [TestCase("user1", 0)]
        public async Task DeleteFavoritePost_WithInvalidData_ReturnsInvalidData(string userId, int postId)
        {
            var result = await _favoritePostsService.DeleteFavoritePostAsync(userId, postId);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Message, Is.EqualTo("Invalid Data Model"));
            });
        }

        [Test]
        public async Task DeleteFavoritePost_WithEmptyUserFavPosts_ReturnsEmpty()
        {
            var result = await _favoritePostsService.DeleteFavoritePostAsync("user3");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
                Assert.That(result.Message, Is.EqualTo("No favorite posts found for this user."));
            });
        } 
        #endregion


        #region Helpers
        private void CreateSampleData()
        {
            _sampleUsers = new List<ApplicationUser>
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
                        JobTitle = "Designer",
                        Gender = "Female",
                        Country = "UK",
                        CreatedAt = DateTime.UtcNow.AddDays(-20)
                    }
            };

            _samplePosts = new List<Post>
            {
                new Post
                {
                    Id = 1,
                    Title = "Post 1",
                    Content = "Content 1",
                    CreatorId="user1" ,
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
                    CreatorId="user2",
                    CreatedDate = DateTime.UtcNow.AddMinutes(-15),
                    PostTags = new List<PostTags>(),
                    Reacts = new List<Reacts>(),
                    Comments = new List<Comment>()
                }
            };

            _sampleFavoritePosts = new List<FavoritePosts>
            {
                new FavoritePosts
                {
                    Id=100,
                    SavedAt= DateTime.UtcNow,
                    UserId="user1",
                    PostId=2
                },
                new FavoritePosts
                {
                    Id=101,
                    SavedAt= DateTime.UtcNow,
                    UserId="user1",
                    PostId=1
                },
                new FavoritePosts
                {
                    Id=102,
                    SavedAt= DateTime.UtcNow,
                    UserId="user2",
                    PostId=1
                },
            };

            _sampleReacts = new List<Reacts>
            {
                new Reacts
                {
                    Id=10,
                    ReactDate= DateTime.UtcNow,
                    ReactType=ReactType.VOTEUP,
                    PostId=2,
                    UserId="user2",
                }
            };

            _sampleTags = new List<Tags>
            {
                new Tags
                {
                    Id=5,
                    Name="DotNet"
                },

                new Tags
                {
                    Id=6,
                    Name="GraphQL"
                },

                new Tags
                {
                    Id=7,
                    Name="PostgreSQL"
                }
            };

            _samplePostTags = new List<PostTags>
            {
                new PostTags
                {
                    Id= 8,
                    PostId=1,
                    TagId=5
                },
                new PostTags
                {
                    Id= 9,
                    PostId=2,
                    TagId=6
                },
            };

            // add to the in memory database
            _context.Posts.AddRange(_samplePosts);
            _context.Users.AddRange(_sampleUsers);
            _context.FavoritePosts.AddRange(_sampleFavoritePosts);
            _context.Reacts.AddRange(_sampleReacts);
            _context.Tags.AddRange(_sampleTags);
            _context.PostTags.AddRange(_samplePostTags);
            _context.SaveChanges();
        }

        private void UOWSetup()
        {
            _mockUnitOfWork.Setup(uow => uow.FavoritePosts.AddAsync(It.IsAny<FavoritePosts>()))
                .ReturnsAsync((FavoritePosts entity) => { return entity; });

            _mockUnitOfWork.Setup(uow => uow.FavoritePosts.DeleteAsync(It.IsAny<FavoritePosts>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(uow => uow.FavoritePosts.GetQueryable(
                    It.IsAny<Expression<Func<FavoritePosts, bool>>>(),
                    It.IsAny<string>(),
                    It.IsAny<Func<IQueryable<FavoritePosts>, IOrderedQueryable<FavoritePosts>>>()))
                .Returns<Expression<Func<FavoritePosts, bool>>, string, Func<IQueryable<FavoritePosts>, IOrderedQueryable<FavoritePosts>>>((filter, includeProperties, orderBy) =>
                {
                    var query = _context.FavoritePosts.AsQueryable();
                    if (filter != null)
                    {
                        query = query.Where(filter);
                    }
                    if (orderBy != null)
                    {
                        query = orderBy(query);
                    }

                    return query;
                });

            _mockUnitOfWork.Setup(uow => uow.FavoritePosts.GetByExpressionAsync(
                It.IsAny<Expression<Func<FavoritePosts, bool>>>()
                )).Returns<Expression<Func<FavoritePosts, bool>>>(expression =>
                {
                    var dlg= expression.Compile();
                    var favPost = _sampleFavoritePosts.FirstOrDefault(fp => dlg(fp));
                    return Task.FromResult(favPost!);
                });

            _mockUnitOfWork.Setup(uow => uow.FavoritePosts.GetAllByExpressionAsync(
                It.IsAny<Expression<Func<FavoritePosts, bool>>>()
                )).Returns<Expression<Func<FavoritePosts, bool>>>(expression =>
                {
                    var dlg = expression.Compile();
                    var favPosts = _sampleFavoritePosts.Where(fp=>dlg(fp)).ToList();
                    return Task.FromResult(favPosts!.AsEnumerable());
                });

            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) =>
                {
                    var user = _sampleUsers.FirstOrDefault(x => x.Id == id);
                    return user!;
                });

            _mockUnitOfWork.Setup(uow => uow.Posts.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var post=_samplePosts.FirstOrDefault(x => x.Id == id);
                    return post!;
                });
        }

        private MapperConfiguration MappingProfiles()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AddToFavoriteDTO, FavoritePosts>();

            });
            return mapperConfig;
        }
        #endregion
    }
}

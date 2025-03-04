using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using Optern.Application.DTOs.Post;
using Optern.Application.Interfaces.IPostService;
using Optern.Application.Response;
using Optern.Domain.Entities;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.ExternalInterfaces.ICacheService;
using Optern.Infrastructure.Services.PostService;
using Optern.Infrastructure.UnitOfWork;
using Task = System.Threading.Tasks.Task;

namespace Optern.Test.BlogTest
{
    [TestFixture]
    [Category("PostTests")]
    public class PostTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork = null!;
        private OpternDbContext _context = null!;
        private Mock<ICacheService> _mockCacheService = null!;
        private IPostService _postService = null!;
        private List<Post> _samplePosts = null!;
        private List<ApplicationUser> _sampleUsers = null!;
        private List<FavoritePosts> _sampleFavoritePosts = null!;
        private List<Reacts> _sampleReacts = null!;
        private List<Tags> _sampleTags = null!;
        private List<PostTags> _samplePostTags = null!;
        private IMapper _mapper = null!;
        private static int _postId = 1;
        private static int _tagId = 5;
        private static int _postTagId = 8;
        private static int _favPostId = 100;

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
            _mockCacheService = new Mock<ICacheService>();

            // create sample data and add to in memory DB
            CreateSampleData();
            // UOW mocking setup
            UOWSetup();

            _postService = new PostService(
                _mockUnitOfWork.Object,
                _context,
                _mapper,
                _mockCacheService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region GetLatestPosts
        
        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
        public async Task GetLatestPosts_EmptyPosts_ReturnsEmptyList()
        {
            // Arrange
            _context.RemoveRange(_samplePosts);
            _context.SaveChanges();

            // Act
            var result = await _postService.GetLatestPostsAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.Count(), Is.EqualTo(0));
                Assert.That(result.Message, Is.EqualTo("No posts found."));
            });
        }

        [Test]
        public async Task GetLatestPosts_WithExceptions_ShouldHandleException()
        {
            // Arrange
            OpternDbContext dbContext = null!;
            _postService = new PostService(_mockUnitOfWork.Object, dbContext, _mapper, _mockCacheService.Object);
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

        #region GetPostById
        [Test]
        [TestCase(1)]
        public async Task GetPostById_WithPostId_ReturnsPost(int postId)
        {
            // Act
            var result = await _postService.GetPostByIdAsync(postId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Message, Is.EqualTo("Post retrieved successfully."));
                Assert.That(result.Data.Content, Is.EqualTo($"Content {postId}"));
            });
        }

        [Test]
        [TestCase(1, "user1")]
        [TestCase(1, "user2")]
        public async Task GetPostById_WithUserId_ReturnsIsFavPost(int postId, string userId)
        {
            // Act
            var result = await _postService.GetPostByIdAsync(postId, userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Message, Is.EqualTo("Post retrieved successfully."));
                Assert.That(result.Data.Content, Is.EqualTo($"Content {postId}"));
                Assert.That(result.Data.IsFav, Is.True);
            });
        }

        [Test]
        [TestCase(2, "user2")]
        public async Task GetPostById_WithUserId_ReturnsIsNotFavAndReactType(int postId, string userId)
        {
            // Act
            var result = await _postService.GetPostByIdAsync(postId, userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.IsFav, Is.False);
                Assert.That(result.Data.UserReact, Is.EqualTo(ReactType.VOTEUP));
            });
        }

        [Test]
        [TestCase(3)]
        [TestCase(0)]
        public async Task GetPostById_WithNonExistentPost_ReturnsNotFound(int postId)
        {
            // Act
            var result = await _postService.GetPostByIdAsync(postId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Data, Is.Not.Null);
            });
        }
        #endregion

        #region GetRecommendedPosts
        [Test]
        [TestCase(2)]
        public async Task GetRecommendedPosts_WithTopN_ReturnsTopPosts(int topN)
        {
            // Act
            var result = await _postService.GetRecommendedPostsAsync(topN);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.Count, Is.EqualTo(topN));
            });
        }

        [Test]
        public async Task GetRecommendedPosts_WithEmptyList_ReturnsNoPostsFound()
        {
            //Arrange
            _context.RemoveRange(_samplePosts);
            _context.SaveChanges();

            // Act
            var result = await _postService.GetRecommendedPostsAsync(1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.Count, Is.EqualTo(0));
            });
        }
        #endregion

        #region SearchPosts (service needs to refactor)
        //[Test]
        //[TestCase("DotNet", null, null, 0, 1)]
        //[TestCase("DotNet", null, null, 1, 1)]
        //public async Task SearchPosts_WithTag_ReturnsTagPosts(string tag, string userName, string keyword, int skip, int take)
        //{
        //    // Act
        //    var result = await _postService.SearchPostsAsync(tag, userName, keyword, skip, take);

        //    // Assert
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(result.IsSuccess, Is.True);
        //        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        //        Assert.That(result.Data, Is.Not.Null);
        //        Assert.That(result.Data.Count, Is.EqualTo(take - skip));
        //    });
        //}

        //[Test]
        //[TestCase(null, "John Doe", null, 0, 1)]
        //public async Task SearchPosts_WithUserName_ReturnsUserPosts(string tag, string userName, string keyword, int skip, int take)
        //{
        //    // Act
        //    var result = await _postService.SearchPostsAsync(tag, userName, keyword, skip, take);

        //    // Assert
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(result.IsSuccess, Is.True);
        //        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        //        Assert.That(result.Data, Is.Not.Null);
        //        Assert.That(result.Data.Count, Is.Not.EqualTo(0));
        //        Assert.That(result.Message, Is.EqualTo("Search completed successfully."));
        //    });
        //} 
        #endregion

        #region CreatePost
        [Test]
        public async Task CreatePost_WithValidData_ReturnsCreatedPost() // will fail
        {
            var post = new ManagePostDTO
            {
                Title = "Test Post",
                Content = "Test Content",
                Tags = new List<string> { "test1", "test2" }
            };

            var result = await _postService.CreatePostAsync("user1", post);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.CreatorName, Is.EqualTo("John Doe"));
                Assert.That(result.Data.Tags![0], Is.EqualTo("test1"));
            });
        }

        [Test]
        [TestCase("", "content test")]
        [TestCase("title test", "")]
        public async Task CreatePost_WithEmptyTitleOrContent_ReturnsBadRequest(string title, string content)
        {
            var post = new ManagePostDTO
            {
                Title = title,
                Content = content,
                Tags = new List<string> { "test1", "test2" }
            };

            var result = await _postService.CreatePostAsync("user1", post);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Message, Is.EqualTo("Title and Content cannot be empty."));
            });
        }

        [Test]
        public async Task CreatePost_WithNonExistentUser_ReturnsNotFound()
        {
            var post = new ManagePostDTO
            {
                Title = "Test Title",
                Content = "Test Content",
                Tags = new List<string> { "test1", "test2" }
            };

            var result = await _postService.CreatePostAsync("user4", post);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Is.EqualTo("User not found."));
            });
        }

        [Test]
        public async Task CreatePost_WithInValidData_ReturnsInvalidData()
        {
            var post = new ManagePostDTO
            {
                Title = "this Test Title cannot be more than 150 characters - " +
                    "this Test Title cannot be more than 150 characters - " +
                    "this Test Title cannot be more than 150 characters",
                Content = "Test Content",
                Tags = new List<string> { "test1", "test2" }
            };

            var result = await _postService.CreatePostAsync("user1", post);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            });
        }
        #endregion

        #region DeletePost
        [Test]
        public async Task DeletePost_WithValidData_ShouldDeletePost()
        {
            var result = await _postService.DeletePostAsync(1, "user1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Is.EqualTo("Post deleted successfully"));
                Assert.That(result.Data, Is.Not.Null);
            });
        }

        [Test]
        public async Task DeletePost_WithNonExistentPost_ReturnsNotFound()
        {
            var result = await _postService.DeletePostAsync(4, "user1");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Is.EqualTo("Post not found"));
            });
        }

        [Test]
        public async Task DeletePost_WithUnAuthorizedUser_ReturnsNotAuthorized()
        {
            var result = await _postService.DeletePostAsync(1, "user2");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
                Assert.That(result.Message, Is.EqualTo("You are not authorized to delete this post"));
            });
        }
        #endregion

        #region EditPost
        [Test]
        public async Task EditPost_WithValidData_ReturnsUpdatedPost() // will fail
        {
            var post = new ManagePostDTO
            {
                Title = "Updated title",
                Content = "Updated content",
                Tags = new List<string> { "addedTag1", "addedTag2" }
            };

            var result = await _postService.EditPostAsync(1, "user1", post);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Is.EqualTo("Post edited successfully."));
                Assert.That(result.Data.CreatorName, Is.EqualTo("John Doe"));
            });
        }

        [Test]
        public async Task EditPost_WithNonExistentPost_ReturnsNotFound()
        {
            var post = new ManagePostDTO
            {
                Title = "Updated title",
                Content = "Updated content",
                Tags = new List<string> { "addedTag1", "addedTag2" }
            };

            var result = await _postService.EditPostAsync(4, "user1", post);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Is.EqualTo("Post not found"));
            });
        }

        [Test]
        public async Task EditPost_WithUnAuthorizedUser_ReturnsUnAuthorized()
        {
            var post = new ManagePostDTO
            {
                Title = "Updated title",
                Content = "Updated content",
                Tags = new List<string> { "addedTag1", "addedTag2" }
            };

            var result = await _postService.EditPostAsync(1, "user4", post);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
                Assert.That(result.Message, Is.EqualTo("You are not authorized to edit this post"));
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
                    }
            };

            _samplePosts = new List<Post>
            {
                new Post
                {
                    Id = _postId++,
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
                    Id = _postId++,
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
                    Id=_favPostId++,
                    SavedAt= DateTime.UtcNow,
                    UserId="user1",
                    PostId=2
                },
                new FavoritePosts
                {
                    Id=_favPostId++,
                    SavedAt= DateTime.UtcNow,
                    UserId="user1",
                    PostId=1
                },
                new FavoritePosts
                {
                    Id=_favPostId++,
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
                    Id=_tagId++,
                    Name="DotNet"
                },

                new Tags
                {
                    Id=_tagId++,
                    Name="GraphQL"
                },

                new Tags
                {
                    Id=_tagId++,
                    Name="PostgreSQL"
                }
            };

            _samplePostTags = new List<PostTags>
            {
                new PostTags
                {
                    Id= _postTagId++,
                    PostId=1,
                    TagId=5
                },
                new PostTags
                {
                    Id= _postTagId++,
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
            _mockUnitOfWork.Setup(uow => uow.Posts.AddAsync(It.IsAny<Post>()))
                .ReturnsAsync((Post entity) => 
                {
                    entity.Creator= _context.Users.Find(entity.CreatorId)!;
                    entity.Id = _postId++;
                    return entity;
                });

            _mockUnitOfWork.Setup(uow => uow.Posts.DeleteAsync(It.IsAny<Post>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(uow => uow.PostTags.AddAsync(It.IsAny<PostTags>()))
                .ReturnsAsync((PostTags entity) =>
                {
                    entity.Id = _postTagId++;
                    return entity;
                });

            _mockUnitOfWork.Setup(uow => uow.PostTags.AddRangeAsync(It.IsAny<List<PostTags>>()))
                .Returns((List<PostTags> entities) =>
                {
                    foreach (var entity in entities)
                    {
                        entity.Id = _postTagId++;
                    }
                    return Task.CompletedTask;
                });

            _mockUnitOfWork.Setup(uow => uow.Tags.AddAsync(It.IsAny<Tags>()))
                .ReturnsAsync((Tags entity) => 
                {
                    entity.Id = _tagId++;
                    return entity;
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
                    var post = _samplePosts.FirstOrDefault(x => x.Id == id);
                    return post!;
                });

        }

        private MapperConfiguration MappingProfiles()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Post, PostDTO>()
                    .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.Creator.ProfilePicture))
                    .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.Creator.FirstName + " " + src.Creator.LastName))
                    .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.PostTags.Select(pt => pt.Tag.Name).ToList()));
            
            });
            return mapperConfig;
        }

        #endregion


    }
}

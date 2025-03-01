using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GreenDonut;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using Optern.Application.DTOs.React;
using Optern.Application.Interfaces.IReactService;
using Optern.Application.Response;
using Optern.Domain.Entities;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Services.ReactService;
using Optern.Infrastructure.UnitOfWork;
using Task = System.Threading.Tasks.Task;

namespace Optern.Test.BlogTest.ReactTests
{
    [TestFixture]
    public class ReactTests
    {
        private  Mock<IUnitOfWork> _mockUnitOfWork;
        private  Mock<IMapper> _mockMapper;
        private  OpternDbContext _context;
        private  IReactService _reactService;
        private  List<Post> _postsData;
        private  List<Reacts> _reactsData;
        private  List<Comment> _commentsData;
        private  List<CommentReacts> _commentReactsData;

        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork= new Mock<IUnitOfWork>();
            _mockMapper= new Mock<IMapper>();

            var options = new DbContextOptionsBuilder<OpternDbContext>()
                .UseInMemoryDatabase(databaseName: "OpternTestDb")
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)) 
                .Options;
            _context = new OpternDbContext(options);
            LoadData();
            _reactService = new ReactService(_mockUnitOfWork.Object, _context, _mockMapper.Object);
        }


        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
             
        }

        #region Manage React on Post
        [Test]
        [Category("ReactPostTests")]
        [TestCase(6, "user5", ReactType.VOTEUP)]
        public async Task ManageReacts_ShouldReturnPostNotFound_WhenPostNotExist(int postId, string userId, ReactType reactType)
        {
            var result = await _reactService.ManageReactAsync(postId, userId, reactType);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False, "Expected IsSuccess To be False When Post Does Not Exist");
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound), "Expected Status To be 404 When Post Not Found");
                Assert.That(result.Data, Is.TypeOf<ReactDTO>(), "Expected Data Will be New Object of ReactDTO When Post Not Found");
            });
        }

        [Test]
        [Category("ReactPostTests")]
        [TestCase(2, "user2", ReactType.VOTEUP)]
        public async Task ManageReacts_ShouldCreateNewReact_WhenReactNotExistOnPost(int postId, string userId, ReactType reactType)
        {

            MapSingleObjectDataWithMock();

            var result = await _reactService.ManageReactAsync(postId, userId, reactType);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.ReactType, Is.EqualTo(reactType));
            });
        }

        [Test]
        [Category("ReactPostTests")]
        [TestCase(3, "user3", ReactType.VOTEUP)]
        [TestCase(1, "user1", ReactType.VOTEDOWN)]
        [TestCase(1, "user2", ReactType.VOTEUP)]
        public async Task ManageReacts_ShouldUpdateReact_WhenReactChangedOnPost(int postId, string userId, ReactType reactType)
        {
            MapSingleObjectDataWithMock();

            var result = await _reactService.ManageReactAsync(postId, userId, reactType);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.ReactType, Is.EqualTo(reactType));
                Assert.That(result.Message, Is.EqualTo("React updated successfully."));
            });

        }

        [Test]
        [Category("ReactPostTests")]
        [TestCase(1, "user1", ReactType.VOTEUP)]
        [TestCase(1, "user2", ReactType.VOTEDOWN)]
        public async Task ManageReacts_ShouldRemoveReact_WhenSameReactExist(int postId, string userId, ReactType reactType)
        {
            MapSingleObjectDataWithMock();
            var result = await _reactService.ManageReactAsync(postId, userId, reactType);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Message, Is.EqualTo("React removed successfully."));
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data.ReactType, Is.EqualTo(ReactType.NOTVOTEYET));
            });

        }
        #endregion

        #region GetReacts for Post
        [Test]
        [Category("GetReactsTests")]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(105)]
        public async Task GetReacts_ShouldReturnPostNotFound_WhenPostNotExist(int postId, ReactType? reactType = null)
        {
            var result = await _reactService.GetReactsAsync(postId);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Data, Is.TypeOf<List<ReactDTO>>());
                Assert.That(result.Data, Is.Empty);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            });
        }

        [Test]
        [Category("GetReactsTests")]
        [TestCase(2)]
        public async Task GetReacts_ShouldReturnEmptyReactList_WhenPostNotContainsReacts(int postId, ReactType? reactType = null)
        {
            var result = await _reactService.GetReactsAsync(postId);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Data, Is.TypeOf<List<ReactDTO>>());
                Assert.That(result.Data, Is.Empty);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
                Assert.That(result.Data.Count, Is.EqualTo(0));
            });
        }

        [Test]
        [Category("GetReactsTests")]
        [TestCase(1)]
        [TestCase(3)]
        public async Task GetReacts_ShouldReturnReactList_WhenPostContainsReacts(int postId, ReactType? reactType = null)
        {

            MapListOfReactsOnPostWithMock();

            var result = await _reactService.GetReactsAsync(postId);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Data, Is.Not.Empty);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data.Count, Is.GreaterThan(0));
                Assert.That(result.Message, Is.EqualTo("Reacts retrieved successfully."));
            });
        }

        [Test]
        [Category("GetReactsTests")]
        [TestCase(1, ReactType.VOTEUP)]
        public async Task GetReacts_ShouldReturnReactListForSpecificReact_WhenPostContainsSpecificReacts(int postId, ReactType? reactType = null)
        {

            MapListOfReactsOnPostWithMock();

            var result = await _reactService.GetReactsAsync(postId, reactType);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Data, Is.Not.Empty);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data.Count, Is.EqualTo(1));
                Assert.That(result.Message, Is.EqualTo("Reacts retrieved successfully."));
            });
        } 

        #endregion

        #region Manage Reacts on Comment
        [Test]
        [Category("ReactCommentsTests")]
        [TestCase(5, "user1", ReactType.VOTEUP)]
        public async Task ManageCommentReact_ShouldReturnCommentNotFound_WhenCommentNotExist(int commentId, string userId, ReactType reactType)
        {
            var result = await _reactService.ManageCommentReactAsync(commentId, userId, reactType);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Is.EqualTo("Comment not found."));
                Assert.That(result, Is.InstanceOf<Response<CommentReactDTO>>());
            });
        }

        [Test]
        [Category("ReactCommentsTests")]
        [TestCase(3, "user1", ReactType.VOTEUP)]
        public async Task ManageCommentReact_ShouldCreateNewReactComment_WhenCommentNotContainsReacts(int commentId, string userId, ReactType reactType)
        {
            var result = await _reactService.ManageCommentReactAsync(commentId, userId, reactType);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
                Assert.That(result.Data.ReactType, Is.EqualTo(reactType));
                Assert.That(result.Data.UserId, Is.EqualTo("user1"));
            });
        }

        [Test]
        [Category("ReactCommentsTests")]
        [TestCase(1, "user1", ReactType.VOTEDOWN)]
        [TestCase(1, "user2", ReactType.VOTEUP)]
        public async Task ManageCommentReact_ShouldUpdateReactComment_WhenReactOnCommentChanges(int commentId, string userId, ReactType reactType)
        {
            var result = await _reactService.ManageCommentReactAsync(commentId, userId, reactType);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Data, Is.InstanceOf<CommentReactDTO>());
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data.UserId, Is.EqualTo(userId));
                Assert.That(result.Data.ReactType, Is.EqualTo(reactType));
                Assert.That(result.Message, Does.Contain("React updated successfully."));
            });
        }
        [Test]
        [Category("ReactCommentsTests")]
        [TestCase(1, "user1", ReactType.VOTEUP)]
        [TestCase(1, "user2", ReactType.VOTEDOWN)]
        public async Task ManageCommentReact_ShouldRemoveReactComment_WhenSameReactPutOnComment(int commentId, string userId, ReactType reactType)
        {
            var result = await _reactService.ManageCommentReactAsync(commentId, userId, reactType);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.InstanceOf<CommentReactDTO>());
                Assert.That(result.Data.UserId, Is.EqualTo(userId));
                Assert.That(result.Data.ReactType, Is.EqualTo(reactType));
                Assert.That(result.Message, Does.Contain("React removed successfully."));
            });

        }

        [Test]
        [Category("ReactCommentsTests")]
        public async Task ManageCommentReact_ShouldThrowException_WhenExceptionOccure()
        {
            int commentId = 1;
            string userId = null; 
            ReactType reactType = ReactType.VOTEUP;

            var result = await _reactService.ManageCommentReactAsync(commentId, userId, reactType);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Does.Contain("An error occurred while processing the comment react"));
        }
        #endregion

        #region Get Reacts On Comment
        [Test]
        [Category("GetReactsOnCommentsTests")]
        [TestCase(5)]
        public async Task GetCommentReacts_ShouldReturnCommentNotFound_WhenReactOnCommentNotExist(int commentId, ReactType? reactType = null)
        {
            var result = await _reactService.GetCommentReactsAsync(commentId);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Data, Is.TypeOf<List<CommentReactDTO>>());
                Assert.That(result.Data, Is.Empty);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Message, Does.Contain("Comment not found."));

            });
        }
        [Test]
        [Category("GetReactsOnCommentsTests")]
        [TestCase(3)]
        public async Task GetCommentReacts_ShouldReturnEmptyReactList_WhenCommentNotContainsReacts(int commentId, ReactType? reactType = null)
        {
            var result = await _reactService.GetCommentReactsAsync(commentId);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Data, Is.TypeOf<List<CommentReactDTO>>());
                Assert.That(result.Data, Has.Exactly(0).Items);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
                Assert.That(result.Message, Does.Contain("No reacts found for the specified criteria."));
            });
        }

        [Test]
        [Category("GetReactsOnCommentsTests")]
        [TestCase(1)]
        public async Task GetCommentReacts_ShouldReturnReactList_WhenCommentContainsReacts(int commentId, ReactType? reactType = null)
        {
            MapListOfReactsOnCommentWithMock();
            var result = await _reactService.GetCommentReactsAsync(commentId);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Data.Count, Is.GreaterThan(0));
                Assert.That(result.Data, Has.Exactly(2).Items);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Does.Contain("Comment reacts retrieved successfully."));
            });
        }

        [Test]
        [Category("GetReactsOnCommentsTests")]
        [TestCase(1, ReactType.VOTEDOWN)]
        public async Task GetCommentReacts_ShouldReturnReactList_WhenCommentContainsSpecificReactType(int commentId, ReactType? reactType = null)
        {
            MapListOfReactsOnCommentWithMock();
            var result = await _reactService.GetCommentReactsAsync(commentId, reactType);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Data.Count, Is.GreaterThan(0));
                Assert.That(result.Data, Has.Exactly(1).Items);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Message, Does.Contain("Comment reacts retrieved successfully."));
            });
        } 
        #endregion

        #region Map Data
        private void MapSingleObjectDataWithMock()
        {
            _mockMapper.Setup(m => m.Map<ReactDTO>(It.IsAny<Reacts>()))
                .Returns((Reacts src) => new ReactDTO
                {
                    Id = src.Id,
                    ReactType = src.ReactType,
                    ReactDate = src.ReactDate,
                    UserId = src.UserId,
                    UserName = src.User?.FirstName + " " + src.User?.LastName,

                });
        }

        private void MapListOfReactsOnPostWithMock()
        {
            _mockMapper.Setup(m => m.Map<List<ReactDTO>>(It.IsAny<List<Reacts>>()))
                .Returns((List<Reacts> src) => src.Select(react => new ReactDTO
                {
                    Id = react.Id,
                    ReactType = react.ReactType,
                    ReactDate = react.ReactDate,
                    UserId = react.UserId,
                    UserName = react.User?.FirstName + " " + react.User?.LastName,
                }).ToList());
        }

        private void MapListOfReactsOnCommentWithMock()
        {
            _mockMapper.Setup(m => m.Map<List<CommentReactDTO>>(It.IsAny<List<CommentReacts>>()))
                .Returns((List<CommentReacts> src) => src.Select(react => new CommentReactDTO()
                {
                    Id = react.Id,
                    ReactType = react.ReactType,
                    UserId = react.UserId,
                    UserName = react.User?.FirstName + " " + react.User?.LastName,
                }).ToList());
        }
        #endregion

        #region Load Data


        private void LoadData()
        {

            _postsData = new List<Post>()
            {
                new Post()
                {
                    Id = 1,
                    Title = "Introduction to C#",
                    Content = "C# is a modern, object-oriented programming language developed by Microsoft...",
                    ContentType = ContentType.Others,
                    CreatedDate = DateTime.Now.AddDays(-10),
                    EditedDate = DateTime.Now.AddDays(-5),
                    CreatorId = "user1",
                    Creator = new ApplicationUser
                    {
                        Id = "user1", UserName = "JohnDoe", Email = "user1@gmail.com", FirstName = "user",
                        LastName = "1"
                    },
                    Reacts = new List<Reacts>(),
                    Comments = new List<Comment>(),
                    FavoritePosts = new List<FavoritePosts>(),
                    PostTags = new List<PostTags>()
                },
                new Post()
                {
                    Id = 2,
                    Title = "Getting Started with ASP.NET Core",
                    Content =
                        "ASP.NET Core is a cross-platform, high-performance framework for building modern applications...",
                    ContentType = ContentType.Link,
                    CreatedDate = DateTime.Now.AddDays(-8),
                    EditedDate = DateTime.Now.AddDays(-3),
                    CreatorId = "user2",
                    Creator = new ApplicationUser
                    {
                        Id = "user2", UserName = "JaneSmith", Email = "user2@gmail.com", FirstName = "user",
                        LastName = "2"
                    },
                    Reacts = new List<Reacts>(),
                    Comments = new List<Comment>(),
                    FavoritePosts = new List<FavoritePosts>(),
                    PostTags = new List<PostTags>()
                },
                new Post()
                {
                    Id = 3,
                    Title = "Understanding Dependency Injection",
                    Content =
                        "Dependency Injection (DI) is a design pattern used in .NET to achieve Inversion of Control (IoC)...",
                    ContentType = ContentType.Image,
                    CreatedDate = DateTime.Now.AddDays(-15),
                    EditedDate = DateTime.Now.AddDays(-7),
                    CreatorId = "user3",
                    Creator = new ApplicationUser
                    {
                        Id = "user3", UserName = "MichaelBrown", Email = "user3@gmail.com", FirstName = "user",
                        LastName = "3"
                    },
                    Reacts = new List<Reacts>(),
                    Comments = new List<Comment>(),
                    FavoritePosts = new List<FavoritePosts>(),
                    PostTags = new List<PostTags>()
                },
            };
            _reactsData = new List<Reacts>()
            {
                new Reacts()
                {
                    Id = 1,
                    ReactDate = DateTime.UtcNow.AddDays(-2),
                    ReactType = ReactType.VOTEUP,
                    UserId = "user1",
                    PostId = 1
                },
                new Reacts()
                {
                    Id = 2,
                    ReactDate = DateTime.UtcNow.AddDays(-1),
                    ReactType = ReactType.VOTEDOWN,
                    UserId = "user2",
                    PostId = 1
                },
                new Reacts()
                {
                    Id = 3,
                    ReactDate = DateTime.UtcNow.AddHours(-5),
                    ReactType = ReactType.NOTVOTEYET,
                    UserId = "user3",
                    PostId = 3
                }
            };

            _commentsData = new List<Comment>()
            {
                new Comment()
                {
                    Id = 1,
                    Content = "This is a great post about C#!",
                    CommentDate = DateTime.UtcNow.AddDays(-3),
                    Type = ContentType.Others,
                    UserId = "user2",
                    PostId = 1,
                    ParentId = null,  
                 
                },
                new Comment()
                {
                    Id = 2,
                    Content = "I agree, C# is very powerful!",
                    CommentDate = DateTime.UtcNow.AddDays(-2),
                    Type = ContentType.Text,
                    UserId = "user3",
                    PostId = 1,
                    ParentId = 1,  
                 
                },
                new Comment()
                {
                    Id = 3,
                    Content = "What do you think about C# performance compared to Java?",
                    CommentDate = DateTime.UtcNow.AddDays(-1),
                    Type = ContentType.Link,
                    UserId = "user3",
                    PostId = 1,
                    ParentId = null, 
                   
                }
            };
            _commentReactsData = new List<CommentReacts>
            {
                new CommentReacts
                {
                    Id = 1,
                    ReactType = ReactType.VOTEUP,
                    CommentId = 1,
                    UserId = "user1",
                },
                new CommentReacts
                {
                    Id = 2,
                    ReactType = ReactType.VOTEDOWN,
                    CommentId = 1,
                    UserId = "user2",
                },
                new CommentReacts
                {
                    Id = 3,
                    ReactType = ReactType.NOTVOTEYET,
                    CommentId = 2,
                    UserId = "user3",
                },
                new CommentReacts
                {
                    Id = 4,
                    ReactType = ReactType.VOTEUP,
                    CommentId = 2,
                    UserId = "user4",
                }
            };

            _context.CommentReacts.AddRange(_commentReactsData);
            _context.Comments.AddRange(_commentsData);
            _context.Posts.AddRange(_postsData);
            _context.Reacts.AddRange(_reactsData);
            _context.SaveChanges();
        } 

        #endregion
    }
}

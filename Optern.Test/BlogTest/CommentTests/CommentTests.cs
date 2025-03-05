using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using Optern.Application.DTOs.Comment;
using Optern.Application.Interfaces.ICommentService;
using Optern.Application.Interfaces.IUserService;
using Optern.Domain.Entities;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Services.CommentService;
using Optern.Infrastructure.UnitOfWork;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Task = System.Threading.Tasks.Task;

namespace Optern.Test.BlogTest.CommentTests
{
    [TestFixture]
    public class CommentTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private OpternDbContext _context;
        private ICommentService _commentService;
        private List<Comment> _commentsData;
        private List<CommentReacts> _commentReactsData;
        private List<ApplicationUser> _users;
        private List<Post> _PostsData;


        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            
            var options = new DbContextOptionsBuilder<OpternDbContext>()
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .UseInMemoryDatabase(databaseName: "OpternTestDb")
                .Options;
            _context = new OpternDbContext(options);
            LoadData();
            _commentService = new CommentService(_mockUnitOfWork.Object,_context);
    
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();

        }

        [Test]
        [Category("GetRepliesForCommentTests")]
        [TestCase(9,"user1")]
        [TestCase(12,"user1")]
        [TestCase(120,"user1")]
        public async Task GetRepliesForComment_ShouldReturnCommentNotFound_WhenTargetCommentNotFound(int commentId, string userId)
        {
            var result = await _commentService.GetRepliesForCommentAsync(commentId, userId);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess,Is.False);
                Assert.That(result.Data,Has.Exactly(0).Items);
                Assert.That(result.StatusCode,Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Data,Is.InstanceOf<List<CommentDTO>>());
            });
        }

        [Test]
        [Category("GetRepliesForCommentTests")]
        [TestCase(1, null)]
        [TestCase(1, "user1")]
        public async Task GetRepliesForComment_ShouldReturnAllRepliesForComments_WhenTargetCommentContainsReplies(int commentId, string userId)
        {
            var result = await _commentService.GetRepliesForCommentAsync(commentId, userId);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess,Is.True);
                Assert.That(result.StatusCode,Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data.Count,Is.GreaterThan(0));
                Assert.That(result.Data,Has.Exactly(2).Items);
                Assert.That(result.Message,Does.Contain("Replies for the comment fetched successfully."));

            });
        }

        [Test]
        [Category("GetRepliesForCommentTests")]
        [TestCase(3, null)]
        public async Task GetRepliesForComment_ShouldReturnEmptyList_WhenTargetCommentNotContainsReplies(int commentId, string userId)
        {
            var result = await _commentService.GetRepliesForCommentAsync(commentId, userId);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data.Count, Is.EqualTo(0));
                Assert.That(result.Data, Has.Exactly(0).Items);
                Assert.That(result.Message, Does.Contain("Replies for the comment fetched successfully."));

            });
        }

        [Test]
        [Category("AddComment")]
        public async Task AddComment_ShouldReturnCommentAdded_WhenDataIsValid()
        {
            var userId = "user1";
            var input = new AddCommentInputDTO()
            {
                PostId = 1,
                Content = "Test"
            };
            var result = await _commentService.AddCommentAsync(input, userId);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess,Is.True);
                Assert.That(result.StatusCode,Is.EqualTo(StatusCodes.Status201Created));
                Assert.That(result.Data.Content,Is.EqualTo(input.Content));
                Assert.That(result.Message,Does.Contain("Comment added successfully."));
            });
        }

        [Test]
        [Category("AddComment")]
        public async Task AddComment_ShouldReturnUserNotFound_WhenUserNotExist()
        {
            var userId = "user105";
            var input = new AddCommentInputDTO()
            {
                PostId = 1,
                Content = "Test"
            };
            var result = await _commentService.AddCommentAsync(input, userId);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Data, Is.InstanceOf<CommentDTO>());
                Assert.That(result.Message, Does.Contain("User Not Found"));
            });
        }

        [Test]
        [Category("AddComment")]
        public async Task AddComment_ShouldReturnInvalidData_WhenDataIsNotValid()
        {
            var userId =string.Empty;
            var input = new AddCommentInputDTO()
            {
                PostId = 1,
                Content = "Test"
            };
            var result = await _commentService.AddCommentAsync(input, userId);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Data, Is.InstanceOf<CommentDTO>());
                Assert.That(result.Message, Does.Contain("Invalid Data"));
            });
        }

        [Test]
        [Category("AddReplyToComment")]
        public async Task AddReply_ShouldReturnCommentNotFound_WhenParentCommentNotExist()
        {
            AddReplyInputDTO input = new AddReplyInputDTO()
            {
                Content = "TestComment",
                PostId = 1,
                ParentId = 50
            };
            var userId = "user1";
            var result = await _commentService.AddReplyAsync(input, userId);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Data, Is.InstanceOf<CommentDTO>());
                Assert.That(result.Message, Does.Contain("Parent comment not found"));
            });
        }

        [Test]
        [Category("AddReplyToComment")]
        public async Task AddReply_ShouldAddNewReplyOnComment_WhenDataIsValid()
        {
            AddReplyInputDTO input = new AddReplyInputDTO()
            {
                Content = "TestComment",
                PostId = 1,
                ParentId = 1
            };
            var userId = "user1";
            var result = await _commentService.AddReplyAsync(input, userId);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
                Assert.That(result.Data.Content, Is.EqualTo(input.Content));
                Assert.That(result.Message, Does.Contain("Reply added successfully."));
            });
        }

        [Test]
        [Category("AddReplyToComment")]
        public async Task AddReply_ShouldReturnInvalidData_WhenDataIsInValid()
        {
            AddReplyInputDTO input = new AddReplyInputDTO()
            {
                Content = "TestComment",
                PostId = 1,
                ParentId = 50
            };
            var userId = string.Empty;
            var result = await _commentService.AddReplyAsync(input, userId);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Data, Is.InstanceOf<CommentDTO>());
                Assert.That(result.Message, Does.Contain("Invalid Data"));
            });
        }

        [Test]
        [Category("UpdateComment")]
        public async Task UpdateComment_ShouldReturnCommentNotFound_WhenCommentNotExist()
        {
            var input = new UpdateCommentInputDTO()
            {
                UpdatedContent = "updated Content for Comment not Exist",
            };
            int commentId = 50;
            var result = await _commentService.UpdateCommentAsync(commentId, input);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Data, Is.InstanceOf<CommentDTO>());
                Assert.That(result.Message, Does.Contain("Comment not found."));
            });
        }


        [Test]
        [Category("UpdateComment")]
        public async Task UpdateComment_ShouldUpdateComment_WhenCommentExist()
        {
            var input = new UpdateCommentInputDTO()
            {
                UpdatedContent = "updated Content for Comment Exist",
            };
            int commentId = 1;
            var result = await _commentService.UpdateCommentAsync(commentId, input);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data.Content, Is.SameAs(input.UpdatedContent));
                Assert.That(result.Message, Does.Contain("Comment updated successfully."));
            });
        }

        [Test]
        [Category("DeleteComment")]
        [TestCase(50)]
        [TestCase(150)]
        [TestCase(106)]
        public async Task DeleteComment_ShouldReturnCommentNotFound_WhenCommentNotExist(int commentId)
        {
            var result = await _commentService.DeleteCommentAsync(commentId);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(result.Data, Is.InstanceOf<bool>());
                Assert.That(result.Message, Does.Contain("Comment not found."));
            });
        }

        [Test]
        [Category("DeleteComment")]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task DeleteComment_ShouldDeleteComment_WhenCommentExist(int commentId)
        {
            var result = await _commentService.DeleteCommentAsync(commentId);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data, Is.True);
                Assert.That(result.Message, Does.Contain("Comment deleted successfully."));
            });
        }
        private void LoadData()
        {

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

                },
                new Comment()
                {
                    Id =4,
                    Content = "What do you think about C# performance compared to Java?",
                    CommentDate = DateTime.UtcNow.AddDays(-1),
                    Type = ContentType.Link,
                    UserId = "user3",
                    PostId = 1,
                    ParentId = 1,

                },
                new Comment()
                {
                    Id = 5,
                    Content = "What do you think about C# performance compared to Java?",
                    CommentDate = DateTime.UtcNow.AddDays(-1),
                    Type = ContentType.Link,
                    UserId = "user3",
                    PostId = 1,
                    ParentId = 2,

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
            _users = new List<ApplicationUser>()
            {

                new ApplicationUser { Id = "user1", FirstName = "Loay", LastName = "Doe", ProfilePicture = "Loay.png",Email = "user1@gmail.com"},
                new ApplicationUser { Id = "user2", FirstName = "John", LastName = "Doe", ProfilePicture = "john.png",Email = "user2@gmail.com" },
                new ApplicationUser { Id = "user3", FirstName = "Alice", LastName = "Smith", ProfilePicture = "alice.png",Email = "user3@gmail.com" },
                new ApplicationUser { Id = "user4", FirstName = "Bob", LastName = "Brown", ProfilePicture = "bob.png",Email = "user4@gmail.com" }

            };
            _PostsData = new List<Post>
            {
                new Post
                {
                    Id = 1,
                    Title = "Post 1",
                    Content = "Content 1",
                   CreatorId = "user1",
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
                    CreatorId = "user2",
                    CreatedDate = DateTime.UtcNow.AddMinutes(-15),
                    PostTags = new List<PostTags>(),
                    Reacts = new List<Reacts>(),
                    Comments = new List<Comment>()
                }
            };
            _context.Users.AddRange(_users);
            _context.Posts.AddRange(_PostsData);
            _context.CommentReacts.AddRange(_commentReactsData);
            _context.Comments.AddRange(_commentsData);
            _context.SaveChanges();
        }
    }
}

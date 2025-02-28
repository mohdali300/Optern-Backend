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
using Optern.Application.DTOs.React;
using Optern.Application.Interfaces.IReactService;
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
        Creator = new ApplicationUser { Id = "user1", UserName = "JohnDoe" ,Email = "user1@gmail.com",FirstName = "user",LastName = "1"},
        Reacts = new List<Reacts>(),
        Comments = new List<Comment>(),
        FavoritePosts = new List<FavoritePosts>(),
        PostTags = new List<PostTags>()
    },
    new Post()
    {
        Id = 2,
        Title = "Getting Started with ASP.NET Core",
        Content = "ASP.NET Core is a cross-platform, high-performance framework for building modern applications...",
        ContentType = ContentType.Link,
        CreatedDate = DateTime.Now.AddDays(-8),
        EditedDate = DateTime.Now.AddDays(-3),
        CreatorId = "user2",
        Creator = new ApplicationUser { Id = "user2", UserName = "JaneSmith",Email = "user2@gmail.com",FirstName = "user", LastName = "2" },
        Reacts = new List<Reacts>(),
        Comments = new List<Comment>(),
        FavoritePosts = new List<FavoritePosts>(),
        PostTags = new List<PostTags>()
    },
    new Post()
    {
        Id = 3,
        Title = "Understanding Dependency Injection",
        Content = "Dependency Injection (DI) is a design pattern used in .NET to achieve Inversion of Control (IoC)...",
        ContentType = ContentType.Image,
        CreatedDate = DateTime.Now.AddDays(-15),
        EditedDate = DateTime.Now.AddDays(-7),
        CreatorId = "user3",
        Creator = new ApplicationUser { Id = "user3", UserName = "MichaelBrown",Email = "user3@gmail.com",FirstName = "user",LastName = "3" },
        Reacts = new List<Reacts>(),
        Comments = new List<Comment>(),
        FavoritePosts = new List<FavoritePosts>(),
        PostTags = new List<PostTags>()
    },
    new Post()
    {
        Id = 4,
        Title = "LINQ Basics in C#",
        Content = "LINQ (Language Integrated Query) is a powerful feature in C# that allows querying collections easily...",
        ContentType = ContentType.Text,
        CreatedDate = DateTime.Now.AddDays(-12),
        EditedDate = DateTime.Now.AddDays(-6),
        CreatorId = "user4",
        Creator = new ApplicationUser { Id = "user4", UserName = "EmilyClark" ,Email = "user4@gmail.com",FirstName = "user",LastName = "4"},
        Reacts = new List<Reacts>(),
        Comments = new List<Comment>(),
        FavoritePosts = new List<FavoritePosts>(),
        PostTags = new List<PostTags>()
    },
    new Post()
    {
        Id = 5,
        Title = "Exploring Entity Framework Core",
        Content = "Entity Framework Core is an ORM (Object-Relational Mapper) for .NET applications...",
        ContentType = ContentType.Text,
        CreatedDate = DateTime.Now.AddDays(-20),
        EditedDate = DateTime.Now.AddDays(-10),
        CreatorId = "user5",
        Creator = new ApplicationUser { Id = "user5", UserName = "DavidWilson",Email = "user5@gmail.com",FirstName = "user",LastName = "5"},
        Reacts = new List<Reacts>(),
        Comments = new List<Comment>(),
        FavoritePosts = new List<FavoritePosts>(),
        PostTags = new List<PostTags>()
    }
};
             _reactsData = new List<Reacts>()
            {
                new Reacts()
                {
                    Id = 1,
                    ReactDate = DateTime.UtcNow.AddDays(-2),
                    UserId = "user1",
                    PostId = 1
                },
                new Reacts()
                {
                    Id = 2,
                    ReactDate = DateTime.UtcNow.AddDays(-1),
                    ReactType = ReactType.VOTEDOWN,
                    UserId = "user2",
                    PostId = 2
                },
                new Reacts()
                {
                    Id = 3,
                    ReactDate = DateTime.UtcNow.AddHours(-5),
                    ReactType = ReactType.VOTEDOWN,
                    UserId = "user3",
                    PostId = 3
                },
                new Reacts()
                {
                    Id = 4,
                    ReactDate = DateTime.UtcNow.AddMinutes(-30),
                    ReactType = ReactType.NOTVOTEYET,
                    UserId = "user4",
                    PostId = 4
                },
                new Reacts()
                {
                    Id = 5,
                    ReactDate = DateTime.UtcNow,
                    UserId = "user5",
                    PostId = 5
                }
            };

            _context.Posts.AddRange(_postsData);
            _context.Reacts.AddRange(_reactsData);
            _context.SaveChanges();

            _reactService = new ReactService(_mockUnitOfWork.Object, _context, _mockMapper.Object);
        }


        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();

        }

        [Test]
        [Category("ReactTests")]
        [TestCase(6, "user5", ReactType.VOTEUP)] 
        [TestCase(2, "user5", ReactType.VOTEDOWN)] 
        public async Task ManageReacts_ShouldReturnPostNotFound_WhenPostNotExist(int postId, string userId, ReactType reactType)
        {
            var result = await _reactService.ManageReactAsync(postId, userId, reactType);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess,Is.False, "Expected IsSuccess To be False When Post Does Not Exist");
                Assert.That(result.StatusCode,Is.EqualTo(StatusCodes.Status404NotFound),"Expected Status To be 404 When Post Not Found");
                Assert.That(result.Data, Is.TypeOf<ReactDTO>(),"Expected Data Will be New Object of ReactDTO When Post Not Found"); 
            });
        }

        [Test]
        [Category("ReactTests")]
        [TestCase(1,"user1", ReactType.VOTEUP)]
        [TestCase(2,"user2", ReactType.VOTEDOWN)]
        public async Task ManageReacts_ShouldCreateNewReact_WhenReactNotExistOnPost(int postId, string userId, ReactType reactType)
        {
            _mockMapper.Setup(m => m.Map<ReactDTO>(It.IsAny<Reacts>()))
                .Returns(new ReactDTO());

            var result = await _reactService.ManageReactAsync(postId, userId, reactType);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess,Is.True); 
                Assert.That(result.StatusCode,Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Data,Is.Not.Null);
                Assert.That(result.Data.ReactType,Is.EqualTo(reactType));
            });
        }


    }
}

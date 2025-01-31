
// Program.cs
global using AppAny.HotChocolate.FluentValidation;
global using FluentValidation;
global using Optern.Infrastructure.DependencyInjection;
global using Optern.Infrastructure.Hubs;
global using Optern.Infrastructure.Validations;
global using Optern.Presentation.GraphQlApi.BookMarkedTask.Mutation;
global using Optern.Presentation.GraphQlApi.BookMarkedTask.Query;
// GraphQlAPI/Auth/
global using Optern.Application.DTOs.Login;
global using Optern.Application.DTOs.ResetPassword;
global using Optern.Application.DTOS.Register;
global using Optern.Application.Helpers;
global using Optern.Application.Interfaces.IAuthService;
global using Optern.Domain.Enums;
global using Optern.Infrastructure.ExternalDTOs.LoginForJWT;
global using Optern.Infrastructure.ExternalDTOs.Refresh_Token;
global using Optern.Infrastructure.ExternalInterfaces.IJWTService;
global using Optern.Infrastructure.ExternalServices.BackgroundJobs;
global using Optern.Infrastructure.ExternalServices.UserCleanUp;
global using Optern.Infrastructure.Response;
global using Org.BouncyCastle.Asn1.Ocsp;
global using Optern.Application.Interfaces.IRoomService;
global using Optern.Domain.Entities;

// GraphQlAPI/Comment
global using Optern.Application.DTOs.Comment;
global using Optern.Application.Interfaces.ICommentService;
global using Optern.Application.DTOs.Post;

// GraphQLAPI/FavoritePost
global using Optern.Application.DTOs.FavoritePosts;
global using Optern.Application.DTOs.Room;
global using Optern.Application.Interfaces.IFavoritePostsService;
global using Optern.Application.Interfaces.IPostService;
// GraphQlAPI/Post
global using Optern.Application.DTOs.React;
global using Optern.Application.DTOs.Track;

//GraphQlAPI/React
global using Optern.Application.Interfaces.IReactService;

//GraphQLAPI/RoomTrack
global using Optern.Application.Interfaces.IRoomTrackService;
// GraphQLAPI/RoomUser
global using Optern.Application.DTOs.RoomUser;
global using Optern.Application.Interfaces.IRoomUserService;
global using Optern.Application.Interfaces.IRoomSettingService;

// GraphQlAPI/Sprint
global using Optern.Application.DTOs.Sprint;
global using Optern.Application.Interfaces.ISprintService;

// GraphQLAPI/SubTrack
global using Optern.Application.DTOs.SubTrack;
global using Optern.Application.Interfaces.ISubTrackService;
//GraphQlAPI/Tag
global using Optern.Application.DTOs.Tags;
global using Optern.Application.Interfaces.ITagService;
// GraphQL/Task
global using Optern.Application.DTOs.Task;
global using Optern.Application.Interfaces.ITaskService;

//GraphQl/Track
global using Optern.Application.Interfaces.ITrackService;
// GraphQlAPi/WorkSpace
global using Optern.Application.DTOs.WorkSpace;
global using Optern.Application.Interfaces.IWorkSpaceService;

// GraphQlAPi/BookMarkedTask
global using Optern.Application.DTOs.BookMarkedTask;
global using Optern.Application.Interfaces.IBookMarkedTaskService;

// Hangfire
global using Hangfire;














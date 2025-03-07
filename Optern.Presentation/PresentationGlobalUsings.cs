
// Program.cs
global using Optern.Presentation.GraphQlApi.BookMarkedTask.Mutation;
global using Optern.Presentation.GraphQlApi.BookMarkedTask.Query;

global using AppAny.HotChocolate.FluentValidation;
global using FluentValidation;
global using Optern.Infrastructure.DependencyInjection;
global using Optern.Infrastructure.ExternalServices.BackgroundJobs;
global using Optern.Infrastructure.Hubs;
global using Optern.Infrastructure.Validations;
// GraphQlAPI/Auth/
global using Optern.Application.DTOs.Login;
global using Optern.Application.DTOs.ResetPassword;
global using Optern.Application.DTOS.Register;
global using Optern.Application.Interfaces.IAuthService;
global using Optern.Domain.Enums;
global using Optern.Application.Interfaces.IRoomService;

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

//GraphQLAPI/RoomPosition
global using Optern.Application.Interfaces.IRoomTrackService;
// GraphQLAPI/RoomUser
global using Optern.Application.DTOs.RoomUser;
global using Optern.Application.Interfaces.IRoomUserService;
global using Optern.Application.Interfaces.IRoomSettingService;

// GraphQlAPI/Sprint
global using Optern.Application.DTOs.Sprint;
global using Optern.Application.Interfaces.ISprintService;

// GraphQLAPI/Position
global using Optern.Application.DTOs.Position;
global using Optern.Application.Interfaces.IPositionService;
//GraphQlAPI/Tag
global using Optern.Application.DTOs.Tags;
global using Optern.Application.Interfaces.ITagService;
// GraphQL/Task
global using Optern.Application.DTOs.Task;
global using Optern.Application.Interfaces.ITaskService;
global using Optern.Presentation.GraphQlApi.TaskActivity.Mutation;
global using Optern.Presentation.GraphQlApi.TaskActivity.Query;


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

//GraphQLAPI/RepositoryFile

global using Optern.Application.DTOs.RepositoryFile;
global using Optern.Application.Interfaces.IRepositoryFileService;

// GraphQLAPI/Skill
global using Optern.Application.Interfaces.ISkillService;

global using Optern.Presentation.GraphQlApi.Chat.Query;
global using Optern.Application.Interfaces.IChatService;

global using Optern.Application.Response;



global using Optern.Application.DTOs.LoginForJWT;
global using Optern.Infrastructure.ExternalDTOs.Refresh_Token;
global using Optern.Infrastructure.ExternalInterfaces.IJWTService;
global using Optern.Infrastructure.Helpers;
global using Optern.Domain.Entities;
global using Optern.Presentation.GraphQlApi.ExternalAuth.GoogleAuth.Mutation;
global using Optern.Presentation.GraphQlApi.ExternalAuth.GoogleAuth.Query;
global using Optern.Infrastructure.MiddleWares;


global using Serilog;
global using Optern.Presentation.GraphQlApi.Message.Mutation;
//GraphQLAPI/ Notification

global using Optern.Presentation.GraphQlApi.Notification.Mutation;
global using Optern.Presentation.GraphQlApi.UserNotification.Mutation;
global using Optern.Presentation.GraphQlApi.UserNotification.Query;
global using Optern.Application.DTOs.UserNotification;
global using Optern.Application.Interfaces.IUserNotificationService;
global using Optern.Presentation.GraphQlApi.Message.Query;

// PTPFeedback
global using Optern.Presentation.GraphQlApi.PTPFeedback.Mutation;
global using Optern.Application.DTOs.PTPFeedback;
global using Optern.Application.Interfaces.IPTPFeedbackService;
global using Optern.Presentation.GraphQlApi.PTPFeedback.Query;

// PTPInterview
global using Optern.Presentation.GraphQlApi.PTPInterview.Query;
global using Optern.Application.DTOs.PTPInterview;
global using Optern.Application.Interfaces.IPTPInterviewService;
global using Optern.Presentation.GraphQlApi.PTPInterview.Mutation;

// Profile
global using Optern.Presentation.GraphQlApi.Education.Mutation;
global using Optern.Presentation.GraphQlApi.User.Mutation;
global using Optern.Presentation.GraphQlApi.Education.Query;
global using Optern.Presentation.GraphQlApi.UserSkills.Mutation;
global using Optern.Presentation.GraphQlApi.UserSkills.Query;
global using Optern.Presentation.GraphQlApi.Skill.Mutation;
global using Optern.Application.DTOs.Skills;
global using Optern.Application.Interfaces.IUserSkillsService;

















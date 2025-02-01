using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Optern.Application.Helpers;
using Optern.Application.Interfaces.IAuthService;
using Optern.Application.Interfaces.IBookMarkedTaskService;
using Optern.Application.Interfaces.ICommentService;
using Optern.Application.Interfaces.IFavoritePostsService;
using Optern.Application.Interfaces.IPostService;
using Optern.Application.Interfaces.IReactService;
using Optern.Application.Interfaces.IRoomService;
using Optern.Application.Interfaces.IRoomTrackService;
using Optern.Application.Interfaces.ISprintService;
using Optern.Application.Interfaces.IRoomUserService;
using Optern.Application.Interfaces.ISubTrackService;
using Optern.Application.Interfaces.ITagService;
using Optern.Application.Interfaces.ITrackService;
using Optern.Application.Interfaces.IUserService;
using Optern.Application.Interfaces.IWorkSpaceService;
using Optern.Application.Mappings;
using Optern.Application.Services.AuthService;
using Optern.Application.Services.BookMarkedTaskService;
using Optern.Application.Services.CommentService;
using Optern.Application.Services.FavoritePostsService;
using Optern.Application.Services.PostService;
using Optern.Application.Services.ReactService;
using Optern.Application.Services.RoomService;
using Optern.Application.Services.RoomTrackService;
using Optern.Application.Services.SprintService;
using Optern.Application.Services.RoomUserService;
using Optern.Application.Services.SubTrackService;
using Optern.Application.Services.TagService;
using Optern.Application.Services.TrackService;
using Optern.Application.Services.UserService;
using Optern.Application.Services.WorkSpaceService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.ExternalDTOs.Mail;
using Optern.Infrastructure.ExternalInterfaces.ICacheService;
using Optern.Infrastructure.ExternalInterfaces.IFileService;
using Optern.Infrastructure.ExternalInterfaces.IJWTService;
using Optern.Infrastructure.ExternalInterfaces.IMail;
using Optern.Infrastructure.ExternalServices.BackgroundJobs;
using Optern.Infrastructure.ExternalServices.CacheService;
using Optern.Infrastructure.ExternalServices.FileService;
using Optern.Infrastructure.ExternalServices.JWTService;
using Optern.Infrastructure.ExternalServices.MailService;
using Optern.Infrastructure.ExternalServices.UserCleanUp;
using Optern.Infrastructure.UnitOfWork;
using Optern.Application.Interfaces.ITaskService;
using Optern.Application.Services.TaskService;
using Optern.Application.Interfaces.IRoomSettingService;
using Optern.Application.Services.RoomSettings;

namespace Optern.Infrastructure.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            // Add Postgresql Setting
            services.AddDbContext<OpternDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            );

            // Add Hangifire
            services.AddHangfire(h => h.UsePostgreSqlStorage(configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();
            services.AddScoped<UserCleanUpService>();
            services.AddTransient<UserCleanUpJob>();

            // Add Redis Cache
            services.AddStackExchangeRedisCache(option =>
            {
                option.Configuration = configuration.GetConnectionString("Redis");
                option.InstanceName = "Optern";
            });

            // DI for Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<OpternDbContext>()
                .AddDefaultTokenProviders();

            // Mail settings
            var emailConfig = services.Configure<MailSettingsDTO>(configuration.GetSection("MailSettings"));
            services.AddSingleton(emailConfig);
            services.AddTransient<IMailService, MailService>();

            // Auto Mapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // Custom injection for External services
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<OTP>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();



            // Dependency Injection for Application Services
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITagsService, TagsService>();
            services.AddScoped<ITrackService, TrackService>();
            services.AddScoped<ISubTrackService, SubTrackService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IFavoritePostsService, FavoritePostsService>();
            services.AddScoped<IRoomTrackService, RoomTrackService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IReactService, ReactService>();
            services.AddScoped<IWorkSpaceService, WorkSpaceService>();
            services.AddScoped<ISprintService, SprintService>();
            services.AddScoped<IRoomUserService, RoomUserService>();
            services.AddScoped<IBookMarkedTaskService, BookMarkedTaskService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IRoomSettingService, RoomSettingService>();


            return services;
        }
    }
}

using Hangfire.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Optern.Application.Interfaces.IUserNotificationService;
using Optern.Application.Mappings;
using Optern.Application.Services.NotificationService;
using Optern.Application.Services.UserNotificationService;
using Optern.Infrastructure.ExternalServices.AutoCompleteService;
using Optern.Infrastructure.ExternalServices.BackgroundJobs;
using Optern.Infrastructure.ExternalServices.CacheService;
using Optern.Infrastructure.ExternalServices.FileService;
using Optern.Infrastructure.ExternalServices.JWTService;
using Optern.Infrastructure.ExternalServices.MailService;
using Optern.Infrastructure.Services.AuthService;
using Optern.Infrastructure.Services.BookMarkedTaskService;
using Optern.Infrastructure.Services.ChatService;
using Optern.Infrastructure.Services.CommentService;
using Optern.Infrastructure.Services.FavoritePostsService;
using Optern.Infrastructure.Services.MessageService;
using Optern.Infrastructure.Services.PositionService;
using Optern.Infrastructure.Services.PostService;
using Optern.Infrastructure.Services.ReactService;
using Optern.Infrastructure.Services.RepositoryFileService;
using Optern.Infrastructure.Services.RepositoryService;
using Optern.Infrastructure.Services.RoomService;
using Optern.Infrastructure.Services.RoomSettings;
using Optern.Infrastructure.Services.RoomSkillService;
using Optern.Infrastructure.Services.RoomTrackService;
using Optern.Infrastructure.Services.RoomTracksService;
using Optern.Infrastructure.Services.RoomUserService;
using Optern.Infrastructure.Services.SkillService;
using Optern.Infrastructure.Services.SprintService;
using Optern.Infrastructure.Services.TagService;
using Optern.Infrastructure.Services.TaskActivityService;
using Optern.Infrastructure.Services.TaskService;
using Optern.Infrastructure.Services.TrackService;
using Optern.Infrastructure.Services.UserService;
using Optern.Infrastructure.Services.WorkSpaceService;

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
            services.AddScoped<IAutoCompleteService, AutoCompleteService>();



            // Dependency Injection for Application Services
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITagsService, TagsService>();
            services.AddScoped<ITrackService, TrackService>();
            services.AddScoped<IPositionService, PositionService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IFavoritePostsService, FavoritePostsService>();
            services.AddScoped<IRoomPositionService, RoomPositionService>(); 
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IReactService, ReactService>();
            services.AddScoped<IWorkSpaceService, WorkSpaceService>();
            services.AddScoped<ISprintService, SprintService>();
            services.AddScoped<IRoomUserService, RoomUserService>();
            services.AddScoped<IBookMarkedTaskService, BookMarkedTaskService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IRoomSettingService, RoomSettingService>();
            services.AddScoped<IRoomTrackService, RoomTrackService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<IRoomSkillService, RoomSkillService>();
            services.AddScoped<ITaskActivityService, TaskActivityService>();
            services.AddScoped<IRepositoryService, RepositoryService>();
            services.AddScoped<IRepositoryFileService, RepositoryFileService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserNotificationService, UserNotificationService>();


            return services;
        }
    }
}

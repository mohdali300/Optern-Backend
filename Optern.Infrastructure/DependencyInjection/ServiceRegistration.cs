

using Optern.Application.Interfaces.IExperienceService;
using Optern.Infrastructure.Services.EducationService;
using Optern.Infrastructure.Services.Experience;
using Optern.Infrastructure.Services.UserSkillsService;

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
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();



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
            services.AddScoped<IPTPFeedbackService,PTPFeedbackService>();  
            services.AddScoped<IPTPInterviewService, PTPInterviewService>();   
            services.AddScoped<IEducationService, EducationService>();
            services.AddScoped<IUserSkillsService, UserSkillsService>();
            services.AddScoped<IExperienceService, ExperienceService>();


            return services;
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Optern.Application.Helpers;
using Optern.Application.Interfaces.IAuthService;
using Optern.Application.Interfaces.ICommentService;
using Optern.Application.Interfaces.IFavoritePostsService;
using Optern.Application.Interfaces.IPostService;
using Optern.Application.Interfaces.IRoomService;
using Optern.Application.Interfaces.IRoomTrackService;
using Optern.Application.Interfaces.ISubTrackService;
using Optern.Application.Interfaces.ITagService;
using Optern.Application.Interfaces.ITrackService;
using Optern.Application.Interfaces.IUserService;
using Optern.Application.Mappings;
using Optern.Application.Services.AuthService;
using Optern.Application.Services.CommentService;
using Optern.Application.Services.FavoritePostsService;
using Optern.Application.Services.PostService;
using Optern.Application.Services.RoomService;
using Optern.Application.Services.RoomTrackService;
using Optern.Application.Services.SubTrackService;
using Optern.Application.Services.TagService;
using Optern.Application.Services.TrackService;
using Optern.Application.Services.UserService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.ExternalDTOs.Mail;
using Optern.Infrastructure.ExternalInterfaces.ICacheService;
using Optern.Infrastructure.ExternalInterfaces.IFileService;
using Optern.Infrastructure.ExternalInterfaces.IJWTService;
using Optern.Infrastructure.ExternalInterfaces.IMail;
using Optern.Infrastructure.ExternalServices.CacheService;
using Optern.Infrastructure.ExternalServices.FileService;
using Optern.Infrastructure.ExternalServices.JWTService;
using Optern.Infrastructure.ExternalServices.MailService;
using Optern.Infrastructure.UnitOfWork;

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
            services.AddScoped<IFileService, FileService>();



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


            return services;
        }
    }
}

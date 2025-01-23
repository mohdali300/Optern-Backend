using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Optern.Application.DTOs.Mail;
using Optern.Application.Helpers;
using Optern.Application.Interfaces.IAuthService;
using Optern.Application.Interfaces.ICacheService;
using Optern.Application.Interfaces.IJWTService;
using Optern.Application.Interfaces.ITagService;
using Optern.Application.Mappings;
using Optern.Application.Services.AuthService;
using Optern.Application.Services.TagService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.ExternalServices.CacheService;
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


            // Dependency Injection for Application Services
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<ITagsService, TagsService>();


			return services;
		}
	}
}

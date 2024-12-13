using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Optern.Infrastructure.Data;
using FluentValidation;
using Optern.Infrastructure.Validations;
using AppAny.HotChocolate.FluentValidation;
using Optern.Presentation.GraphQlApi.Auth.Query;
using Optern.Presentation.GraphQlApi.Auth.Mutation;
using Optern.Application.Interfaces.IAuthService;
using Optern.Application.Services.AuthService;
using Microsoft.AspNetCore.Identity;
using Optern.Domain.Entities;
using Optern.Application.DTOs.Mail;
using Optern.Infrastructure.ExternalServices.MailService;
using Optern.Application.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();


// Add Postgresql Setting
builder.Services.AddDbContext<OpternDbContext>(options =>
	 options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register Graphql
builder.Services
	.AddGraphQLServer()
	.AddQueryType<AuthQuery>()
	.AddMutationType<AuthMutation>()
	.AddFluentValidation();

// DI
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<OpternDbContext>()
    .AddDefaultTokenProviders();

// mail settings
var emailconvig = builder.Services.Configure<MailSettingsDTO>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddSingleton(emailconvig);
builder.Services.AddTransient<IMailService, MailService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped <OTP>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGraphQL("/ui/graphql");
app.Run();

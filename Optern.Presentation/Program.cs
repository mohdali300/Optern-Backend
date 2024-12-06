using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Optern.Infrastructure.Data;
using FluentValidation;
using Optern.Infrastructure.Validations;
using Optern.Infrastructure.MiddleWares;

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
	.AddGraphQLServer();

// Register ValidationMiddleWare
//builder.Services.AddTransient<ValidationMiddleware>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

// use ValidationMiddleware globally for all requests
//app.UseMiddleware<ValidationMiddleware>();

//GraphQL
// app.UseGraphQL<AppSchema>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGraphQL("/ui/graphql");
app.Run();

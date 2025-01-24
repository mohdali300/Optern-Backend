using AppAny.HotChocolate.FluentValidation;
using FluentValidation;
using Optern.Application.Mappings;
using Optern.Infrastructure;
using Optern.Infrastructure.DependencyInjection;
using Optern.Infrastructure.Validations;
using Optern.Presentation.GraphQlApi.Auth.Mutation;
using Optern.Presentation.GraphQlApi.Auth.Query;
using Optern.Presentation.GraphQlApi.SubTrack.Mutation;
using Optern.Presentation.GraphQlApi.SubTrack.Query;
using Optern.Presentation.GraphQlApi.Track.Mutation;
using Optern.Presentation.GraphQlApi.Track.Query;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register Infrastructure Services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();


// Register Graphql
builder.Services
	.AddGraphQLServer()
	.AddQueryType(q => q.Name("Query"))
	.AddType<AuthQuery>()
	.AddType<TrackQuery>()
	.AddType<SubTrackQuery>()
	.AddMutationType(m=>m.Name("Mutation"))
	.AddType<AuthMutation>()
	.AddType<TrackMutation>()
	.AddType<SubTrackMutation>()
    .AddFluentValidation();

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigin", policy =>
	{
		policy.WithOrigins("http://localhost:3000")
			  .AllowAnyHeader()
			  .AllowAnyMethod()
			  .AllowCredentials();
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGraphQL("/ui/graphql");
app.Run();

using AppAny.HotChocolate.FluentValidation;
using FluentValidation;
using Optern.Application.Mappings;
using Optern.Infrastructure;
using Optern.Infrastructure.DependencyInjection;
using Optern.Infrastructure.Hubs;
using Optern.Infrastructure.Validations;
using Optern.Presentation.GraphQlApi;
using Optern.Presentation.GraphQlApi.Auth.Mutation;
using Optern.Presentation.GraphQlApi.Auth.Query;
using Optern.Presentation.GraphQlApi.Comment.Mutation;
using Optern.Presentation.GraphQlApi.Comment.Query;
using Optern.Presentation.GraphQlApi.FavouritePost.Mutation;
using Optern.Presentation.GraphQlApi.FavouritePost.Query;
using Optern.Presentation.GraphQlApi.Post.Mutation;
using Optern.Presentation.GraphQlApi.Post.Query;
using Optern.Presentation.GraphQlApi.React.Mutation;
using Optern.Presentation.GraphQlApi.React.Query;
using Optern.Presentation.GraphQlApi.Rooms.Mutation;
using Optern.Presentation.GraphQlApi.Rooms.Query;
using Optern.Presentation.GraphQlApi.RoomTrack.Query;
using Optern.Presentation.GraphQlApi.RoomUser.Mutation;
using Optern.Presentation.GraphQlApi.RoomUser.Query;
using Optern.Presentation.GraphQlApi.SubTrack.Mutation;
using Optern.Presentation.GraphQlApi.SubTrack.Query;
using Optern.Presentation.GraphQlApi.Tag;
using Optern.Presentation.GraphQlApi.Task.Mutation;
using Optern.Presentation.GraphQlApi.Track.Mutation;
using Optern.Presentation.GraphQlApi.Track.Query;
using Optern.Presentation.GraphQlApi.WorkSpace.Mutation;

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

// register SignalR
builder.Services.AddSignalR(options =>
{
	options.EnableDetailedErrors = true;
});


#region Register GraphQL
builder.Services
.AddGraphQLServer()
.AddQueryType(q => q.Name("Query"))
.AddType<AuthQuery>()
.AddType<RoomQuery>()
.AddType<TrackQuery>()
.AddType<SubTrackQuery>()
.AddType<RoomTrackQuery>()
.AddType<PostQuery>()
.AddType<TagQuery>()
.AddType<FavouritePostsQuery>()
.AddType<CommentQuery>()
.AddType<ReactQuery>()
.AddType<RoomUserQuery>()
.AddMutationType(m => m.Name("Mutation"))
.AddType<AuthMutation>()
.AddType<RoomMutation>()
.AddType<TrackMutation>()
.AddType<SubTrackMutation>()
.AddType<CommentMutation>()
.AddType<FavouritePostsMutation>()
.AddType<UploadType>()
.AddType<ReactMutation>()
.AddType<PostMutation>()
.AddType<WorkSpaceMutation>()
.AddType<TaskMutation>()
.AddType<RoomUserMutation>()
.AddFluentValidation();
#endregion

builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);


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
app.UseStaticFiles();
app.UseAuthorization();

app.MapHub<ChatHub>("/ChatHub");
app.MapHub<NotificationHub>("/NotificationHub");
app.MapControllers();
app.MapGraphQL("/ui/graphql");
app.Run();

// Initialize builder

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


#region Register GraphQL
builder.Services
.AddGraphQLServer()
.AddQueryType(q => q.Name("Query"))
.AddType<AuthQuery>()
.AddType<RoomQuery>()
.AddType<TrackQuery>()
.AddType<PositionQuery>()
.AddType<RoomTrackQuery>()
.AddType<PostQuery>()
.AddType<TagQuery>()
.AddType<FavouritePostsQuery>()
.AddType<CommentQuery>()
.AddType<ReactQuery>()
.AddType<RoomUserQuery>()
.AddType<SprintQuery>()
.AddType<BookMarkedTaskQuery>()
.AddType<TaskQuery>()
.AddType<TaskActivityQuery>()
.AddType<RepositoryFileQuery>()
.AddType<SkillQuery>()
.AddType<ChatQuery>()
.AddType<WorkSpaceQuery>()
.AddType<UserNotificationQuery>()
.AddType<MessageQuery>()
.AddType<GoogleAuthQuery>()
.AddType<PTPFeedbackQuery>()
.AddType<PTPInterviewQuery>()
.AddType<EducationQuery>()
.AddType<UserSkillsQuery>()
.AddMutationType(m => m.Name("Mutation"))
.AddType<AuthMutation>()
.AddType<RoomMutation>()
.AddType<TrackMutation>()
.AddType<PositionMutation>()
.AddType<CommentMutation>()
.AddType<FavouritePostsMutation>()
.AddType<ReactMutation>()
.AddType<PostMutation>()
.AddType<WorkSpaceMutation>()
.AddType<TaskMutation>()
.AddType<RoomUserMutation>()
.AddType<SprintMutation>()
.AddType<BookMarkedTaskMutation>()
.AddType<TaskActivityMutation>()
.AddType<RepositoryFileMutation>()
.AddType<NotificationMutation>()
.AddType<UserNotificationMutation>()
.AddType<MessageMutation>()
.AddType<GoogleAuthMutation>()
.AddType<PTPFeedbackMutation>()
.AddType<PTPInterviewMutation>()
.AddType<UserMutation>()
.AddType<UserSkillsMutation>()
.AddType<EducationMutation>()
.AddType<SkillMutation>()
.AddFluentValidation()
.AddType<UploadType>(); 

#endregion

builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();


// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Logging.ClearProviders()
	.AddSerilog()
	.AddConsole()
	.SetMinimumLevel(LogLevel.Information);

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigin", policy =>
	{
		policy.WithOrigins("http://localhost:3000")//"http://127.0.0.1:5500"
              .AllowAnyHeader()
			  .AllowAnyMethod()
			  .AllowCredentials();
	});
});
// register SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
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

// Hangfire
app.UseHangfireDashboard("/hangfire");
using (var scope = app.Services.CreateScope())
{
	var userCleanUpScheduler = scope.ServiceProvider.GetRequiredService<UserCleanUpJob>();
	userCleanUpScheduler.UserCleanUp();
}

app.MapHub<ChatHub>("/Chat");
app.MapHub<NotificationHub>("/Notification");
app.MapControllers();
app.MapGraphQL("/ui/graphql");
app.UseGoogleAuthMiddleware();
app.Logger.LogInformation("Website is running.");
app.Run();

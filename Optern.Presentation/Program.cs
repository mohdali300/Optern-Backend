using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


// Add Postgresql Setting
// builder.Services.AddDbContext<Context>(options =>
//  	options.UseNpgsql(builder.Configuration.GetConnectionString("connstr"))
// ); 

// Register Graphql
builder.Services
	.AddGraphQLServer();



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


//GraphQL
// app.UseGraphQL<AppSchema>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGraphQL("/ui/graphql");
app.Run();

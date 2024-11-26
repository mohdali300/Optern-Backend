using GraphiQl;
using GraphQL.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Optern.Presentation.GraphQlApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


// Add Postgresql Setting
// builder.Services.AddDbContext<Context>(options =>
//  	options.UseNpgsql(builder.Configuration.GetConnectionString("connstr"))
// ); 

// Register Graphql
builder.Services.AddGraphQL();



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
app.UseGraphQLGraphiQL("/ui/graphql");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using System.Diagnostics;
using Movies.Application;
using Movies.Application.Database;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add business logic layer services
builder.Services.AddApplication();

// Add database connection
var connectionString = builder.Configuration["Database:ConnectionString"];
Debug.Assert(connectionString != null);
builder.Services.AddDatabase(connectionString);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Initialise database
var dbInitialiser = app.Services.GetRequiredService<DbInitialiser>();
await dbInitialiser.InitialiseAsync();

app.Run();
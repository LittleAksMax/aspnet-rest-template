using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Movies.Api.Middleware;
using Movies.Application;
using Movies.Application.Database;

var builder = WebApplication.CreateBuilder(args);

// Add JWT
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
Debug.Assert(jwtKey is not null);
Debug.Assert(jwtIssuer is not null);
Debug.Assert(jwtAudience is not null);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuerSigningKey = true, // actually validate the tokens
        ValidateLifetime = true, // don't allow expired tokens
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        ValidateIssuer = true, // make sure issuer of token matches
        ValidateAudience = true // make sure audience to token matches
    };
});

// Add authorisation
builder.Services.AddAuthorization();

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

// Use authentication and authorisation
app.UseAuthentication();
app.UseAuthorization();

// Add validation middleware
app.UseMiddleware<ValidationMappingMiddleware>();

app.MapControllers();

// Initialise database
var dbInitialiser = app.Services.GetRequiredService<DbInitialiser>();
await dbInitialiser.InitialiseAsync();

app.Run();
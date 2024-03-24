using System.Diagnostics;
using Movies.Api.Extensions.Endpoints;
using Movies.Api.Extensions.ServiceCollection;
using Movies.Api.Middleware;
using Movies.Application.Database;

var builder = WebApplication.CreateBuilder(args);

// Add JWT
var config = builder.Configuration;
var jwtKey = config["Jwt:Key"];
var jwtIssuer = config["Jwt:Issuer"];
var jwtAudience = config["Jwt:Audience"];
var connectionString = config["Database:ConnectionString"];
var apiKey =  config["ApiKey:Key"];

Debug.Assert(jwtKey is not null);
Debug.Assert(jwtIssuer is not null);
Debug.Assert(jwtAudience is not null);
Debug.Assert(connectionString != null);
Debug.Assert(apiKey is not null);

builder.Services.AddServices(jwtKey, jwtAudience, jwtIssuer, apiKey, connectionString);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        foreach (var desc in app.DescribeApiVersions())
        {
            x.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName);
        }
    });
}

// Add endpoint to display health diagnostics
app.MapHealthChecks("_health");

// Redirect traffic to HTTPS
app.UseHttpsRedirection();

// Use authentication and authorisation
app.UseAuthentication();
app.UseAuthorization();

// Use response caching
// app.UseCors(); // for cross-origin requests
// make sure the UseCache statements are below the CORS statement
// app.UseResponseCaching(); // client-side caching
app.UseOutputCache(); // by default only 200 OK are cached
                      // by default only GET and HEAD requests are cached
                      // by default responses that set cookies are not cached
                      // by default responses to authenticated requests are not cached (request headers are checked)

// Add validation middleware
app.UseMiddleware<ValidationMappingMiddleware>();

// Used for controller classes, not used for minimal APIs
// app.MapControllers();

// Add minimal API endpoints
app.MapApiEndpoints();

// Initialise database
var dbInitialiser = app.Services.GetRequiredService<DbInitialiser>();
await dbInitialiser.InitialiseAsync();

app.Run();
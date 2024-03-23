using System.Diagnostics;
using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.Api.Auth.Constants;
using Movies.Api.Health;
using Movies.Api.Middleware;
using Movies.Api.Services;
using Movies.Api.Swagger;
using Movies.Contracts.Requests.Queries.Queries.Application;
using Movies.Application.Database;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>(DatabaseHealthCheck.Name);

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
builder.Services.AddAuthorization(x =>
{
    x.AddPolicy(AuthConstants.AdminPolicyName, p => p.RequireClaim(AuthConstants.AdminClaimName, "true"));
    x.AddPolicy(AuthConstants.TrustedMemberPolicyName, p =>
        p.RequireAssertion(c =>
            c.User.HasClaim(m =>
                m is { Type: AuthConstants.AdminClaimName, Value: "true" }) || 
            c.User.HasClaim(m => m is { Type: AuthConstants.TrustedMemberClaimName, Value: "true" })
            ));
});

// Add versioning
builder.Services.AddApiVersioning(x =>
{
    x.DefaultApiVersion = new ApiVersion(1.0); // set the default API version
    x.AssumeDefaultVersionWhenUnspecified = true; // so the default doesn't need to be specified in the URL
    x.ReportApiVersions = true; // adds supported versions to the response header
                                // deprecated versions are reported under a separate response header
    x.ApiVersionReader = new MediaTypeApiVersionReader(); // where to get version from,
                                                          // this one gets it from the api-version parameter
                                                          // in the request header
}).AddMvc().AddApiExplorer();

// Add Swagger for documentation
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(x => x.OperationFilter<SwaggerDefaultValues>());

// Add controllers
builder.Services.AddControllers();

// Add business logic layer services
builder.Services.AddSingleton<IUriService, UriService>();
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

// Add validation middleware
app.UseMiddleware<ValidationMappingMiddleware>();

app.MapControllers();

// Initialise database
var dbInitialiser = app.Services.GetRequiredService<DbInitialiser>();
await dbInitialiser.InitialiseAsync();

app.Run();
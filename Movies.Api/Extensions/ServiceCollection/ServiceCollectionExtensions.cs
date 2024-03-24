using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.Api.Auth.Constants;
using Movies.Api.Auth.Filters;
using Movies.Api.Auth.Handlers;
using Movies.Api.Caching;
using Movies.Api.Health;
using Movies.Api.Services;
using Movies.Api.Swagger;
using Movies.Application.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Movies.Api.Extensions.ServiceCollection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services,
        string jwtKey, string jwtAudience, string jwtIssuer,
        string apiKey,
        string connectionString)
    {
        // Add Auth
        services
            .AddAuth(jwtKey, jwtAudience, jwtIssuer, apiKey)
            .AddVersioning() // Add API versioning
            .AddCaching() // Add output caching
            .AddMyHealthChecks()
            .AddSwagger() // Add Swagger for documentation
            .AddApiLayerServices()
            .AddApplication() // Add business logic layer services
            .AddDatabase(connectionString);
        return services;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services,
        string jwtKey, string jwtAudience, string jwtIssuer, string apiKey)
    {
        services.AddAuthentication(x =>
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
        services.AddAuthorization(x =>
        {
            x.AddPolicy(AuthConstants.AdminPolicyName, p =>
                p.AddRequirements(new AdminAuthRequirement(apiKey)));
            x.AddPolicy(AuthConstants.TrustedMemberPolicyName, p =>
                p.RequireAssertion(c =>
                    c.User.HasClaim(m =>
                        m is { Type: AuthConstants.AdminClaimName, Value: "true" }) || 
                    c.User.HasClaim(m => m is { Type: AuthConstants.TrustedMemberClaimName, Value: "true" })
                ));
        });

        // Add API Key filter
        services.AddScoped<ApiKeyAuthFilter>();

        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services)
    {
        // builder.Services.AddResponseCaching(); // client-side
        services.AddOutputCache(x =>
        {
            // wherever I label that I want to use output caching, cache the response
            x.AddBasePolicy(c => c.Cache());
            x.AddPolicy(CachingConstants.GetAllMoviePolicyName, c =>
                    c.Cache()
                        .Expire(TimeSpan.FromMinutes(1))
                        .SetVaryByQuery(new[] { "title", "year", "sortBy", "page", "pageSize" })
                        .Tag(CachingConstants.MoviesCacheTag) // used for cache invalidation
            );
        });

        return services;
    }

    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(x => x.OperationFilter<SwaggerDefaultValues>());
        return services;
    }

    private static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(x =>
        {
            x.DefaultApiVersion = new ApiVersion(1.0); // set the default API version
            x.AssumeDefaultVersionWhenUnspecified = true; // so the default doesn't need to be specified in the URL
            x.ReportApiVersions = true; // adds supported versions to the response header
            // deprecated versions are reported under a separate response header
            x.ApiVersionReader = new MediaTypeApiVersionReader(); // where to get version from,
            // this one gets it from the api-version parameter
            // in the request header
        }).AddMvc().AddApiExplorer();
        return services;
    }

    private static IServiceCollection AddMyHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>(DatabaseHealthCheck.Name);
        return services;
    }

    private static IServiceCollection AddApiLayerServices(this IServiceCollection services)
    {
        services.AddSingleton<IUriService, UriService>();
        return services;
    }
}
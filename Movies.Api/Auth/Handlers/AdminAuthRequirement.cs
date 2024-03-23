using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Movies.Api.Auth.Constants;

namespace Movies.Api.Auth.Handlers;

public class AdminAuthRequirement : IAuthorizationHandler, IAuthorizationRequirement
{
    private readonly string _apiKey;

    public AdminAuthRequirement(string apiKey)
    {
        _apiKey = apiKey;
    }

    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.User.HasClaim(AuthConstants.AdminClaimName, "true"))
        {
            context.Succeed(this);
            return Task.CompletedTask;
        }

        var httpContext = context.Resource as HttpContext;
        if (httpContext is null)
        {
            return Task.CompletedTask;
        }
        
        // try to get API Key
        var couldGetKey = httpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey);
        
        // if API Key was not in headers, then 
        if (!couldGetKey)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        // Unauthorised if given API key does not match the one extracted from header
        if (_apiKey != extractedApiKey)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var identity = (ClaimsIdentity)httpContext.User.Identity!;
        // Add user ID claim because it needs to be fetched by controller endpoints as
        // an extension to the HttpContext
        identity.AddClaim(new Claim(AuthConstants.UserIdClaimName, 
            Guid.Parse(AuthConstants.AdminApiKeyUserIdString).ToString()));
        
        context.Succeed(this);
        return Task.CompletedTask;
    }
}
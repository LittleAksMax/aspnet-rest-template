using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Movies.Api.Auth.Constants;

namespace Movies.Api.Auth.Filters;

public class ApiKeyAuthFilter : IAuthorizationFilter
{
    private readonly IConfiguration _configuration;

    public ApiKeyAuthFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // try to get API Key
        var couldGetKey = context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey);
        
        // if API Key was not in headers, then 
        if (!couldGetKey)
        {
            context.Result = new UnauthorizedObjectResult("API Key missing.");
            return;
        }

        // get configured API Key from AppSettings
        var apiKey = _configuration["ApiKey"]!;
        
        // Unauthorised if given API key does not match the one extracted from header
        if (apiKey != extractedApiKey)
        {
            context.Result = new UnauthorizedObjectResult("API Key missing.");
        }
    }
}
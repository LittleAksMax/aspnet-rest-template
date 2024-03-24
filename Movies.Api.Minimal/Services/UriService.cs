using Microsoft.AspNetCore.WebUtilities;
using Movies.Contracts.Requests.Queries;

namespace Movies.Api.Services;

public class UriService : IUriService
{
    public Task<string?> GetPrevPage(HttpContext context, PagedQuery paginationQuery)
    {
        if (paginationQuery.Page == 1)
        {
            return Task.FromResult<string?>(null);
        }
    
        // add previous query parameters and append new page query parameters
        var queries = new Dictionary<string, string?>
        {
            // add pagination query parameters first
            { "page", (paginationQuery.Page - 1).ToString() },
            { "pageSize", paginationQuery.PageSize.ToString() }
        };

        // add previous query parameters to preserve the request type
        foreach (var queryParam in context.Request.Query)
        {
            // skip pagination query parameters, since we will add them later
            if (queryParam.Key.ToLower() == "page" || queryParam.Key.ToLower() == "pagesize")
            {
                continue;
            }
            queries.Add(queryParam.Key, queryParam.Value);
        }
        
        var uri = QueryHelpers.AddQueryString(ApiRoutes.Movies.GetAll, queries);
        return Task.FromResult<string?>($"/{uri}");
    }

    public Task<string?> GetNextPage(HttpContext context, PagedQuery paginationQuery, int totalCount)
    {
        if (paginationQuery.Page * paginationQuery.PageSize >= totalCount)
        {
            return Task.FromResult<string?>(null);
        }
        
        // add previous query parameters and append new page query parameters
        var queries = new Dictionary<string, string?>
        {
            // add new pagination query parameters
            { "page", (paginationQuery.Page + 1).ToString() },
            { "pageSize", paginationQuery.PageSize.ToString() }
        };

        // add previous query parameters to preserve the request type
        foreach (var queryParam in context.Request.Query)
        {
            // skip pagination query parameters, since we will add them later
            if (queryParam.Key.ToLower().Equals("page") || queryParam.Key.ToLower().Equals("pagesize"))
            {
                continue;
            }
            queries.Add(queryParam.Key, queryParam.Value);
        }
        
        var uri = QueryHelpers.AddQueryString(ApiRoutes.Movies.GetAll, queries);
        return Task.FromResult<string?>($"/{uri}");
    }
}
using Movies.Contracts.Requests.Queries;

namespace Movies.Api.Services;

public interface IUriService
{
    Task<string?> GetPrevPage(HttpContext context, int page, int pageSize);
    Task<string?> GetNextPage(HttpContext context, int page, int pageSize, int totalCount);
}
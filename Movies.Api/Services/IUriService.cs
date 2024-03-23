using Movies.Contracts.Requests.Queries;

namespace Movies.Api.Services;

public interface IUriService
{
    Task<string?> GetPrevPage(HttpContext context, PagedQuery paginationQuery);
    Task<string?> GetNextPage(HttpContext context, PagedQuery paginationQuery, int totalCount);
}
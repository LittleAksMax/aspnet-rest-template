namespace Movies.Contracts.Responses.Pagination;

public class PagedResponse
{
    public required int PageSize { get; init; }
    public required int Page { get; init; }
    public required int Total { get; init; }
    public bool HasPrev => Page > 1;
    public bool HasNext => Total > (Page * PageSize);
}
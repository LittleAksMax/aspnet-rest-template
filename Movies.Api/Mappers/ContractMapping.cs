using Movies.Application.Models.Options;
using Movies.Contracts.Dtos;
using Movies.Contracts.Requests;
using Movies.Contracts.Requests.Queries;
using Movies.Contracts.Responses;

namespace Movies.Api.Mappers;

public static class ContractMapping
{
    public static MovieDto MapToMovieDto(this CreateMovieRequest request)
    {
        return new MovieDto
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Slug = null!, // it won't be used to it doesn't need to be bound
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
    }
    
    public static MovieDto MapToMovieDto(this UpdateMovieRequest request, Guid id)
    {
        return new MovieDto
        {
            Id = id,
            Title = request.Title,
            Slug = null!, // it won't be used, so it doesn't need to be bound
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
    }

    public static MovieResponse MapToMovieResponse(this MovieDto movie)
    {
        return new MovieResponse
        {
            Id = movie.Id,
            Title = movie.Title,
            Slug = movie.Slug,
            YearOfRelease = movie.YearOfRelease,
            Rating = movie.Rating,
            UserRating = movie.UserRating,
            Genres = movie.Genres
        };
    }

    public static MoviesResponse MapToMoviesResponse(this IEnumerable<MovieDto> movies, PagedQuery paginationQuery, int totalCount,
        string? prev, string? next)
    {
        return new MoviesResponse
        {
            Movies = movies.Select(m => m.MapToMovieResponse()),
            Page = paginationQuery.Page.GetValueOrDefault(PagedQuery.DefaultPage),
            PageSize = paginationQuery.PageSize.GetValueOrDefault(PagedQuery.DefaultPageSize),
            Total = totalCount,
            Prev = prev,
            Next = next
        };
    }

    public static MovieRatingsResponse MapToMovieRatingResponse(this IEnumerable<MovieRatingDto> ratings)
    {
        return new MovieRatingsResponse
        {
            Ratings = ratings.Select(x => new MovieRatingResponse
            {
                MovieId = x.MovieId,
                Slug = x.Slug,
                Rating = x.Rating
            })
        };
    }

    public static GetAllMoviesOptions MapToOptions(this GetAllMoviesQuery query, PagedQuery paginationQuery)
    {
        return new GetAllMoviesOptions
        {
            Title = query.Title,
            YearOfRelease = query.Year,
            SortField = query.SortBy?.Trim('+', '-'),
            SortOrder = query.SortBy is null ? SortOrder.Unordered
                : (query.SortBy.StartsWith('-') ? SortOrder.Descending : SortOrder.Ascending),
            Page = paginationQuery.Page.GetValueOrDefault(PagedQuery.DefaultPage),
            PageSize = paginationQuery.PageSize.GetValueOrDefault(PagedQuery.DefaultPageSize)
        };
    }

    public static GetAllMoviesOptions WithUser(this GetAllMoviesOptions options, Guid? userId)
    {
        options.UserId = userId;
        return options;
    }
}
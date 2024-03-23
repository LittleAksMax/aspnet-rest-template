using Movies.Application.Models;
using Movies.Application.Models.Options;
using Movies.Contracts.Requests;
using Movies.Contracts.Requests.Queries;
using Movies.Contracts.Responses;

namespace Movies.Api.Mappers;

public static class ContractMapping
{
    public static Movie MapToMovie(this CreateMovieRequest request)
    {
        return new Movie
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
    }
    
    public static Movie MapToMovie(this UpdateMovieRequest request, Guid id)
    {
        return new Movie
        {
            Id = id,
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
    }

    public static MovieResponse MapToMovieResponse(this Movie movie)
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

    public static MoviesResponse MapToMoviesResponse(this IEnumerable<Movie> movies, PagedQuery paginationQuery, int totalCount)
    {
        return new MoviesResponse
        {
            Movies = movies.Select(m => m.MapToMovieResponse()),
            Page = paginationQuery.Page,
            PageSize = paginationQuery.PageSize,
            Total = totalCount,
        };
    }

    public static MovieRatingsResponse MapToMovieRatingResponse(this IEnumerable<MovieRating> ratings)
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
            Page = paginationQuery.Page,
            PageSize = paginationQuery.PageSize
        };
    }

    public static GetAllMoviesOptions WithUser(this GetAllMoviesOptions options, Guid? userId)
    {
        options.UserId = userId;
        return options;
    }
}
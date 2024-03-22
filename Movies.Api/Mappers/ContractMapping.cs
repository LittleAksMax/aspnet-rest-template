using Movies.Application.Models;
using Movies.Application.Models.Options;
using Movies.Contracts.Requests;
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

    public static GetAllMoviesOptions MapToOptions(this GetAllMoviesRequest query)
    {
        return new GetAllMoviesOptions
        {
            Title = query.Title,
            YearOfRelease = query.Year
        };
    }

    public static GetAllMoviesOptions WithUser(this GetAllMoviesOptions options, Guid? userId)
    {
        options.UserId = userId;
        return options;
    }
}
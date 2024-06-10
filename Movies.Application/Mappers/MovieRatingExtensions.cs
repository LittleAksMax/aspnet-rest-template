using Movies.Application.Models;
using Movies.Contracts.Dtos;

namespace Movies.Application.Mappers;

public static class MovieRatingExtensions
{
    public static MovieRatingDto ToDto(this MovieRating mr)
    {
        return new MovieRatingDto
        {
            MovieId = mr.MovieId,
            Slug = mr.Slug,
            Rating = mr.Rating
        };
    }

    public static MovieRating ToDom(this MovieRatingDto dto)
    {
        return new MovieRating
        {
            MovieId = dto.MovieId,
            Slug = dto.Slug,
            Rating = dto.Rating
        };
    }
}
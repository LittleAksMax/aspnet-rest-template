using Movies.Application.Models;
using Movies.Contracts.Dtos;

namespace Movies.Application.Mappers;

public static class MovieExtensions
{
    public static Movie ToDom(this MovieDto dto)
    {
        // NOTE: slug will generate automatically
        return new Movie
        {
            Id = dto.Id,
            Title = dto.Title,
            YearOfRelease = 0,
            Rating = dto.Rating,
            UserRating = dto.UserRating,
            Genres = dto.Genres
        };
    }

    public static MovieDto ToDto(this Movie dom)
    {
        return new MovieDto
        {
            Id = dom.Id,
            Title = dom.Title,
            YearOfRelease = dom.YearOfRelease,
            Slug = dom.Slug,
            Rating = dom.Rating,
            UserRating = dom.UserRating,
            Genres = dom.Genres
        };
    }
}
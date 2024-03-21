using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    // temporary database
    private readonly List<Movie> _movies = new();
    
    public Task<bool> CreateAsync(Movie movie)
    {
        _movies.Add(movie);
        return Task.FromResult(true);
    }

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        var movie = _movies.FirstOrDefault(x => x.Id.Equals(id));
        return Task.FromResult(movie);
    }

    public Task<Movie?> GetBySlugAsync(string slug)
    {
        var movie = _movies.FirstOrDefault(m => m.Slug.Equals(slug));
        return Task.FromResult(movie);
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        return Task.FromResult(_movies.AsEnumerable());
    }

    public Task<bool> UpdateAsync(Movie movie)
    {
        var idx = _movies.FindIndex(x => x.Id.Equals(movie.Id));
        if (idx == -1)
        {
            return Task.FromResult(false);
        }

        _movies[idx] = movie;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteById(Guid id)
    {
        var numberRemoved = _movies.RemoveAll(x => x.Id.Equals(id));
        return Task.FromResult(numberRemoved > 0);
    }
}
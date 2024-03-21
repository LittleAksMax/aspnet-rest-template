using System.Diagnostics;
using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public MovieRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }


    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();
        
        // used to count number of affected rows (used for errors)
        int result;
        
        // create movie object
        result = await connection.ExecuteAsync(new CommandDefinition("""
            INSERT INTO movies (id, title, slug, yearofrelease)
            VALUES (@Id, @Title, @Slug, @YearOfRelease);
            """, movie, cancellationToken: token));

        // there was an error, rows not affected
        if (result <= 0)
        {
            transaction.Rollback();
            return false;
        }
        
        // get all IDs of genres, and return false if
        // one of the genres doesn't exist
        foreach (var genre in movie.Genres)
        {
            result = await connection.ExecuteAsync(
                new CommandDefinition("""
                  INSERT INTO genres (movieid, name)
                  VALUES (@id, @name);
                  """, new { id = movie.Id, name = genre},
                    cancellationToken: token
                ));

            if (result != 1)
            {
                transaction.Rollback();
                return false;
            }
        }

        transaction.Commit();
        return true;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition("""
              SELECT * FROM movies WHERE id=@id
              """, new { id },
                cancellationToken: token)
        );

        // the movie doesn't exist
        if (movie is null)
        {
            return null;
        }
        
        // get corresponding movie genres
        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("""
              SELECT name FROM genres
              WHERE movieid=@id;
              """, new { id }, cancellationToken: token));

        // add each corresponding genre to data model
        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        var result = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition("""
              SELECT COUNT(1) FROM movies
              WHERE id=@id
              """, new { id }, cancellationToken: token
            ));
        
        Debug.Assert(result == 0 || result == 1);
        return result == 1;
    }

    public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition("""
                                  SELECT * FROM movies WHERE slug=@slug
                                  """, new { slug }, cancellationToken: token)
        );

        // the movie doesn't exist
        if (movie is null)
        {
            return null;
        }
        
        // get corresponding movie genres
        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("""
                  SELECT name FROM genres
                  WHERE movieid=@id;
                  """, new { id = movie.Id }, cancellationToken: token));

        // add each corresponding genre to data model
        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        var movies = await connection.QueryAsync(
            new CommandDefinition("""
                                  SELECT m.*, string_agg(g.name, ',') as genres
                                  FROM movies m
                                  INNER JOIN genres g ON g.movieid=m.id
                                  GROUP BY id;
                                  """, cancellationToken: token));
        return movies.Select(m => new Movie
        {
            Id = m.id,
            Title = m.title,
            YearOfRelease = m.yearofrelease,
            Genres = Enumerable.ToList(m.genres.Split(','))
        });
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        // full update on record if it does exist.
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        var transaction = connection.BeginTransaction();

        // counting number of affected rows (used to signal errors)
        int result;
        
        // delete old genres
        result = await connection.ExecuteAsync(
            new CommandDefinition("""
                                  DELETE FROM genres
                                  WHERE movieid=@id;
                                  """, new { id=movie.Id }, cancellationToken: token));

        if (result < 0)
        {
            transaction.Rollback();
            return false;
        }
        
        // add new genres

        foreach (string genre in movie.Genres)
        {
            result = await connection.ExecuteAsync(
                new CommandDefinition("""
                  INSERT INTO genres (movieid, name)
                  VALUES (@id, @name);
                  """, new { id = movie.Id, name = genre }, cancellationToken: token));

            if (result < 0)
            {
                transaction.Rollback();
                return false;
            }
        }

        // update movie fields
        result = await connection.ExecuteAsync(
            new CommandDefinition("""
                  UPDATE movies
                  SET title = @Title,
                      slug = @Slug,
                      yearofrelease = @YearOfRelease
                  WHERE id = @Id;
                  """, movie, cancellationToken: token));

        if (result != 1)
        {
            transaction.Rollback();
            return false;
        }

        transaction.Commit();
        return true;
    }

    public async Task<bool> DeleteById(Guid id, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(
            new CommandDefinition("""
              DELETE FROM movies
              WHERE id=@id;
              """, new { id }, cancellationToken: token));

        if (result < 1)
        {
            transaction.Rollback();
            return false;
        }
        
        transaction.Commit();
        return true;
    }
}
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mappers;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieRepository _movieRepo;

    public MoviesController(IMovieRepository movieRepo)
    {
        _movieRepo = movieRepo;
    }

    [HttpGet(ApiRoutes.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug)
    {
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieRepo.GetByIdAsync(id)
            : await _movieRepo.GetBySlugAsync(idOrSlug);
        if (movie is null)
        {
            return NotFound();
        }

        return Ok(movie.MapToMovieResponse());
    }

    [HttpGet(ApiRoutes.Movies.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var movies = await _movieRepo.GetAllAsync();
        return Ok(new MoviesResponse { Movies = movies.Select(m => m.MapToMovieResponse()) });
    }

    [HttpPost(ApiRoutes.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
    {
        var movie = request.MapToMovie();
        await _movieRepo.CreateAsync(movie);
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
    }

    [HttpPut(ApiRoutes.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request)
    {
        var movie = request.MapToMovie(id);
        var successfullyUpdated = await _movieRepo.UpdateAsync(movie);
        if (!successfullyUpdated)
        {
            return NotFound();
        }

        return Ok();
    }

    [HttpDelete(ApiRoutes.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var successfullyDeleted = await _movieRepo.DeleteById(id);
        if (!successfullyDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
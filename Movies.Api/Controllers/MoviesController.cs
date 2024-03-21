using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Constants;
using Movies.Api.Mappers;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[Authorize]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [Authorize]
    [HttpGet(ApiRoutes.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
    {
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id, token)
            : await _movieService.GetBySlugAsync(idOrSlug, token);
        if (movie is null)
        {
            return NotFound();
        }

        return Ok(movie.MapToMovieResponse());
    }

    [Authorize]
    [HttpGet(ApiRoutes.Movies.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        var movies = await _movieService.GetAllAsync(token);
        return Ok(new MoviesResponse { Movies = movies.Select(m => m.MapToMovieResponse()) });
    }

    [Authorize(AuthConstants.AdminPolicyName)]
    [HttpPost(ApiRoutes.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
    {
        var movie = request.MapToMovie();
        await _movieService.CreateAsync(movie, token);
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiRoutes.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token)
    {
        var movie = request.MapToMovie(id);
        var updatedMovie = await _movieService.UpdateAsync(movie, token);
        if (updatedMovie is null)
        {
            return NotFound();
        }

        return Ok(updatedMovie);
    }

    [Authorize(AuthConstants.AdminPolicyName)]
    [HttpDelete(ApiRoutes.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        var successfullyDeleted = await _movieService.DeleteById(id, token);
        if (!successfullyDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
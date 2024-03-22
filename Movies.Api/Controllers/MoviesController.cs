using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth.Constants;
using Movies.Api.Auth.Extensions;
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
    private readonly IRatingService _ratingService;

    public MoviesController(IMovieService movieService, IRatingService ratingService)
    {
        _movieService = movieService;
        _ratingService = ratingService;
    }

    [Authorize]
    [HttpGet(ApiRoutes.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id, userId, token)
            : await _movieService.GetBySlugAsync(idOrSlug, userId, token);
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
        var userId = HttpContext.GetUserId();

        var movies = await _movieService.GetAllAsync(userId, token);
        return Ok(new MoviesResponse { Movies = movies.Select(m => m.MapToMovieResponse()) });
    }

    [Authorize(AuthConstants.AdminPolicyName)]
    [HttpPost(ApiRoutes.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        var movie = request.MapToMovie();
        await _movieService.CreateAsync(movie, userId, token);
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiRoutes.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        var movie = request.MapToMovie(id);
        var updatedMovie = await _movieService.UpdateAsync(movie, userId, token);
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

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiRoutes.Movies.Rate)]
    public async Task<IActionResult> Rate([FromRoute] Guid id, [FromBody] RateMovieRequest request, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var rating = request.Rating;

        var successfullyRated = await _ratingService.RateMovieAsync(id, userId!.Value, rating, token);

        if (!successfullyRated)
        {
            return NotFound();
        }
        return Ok();
    }
    
    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpDelete(ApiRoutes.Movies.DeleteRating)]
    public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var successfullyDeleted = await _ratingService.DeleteRatingAsync(id, userId!.Value, token);

        if (!successfullyDeleted)
        {
            return NotFound();
        }
        return Ok();
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth.Extensions;
using Movies.Api.Mappers;
using Movies.Api.Auth.Constants;
using Movies.Api.Services;
using Movies.Contracts.Requests.Queries.Queries.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Requests.Queries;

namespace Movies.Api.Controllers;

[Authorize]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IRatingService _ratingService;
    private readonly IUriService _uriService;
    
    public MoviesController(IMovieService movieService, IRatingService ratingService, IUriService uriService)
    {
        _movieService = movieService;
        _ratingService = ratingService;
        _uriService = uriService;
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
    public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesQuery query, [FromQuery] PagedQuery paginationQuery, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var options = query.MapToOptions(paginationQuery)
            .WithUser(userId);
        
        var movies = await _movieService.GetAllAsync(options, token);
        var movieCount = await _movieService.GetCountAsync(options.Title, options.YearOfRelease, token);
        
        // construct response
        var prev = await _uriService.GetPrevPage(HttpContext, paginationQuery);
        var next = await _uriService.GetNextPage(HttpContext, paginationQuery, movieCount);
        var response = movies.MapToMoviesResponse(paginationQuery, movieCount, prev, next);
        
        // set relevant headers
        HttpContext.Response.Headers.Append("X-Total-Count", movieCount.ToString());
        
        return Ok(response);
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

    [Authorize]
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
    
    [Authorize]
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
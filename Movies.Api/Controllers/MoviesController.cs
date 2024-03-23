using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth.Extensions;
using Movies.Api.Mappers;
using Movies.Api.Auth.Constants;
using Movies.Api.Auth.Filters;
using Movies.Api.Caching;
using Movies.Api.Services;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Requests.Queries;
using Movies.Contracts.Responses;
using Movies.Contracts.Responses.Validation;

namespace Movies.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IRatingService _ratingService;
    private readonly IUriService _uriService;
    private readonly IOutputCacheStore _cacheStore;
    
    public MoviesController(IMovieService movieService, IRatingService ratingService, IUriService uriService, IOutputCacheStore cacheStore)
    {
        _movieService = movieService;
        _ratingService = ratingService;
        _uriService = uriService;
        _cacheStore = cacheStore;
    }

    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    [HttpGet(ApiRoutes.Movies.Get)]
    // [ResponseCache(Duration = 30, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    [HttpGet(ApiRoutes.Movies.GetAll)]
    [OutputCache(PolicyName = CachingConstants.GetAllMoviePolicyName)]
    // [ResponseCache(Duration = 30, VaryByQueryKeys = new[] {"title", "year", "sortBy", "page", "pageSize"}, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
    [ProducesResponseType(typeof(MoviesResponse), StatusCodes.Status200OK)]
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
    
    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPost(ApiRoutes.Movies.Create)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        var movie = request.MapToMovie();
        await _movieService.CreateAsync(movie, userId, token);
        
        // invalidate cache for this after successful creation
        await _cacheStore.EvictByTagAsync(CachingConstants.MoviesCacheTag, token);
        
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiRoutes.Movies.Update)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        var movie = request.MapToMovie(id);
        var updatedMovie = await _movieService.UpdateAsync(movie, userId, token);
        if (updatedMovie is null)
        {
            return NotFound();
        }

        // invalidate cache for this after successful creation
        await _cacheStore.EvictByTagAsync(CachingConstants.MoviesCacheTag, token);
        
        return Ok(updatedMovie);
    }

    [Authorize(AuthConstants.AdminPolicyName)]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    [HttpDelete(ApiRoutes.Movies.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        var successfullyDeleted = await _movieService.DeleteById(id, token);
        if (!successfullyDeleted)
        {
            return NotFound();
        }

        // invalidate cache for this after successful creation
        await _cacheStore.EvictByTagAsync(CachingConstants.MoviesCacheTag, token);
        
        return NoContent();
    }

    [Authorize]
    [HttpPut(ApiRoutes.Movies.Rate)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var successfullyDeleted = await _ratingService.DeleteRatingAsync(id, userId!.Value, token);

        if (!successfullyDeleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}
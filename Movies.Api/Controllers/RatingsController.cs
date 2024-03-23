using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api;
using Movies.Api.Auth.Extensions;
using Movies.Api.Mappers;
using Movies.Contracts.Requests.Queries.Queries.Application.Services;

namespace Movies.Api.Controllers;

[ApiController]
public class RatingsController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingsController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }
    
    [Authorize]
    [HttpGet(ApiRoutes.Ratings.GetAllRatings)]
    public async Task<IActionResult> GetAllRatings(CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var ratings = await _ratingService.GetRatingsForUserAsync(userId!.Value, token);
        return Ok(ratings.MapToMovieRatingResponse());
    }
}
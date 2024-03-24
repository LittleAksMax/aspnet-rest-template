using Movies.Api.Auth.Extensions;
using Movies.Api.Mappers;
using Movies.Api.Services;
using Movies.Application.Services;
using Movies.Contracts.Requests.Queries;
using Movies.Contracts.Responses;

namespace Movies.Api.Endpoints.Movies;

public static class GetAllMoviesEndpoint
{
    public static IEndpointRouteBuilder MapGetAllMovies(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Movies.GetAll, async (
            IMovieService movieService,
            IUriService uriService,
            [AsParameters] GetAllMoviesQuery query,
            [AsParameters] PagedQuery paginationQuery,
            HttpContext context,
            CancellationToken token
        ) =>
        {
            var userId = context.GetUserId();
            var options = query.MapToOptions(paginationQuery)
                .WithUser(userId);

            var movies = await movieService.GetAllAsync(options, token);
            var movieCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, token);
        
            // construct response
            var prev = await uriService.GetPrevPage(context, options.Page, options.PageSize);
            var next = await uriService.GetNextPage(context, options.Page, options.PageSize, movieCount);
            var response = movies.MapToMoviesResponse(paginationQuery, movieCount, prev, next);
        
            // set relevant headers
            context.Response.Headers.Append("X-Total-Count", movieCount.ToString());
        
            return TypedResults.Ok(response);
        })
            .Produces<MoviesResponse>(StatusCodes.Status200OK);
        return app;
    }
}
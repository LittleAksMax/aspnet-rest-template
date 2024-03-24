using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth.Constants;
using Movies.Api.Caching;
using Movies.Api.Versioning;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Movies;

public static class DeleteMovieEndpoint
{
    public static IEndpointRouteBuilder MapDeleteMovie(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Movies.Delete, async (
            IMovieService movieService,
            IOutputCacheStore cacheStore,
            [AsParameters] Guid id,
            CancellationToken token
        ) =>
        {
            var successfullyDeleted = await movieService.DeleteById(id, token);
            if (!successfullyDeleted)
            {
                return Results.NotFound();
            }

            // invalidate cache for this after successful creation
            await cacheStore.EvictByTagAsync(CachingConstants.MoviesCacheTag, token);
        
            return Results.NoContent();
        })
            .RequireAuthorization(AuthConstants.AdminPolicyName)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0);
        return app;
    }
}
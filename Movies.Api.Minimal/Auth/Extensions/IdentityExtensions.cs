using Movies.Api.Auth.Constants;

namespace Movies.Api.Auth.Extensions;

public static class IdentityExtensions
{
    public static Guid? GetUserId(this HttpContext context)
    {
        var userId = context.User.Claims
            .SingleOrDefault(x => x.Type == AuthConstants.UserIdClaimName);
        if (Guid.TryParse(userId?.Value, out var userGuid))
        {
            return userGuid;
        }

        return null;
    }
}
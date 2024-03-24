namespace Movies.Api.Auth.Constants;

public static class AuthConstants
{
    public const string AdminPolicyName = "Admin";
    public const string AdminClaimName = "admin";
    public const string TrustedMemberPolicyName = "Member";
    public const string TrustedMemberClaimName = "trusted_member";
    public const string UserIdClaimName = "userid";
    public const string ApiKeyHeaderName = "X-Api-Key";
    
    // this is not a good way to store this
    public const string AdminApiKeyUserIdString = "d8566de3-b1a6-4a9b-b842-8e3887a82e41";
}
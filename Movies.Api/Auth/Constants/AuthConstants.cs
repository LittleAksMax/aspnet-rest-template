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
    public const string AdminApiKeyUserIdString = "47327a49-172b-4f52-a849-e3fb01f58835";
}
using Microsoft.AspNetCore.Authorization;

namespace TaskManagement.API;

public static class AuthorizationPolicies
{
    public static void Configure(AuthorizationOptions options)
    {
        options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
        options.AddPolicy("RequireTeamLeadRole", policy => policy.RequireRole("TeamLead", "Administrator"));
    }
}

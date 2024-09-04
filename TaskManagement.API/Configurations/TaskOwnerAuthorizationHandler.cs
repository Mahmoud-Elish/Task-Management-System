using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TS = System.Threading.Tasks;

namespace TaskManagement.API.Configurations;

public class TaskOwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Task>
{
    protected override TS.Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Task resource)
    {
        var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        if (resource.AssignedUserId == userId ||
            context.User.IsInRole("TeamLead") ||
            context.User.IsInRole("Administrator"))
        {
            context.Succeed(requirement);
        }
        return TS.Task.CompletedTask;
    }
}
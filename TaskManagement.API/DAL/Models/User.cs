using Microsoft.AspNetCore.Identity;

namespace TaskManagement.API;

public class User : IdentityUser<int>
{
    public UserRole Role { get; set; }
    public ICollection<UserTeam> UserTeams { get; set; }
    public ICollection<Task> AssignedTasks { get; set; }
   
    public bool? IsEmailVerified { get; set; }
    public string? TwoFactorCode { get; set; }
    public DateTime? TwoFactorCodeExpiration { get; set; }
}

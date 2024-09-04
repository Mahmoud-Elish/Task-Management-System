namespace TaskManagement.API;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? TeamLeadId { get; set; }
    public User TeamLead { get; set; }
    public ICollection<UserTeam> UserTeams { get; set; }
    public ICollection<Task> Tasks { get; set; }
}

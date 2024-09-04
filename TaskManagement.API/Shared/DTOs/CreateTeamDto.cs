namespace TaskManagement.API;

public class CreateTeamDto
{
    public string Name { get; set; }
    public int TeamLeadId { get; set; }
    public int?[] UserIds { get; set; }
}

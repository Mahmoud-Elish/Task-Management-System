namespace TaskManagement.API;

public interface ITeamService
{
    Task<IEnumerable<TeamDto>> GetAllTeams();
    Task<UsersTeamDto> GetTeamById(int id);
    Task<TeamDto> CreateTeam(CreateTeamDto teamDto);
    Task<bool> AddUserToTeam(int teamId, int userId);
    Task<bool> RemoveUserFromTeam(int teamId, int userId);
}

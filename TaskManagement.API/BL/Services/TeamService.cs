using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TaskManagement.API;

public class TeamService : ITeamService
{
    private readonly TaskContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TeamService(TaskContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<TeamDto>> GetAllTeams()
    {
        return await _context.Teams.AsNoTracking().Where(t=>t.TeamLeadId == GetCurrentUserId())
            .Select(t => new TeamDto { Id = t.Id, Name = t.Name})
            .ToListAsync();
    }
    public async Task<UsersTeamDto> GetTeamById(int id)
    {
        var team = await _context.Teams.AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new UsersTeamDto
            (
                t.Id,
                t.Name,
                t.TeamLeadId.Value,
                t.UserTeams.Select(ut => new UserDto
                {
                    Id = ut.UserId,
                    Username = ut.User.UserName
                }).ToList()
            )).FirstOrDefaultAsync();

        return team;
    }
    public async Task<TeamDto> CreateTeam(CreateTeamDto teamDto)
    {
        var team = new Team { Name = teamDto.Name, TeamLeadId = teamDto.TeamLeadId };
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var teamMembers = teamDto.UserIds
            .Where(userId => userId.HasValue)
            .Select(userId => new UserTeam
            {
                UserId = userId.Value,
                TeamId = team.Id
            })
            .ToList();

        await _context.UserTeams.AddRangeAsync(teamMembers);
        await _context.SaveChangesAsync();

        return new TeamDto { Id = team.Id, Name = team.Name, TeamLeadId = team.TeamLeadId.Value };
    }
    public async Task<bool> AddUserToTeam(int teamId, int userId)
    {
        var userTeam = new UserTeam { UserId = userId, TeamId = teamId };
        _context.UserTeams.Add(userTeam);
        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<bool> RemoveUserFromTeam(int teamId, int userId)
    {
        var userTeam = await _context.UserTeams.FindAsync(userId, teamId);
        if (userTeam == null) return false;
        _context.UserTeams.Remove(userTeam);
        return await _context.SaveChangesAsync() > 0;
    }

    private int GetCurrentUserId()
    {
        var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        return userId;
    }
}

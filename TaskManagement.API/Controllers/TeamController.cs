using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.API.Controllers;


[Authorize(Policy = "RequireTeamLeadRole")]
public class TeamController : BaseApiController
{
    private readonly ITeamService _teamService;

    public TeamController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTeams()
    {
        try
        {
            var teams = await _teamService.GetAllTeams();
            return Ok(teams);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTeam(int id)
    {
        try
        {
            var team = await _teamService.GetTeamById(id);
            if (team == null) return NotFound();
            return Ok(team);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> CreateTeam(CreateTeamDto teamDto)
    {
        try
        {
            var team = await _teamService.CreateTeam(teamDto);
            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [HttpPost("{teamId}/users/{userId}")]
    public async Task<IActionResult> AddUserToTeam(int teamId, int userId)
    {
        try
        {
            var result = await _teamService.AddUserToTeam(teamId, userId);
            if (!result) return BadRequest("Failed to add user to team");
            return NoContent();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [HttpDelete("{teamId}/users/{userId}")]
    public async Task<IActionResult> RemoveUserFromTeam(int teamId, int userId)
    {
        try
        {
            var result = await _teamService.RemoveUserFromTeam(teamId, userId);
            if (!result) return NotFound();
            return NoContent();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
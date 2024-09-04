using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.API.Controllers;

[Authorize]
public class TaskController : BaseApiController
{
    private readonly ITaskService _taskService;
    private readonly IAuthorizationService _authorizationService;

    public TaskController(ITaskService taskService, IAuthorizationService authorizationService)
    {
        _taskService = taskService;
        _authorizationService = authorizationService;
    }
   
    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
        try
        {
            var tasks = await _taskService.GetTasksForCurrentUser();
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(int id)
    {
        try
        {
            var task = await _taskService.GetTaskById(id);
            if (task == null)
                return NotFound();
            return Ok(task);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateTaskDto taskDto)
    {
        try
        {
            var task = await _taskService.CreateTask(taskDto);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto taskDto)
    {
        try
        {
            var task = await _taskService.GetTaskById(id);
            if (task == null) return NotFound();

            if (!await _taskService.CheckTaskUser(id)) return Forbid();

            var result = await _taskService.UpdateTask(id, taskDto);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "RequireTeamLeadRole")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            var result = await _taskService.DeleteTask(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}

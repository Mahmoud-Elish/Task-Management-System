using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TaskManagement.API;

public class TaskService: ITaskService
{
    private readonly TaskContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TaskService(TaskContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<TaskResponseDto> GetTasksForCurrentUser()
    {
        var teamIds = await GetTasksAssignOnTeam();
        var (userId, userName) = GetCurrentUserInfo();
        var tasks = await _context.Tasks.AsNoTracking()
       .Where(t => t.AssignedUserId == userId || teamIds.Contains(t.TeamId.Value))
       .Select(t => new
       {
           t.Id,
           t.Title,
           t.Description,
           t.DueDate,
           t.Priority,
           t.Status
       })
       .ToListAsync();

        // Group tasks by status
        var taskGroups = tasks
            .GroupBy(t => t.Status)
            .ToDictionary(g => g.Key, g => g.Select(t => new TaskDto(
                t.Id,
                t.Title,
                t.Description,
                t.DueDate,
                Enum.GetName(typeof(TaskPriority), t.Priority),
                Enum.GetName(typeof(TaskStatus), t.Status)
            )).ToList());

        taskGroups.TryGetValue(TaskStatus.ToDo, out var toDoTasks);
        toDoTasks ??= new List<TaskDto>();

        taskGroups.TryGetValue(TaskStatus.InProgress, out var inProgressTasks);
        inProgressTasks ??= new List<TaskDto>();

        taskGroups.TryGetValue(TaskStatus.Completed, out var completedTasks);
        completedTasks ??= new List<TaskDto>();

        
        return new TaskResponseDto(
            userName,
            new TasksDto(toDoTasks, toDoTasks.Count()),
            new TasksDto(inProgressTasks,inProgressTasks.Count()),
            new TasksDto(completedTasks, completedTasks.Count())
  
        );
    }
    public async Task<TaskDto> GetTaskById(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return null;
        return new TaskDto
        (
            task.Id,
            task.Title,
            task.Description,
            task.DueDate,
            Enum.GetName(typeof(TaskPriority), task.Priority),
            Enum.GetName(typeof(TaskStatus), task.Status)
        );
    }
    public async Task<TaskDto> CreateTask(CreateTaskDto taskDto)
    {
        var task = new Task
        {
            Title = taskDto.Title,
            Description = taskDto.Description,
            DueDate = taskDto.DueDate,
            Priority = taskDto.Priority,
            Status = taskDto.Status,
            AssignedUserId = GetCurrentUserInfo().UserId
        };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return new TaskDto
       (
            task.Id,
            task.Title,
            task.Description,
            task.DueDate,
            Enum.GetName(typeof(TaskPriority), task.Priority),
            Enum.GetName(typeof(TaskStatus), task.Status)
        );
    }
    public async Task<TaskDto> UpdateTask(int id, UpdateTaskDto taskDto)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return null;
        task.Title = taskDto.Title;
        task.Description = taskDto.Description;
        task.DueDate = taskDto.DueDate;
        task.Priority = taskDto.Priority;
        task.Status = taskDto.Status;
        await _context.SaveChangesAsync();
        return new TaskDto
(
     task.Id,
     task.Title,
     task.Description,
     task.DueDate,
     Enum.GetName(typeof(TaskPriority), task.Priority),
     Enum.GetName(typeof(TaskStatus), task.Status)
 );
    }
    public async Task<bool> DeleteTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return false;
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> CheckTaskUser(int taskId)
    {
        var assignedUserId = await _context.Tasks
           .Where(t => t.Id == taskId)
           .Select(t => t.AssignedUserId)
           .FirstOrDefaultAsync();
        if (assignedUserId == GetCurrentUserInfo().UserId)
            return true;
        return false;
    }

    #region Privates
    private (int UserId, string UserName) GetCurrentUserInfo()
    {
        var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        return (userId, userName);
    }
    private Task<List<int>> GetTasksAssignOnTeam()
    {
        var teamIds = _context.UserTeams.AsNoTracking()
                    .Where(t => t.UserId == GetCurrentUserInfo().UserId).Select(x => x.TeamId).ToListAsync();
        return teamIds;
    }
    #endregion

    #region Comments
    public async Task<CommentInfoDto> AddCommentOnTask(CommentDto dto)
    {
        var task = await _context.Tasks.AnyAsync(t => t.Id == dto.TaskId);
        if (task == false)
            return null;

        var newComment = new Comment
        {
            TaskId = dto.TaskId,
            Content = dto.Content,
            CreatedAt = DateTime.Now,
            UserId = GetCurrentUserInfo().UserId
        };
        _context.Comments.Add(newComment);
        await _context.SaveChangesAsync();
        return new CommentInfoDto(dto.TaskId, dto.Content, newComment.CreatedAt.Value, GetCurrentUserInfo().UserName);

    }
    public async Task<IEnumerable<CommentInfoDto>> GetAllComments(int taskId)
    {
        var comments = await _context.Comments
        .Where(c => c.TaskId == taskId)
        .Select(c => new CommentInfoDto(c.Id, c.Content, c.CreatedAt.Value, c.User.UserName))
        .ToListAsync();

        return comments;

    }
    #endregion
}

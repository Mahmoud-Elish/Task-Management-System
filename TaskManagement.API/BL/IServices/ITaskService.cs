namespace TaskManagement.API;

public interface ITaskService
{
    Task<TaskResponseDto> GetTasksForCurrentUser();
    Task<TaskDto> GetTaskById(int id);
    Task<TaskDto> CreateTask(CreateTaskDto taskDto);
    Task<TaskDto> UpdateTask(int id, UpdateTaskDto taskDto);
    Task<bool> DeleteTask(int id);
    Task<bool> CheckTaskUser(int taskId);
    Task<CommentInfoDto> AddCommentOnTask(CommentDto dto);
    Task<IEnumerable<CommentInfoDto>> GetAllComments(int taskId);
}

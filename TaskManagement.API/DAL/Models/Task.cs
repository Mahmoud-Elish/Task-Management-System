

namespace TaskManagement.API;

public class Task
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public TaskPriority? Priority { get; set; }
    public TaskStatus? Status { get; set; }
    public int? AssignedUserId { get; set; }
    public User AssignedUser { get; set; }
    public int? TeamId { get; set; }
    public Team Team { get; set; }
    public int? DependentTaskId { get; set; }
    public Task DependentTask { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<Attachment> Attachments { get; set; }
}

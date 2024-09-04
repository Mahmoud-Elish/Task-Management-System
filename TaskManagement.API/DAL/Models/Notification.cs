namespace TaskManagement.API;

public class Notification
{
    public int Id { get; set; }
    public string? Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int? TaskId { get; set; }
    public Task Task { get; set; }
}

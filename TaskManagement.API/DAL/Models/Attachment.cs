namespace TaskManagement.API;

public class Attachment
{
    public int Id { get; set; }
    public string? FileName { get; set; }
    public string FilePath { get; set; }
    public long? FileSize { get; set; }
    public string? ContentType { get; set; }
    public DateTime? UploadedAt { get; set; }
    public int TaskId { get; set; }
    public Task Task { get; set; }
    public int? UploadedByUserId { get; set; }
    public User? UploadedByUser { get; set; }
}

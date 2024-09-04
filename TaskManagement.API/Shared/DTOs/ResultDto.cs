namespace TaskManagement.API;

public record ResultDto
(
     bool Success,
     string? Value = null
);

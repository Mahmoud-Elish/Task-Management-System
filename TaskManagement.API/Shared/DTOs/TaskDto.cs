namespace TaskManagement.API;

public record TaskDto
(
     int Id ,
     string Title ,
     string Description ,
     DateTime? DueDate ,
     string? Priority ,
     string? Status 
);

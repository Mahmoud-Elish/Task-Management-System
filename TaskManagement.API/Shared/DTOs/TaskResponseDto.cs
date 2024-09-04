namespace TaskManagement.API;

public record TaskResponseDto
(
     string? UserName ,
     TasksDto? ToDo ,
     TasksDto? InProgress ,
     TasksDto? Completed 
);

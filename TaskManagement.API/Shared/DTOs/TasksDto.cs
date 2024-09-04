namespace TaskManagement.API;

public record TasksDto
(
List<TaskDto>? Tasks,
int? Count = 0
);


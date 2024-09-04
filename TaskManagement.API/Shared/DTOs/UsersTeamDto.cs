namespace TaskManagement.API;

public record UsersTeamDto
(
int Id ,
string TeamName ,
int TeamLeadId ,
List<UserDto> Users 
);

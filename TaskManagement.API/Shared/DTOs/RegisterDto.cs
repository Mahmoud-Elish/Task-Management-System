﻿namespace TaskManagement.API;

public class RegisterDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserRole? Role { get; set; }
}

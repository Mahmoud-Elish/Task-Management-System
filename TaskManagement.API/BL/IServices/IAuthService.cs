
namespace TaskManagement.API;

public interface IAuthService
{
    Task<string> GenerateJwtToken(User user);
    Task<ResultDto> Login(LoginDto loginDTO);
    Task<ResultDto> Register(RegisterDto registerDTO);
}

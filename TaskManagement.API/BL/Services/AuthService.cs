using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TaskManagement.API;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<User> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<string> GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpirationInMinutes"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public async Task<ResultDto> Login(LoginDto loginDTO)
    {
        var user = await _userManager.FindByEmailAsync(loginDTO.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, loginDTO.Password))
        {
            var token = await GenerateJwtToken(user);
            return new ResultDto(true, token); 
        }
        return new ResultDto(false);
    }
    public async Task<ResultDto> Register(RegisterDto registerDTO)
    {
        var user = new User { UserName = registerDTO.Username, Email = registerDTO.Email, Role = registerDTO.Role?? UserRole.RegularUser };
        var result = await _userManager.CreateAsync(user, registerDTO.Password);

        if (result.Succeeded)
        {
            return new ResultDto(true);
        }
        return new ResultDto (Success : false, Value : string.Join(", ", result.Errors.Select(e => e.Description)) );
    }
}

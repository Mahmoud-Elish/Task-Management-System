using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailSender _emailSender;
    private readonly IAuthService _authService;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager,
        IConfiguration configuration, IEmailSender emailSender, IAuthService authService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _emailSender = emailSender;
        _authService = authService;
    }
    #region Normal Login & Registration 
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            var result = await _authService.Register(dto);
            if (result.Success)
            {
                return Ok(new { message = "User registered successfully" });
            }
            return BadRequest(new { message = result.Value });
        }
        catch (Exception ex)
        {
            throw ;
        }
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try
        {
            var result = await _authService.Login(dto);
            if (result.Success)
            {
                return Ok(new { token = result.Value });
            }
            return Unauthorized();
        }
        catch (Exception ex)
        {
            throw;
        }
       
    }


    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return BadRequest("Invalid request");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password has been reset successfully");
            }

            return BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            throw;
        }       
    }
    #endregion


    #region MORE ADV.
    [HttpPost("registerConfirmation")]
    public async Task<IActionResult> RegisterWithConfirmation([FromBody] RegisterDto dto)
    {
        var user = new User { UserName = dto.Email, Email = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, token = token }, Request.Scheme);

            await _emailSender.SendEmailAsync(dto.Email, "Confirm your email",
                $"Please confirm your account by clicking this link: {confirmationLink}", token);

            return Ok(new { message = "User created successfully. Please check your email for confirmation." });
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("loginTwoFactorToken")]
    public async Task<IActionResult> LoginTwoFactorToken([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) return Unauthorized();

        if (!(user.IsEmailVerified ?? false))
        {
            return BadRequest("Email not confirmed.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

        if (result.Succeeded)
        {
            if (await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return await GenerateTwoFactorToken(user);
            }

            return Ok(new { Token = await _authService.GenerateJwtToken(user) });
        }

        return Unauthorized();
    }
    private async Task<IActionResult> GenerateTwoFactorToken(User user)
    {
        var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
        user.TwoFactorCode = code;
        user.TwoFactorCodeExpiration = DateTime.UtcNow.AddMinutes(5);
        await _userManager.UpdateAsync(user);

        await _emailSender.SendEmailAsync(user.Email, "Your two-factor authentication code",
            $"Your two-factor authentication code is: {code}", code);

        return Ok(new { RequiresTwoFactor = true });
    }
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return BadRequest("User not found");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            user.IsEmailVerified = true;
            await _userManager.UpdateAsync(user);
            return Ok("Email confirmed successfully");
        }

        return BadRequest("Email confirmation failed");
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] string Email)
    {
        var user = await _userManager.FindByEmailAsync(Email);
        if (user == null || (!(user.IsEmailVerified ?? false)))
            return Ok("If your email is registered and verified, you will receive a password reset link.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = Url.Action("ResetPassword", "Auth", new { email = user.Email, token = token }, Request.Scheme);

        await _emailSender.SendEmailAsync(Email, "Reset your password",
            $"Please reset your password by clicking here: {resetLink}",token);

        return Ok("If your email is registered and verified, you will receive a password reset link.");
    }
    [HttpPost("verify-2fa")]
    public async Task<IActionResult> VerifyTwoFactorCode([FromBody] VerifyTwoFactorDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) return BadRequest("Invalid request");

        if (user.TwoFactorCode == dto.Code && user.TwoFactorCodeExpiration > DateTime.UtcNow)
        {
            user.TwoFactorCode = null;
            user.TwoFactorCodeExpiration = null;
            await _userManager.UpdateAsync(user);

            return Ok(new { Token =  await _authService.GenerateJwtToken(user) });
        }

        return BadRequest("Invalid or expired code");
    }
    #endregion
}

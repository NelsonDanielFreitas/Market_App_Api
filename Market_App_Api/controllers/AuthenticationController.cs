using MarkerAPI.DTO;
using MarkerAPI.Models;
using MarkerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarkerAPI.controllers;

[Route("api/auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly AuthenticationService _authenticationService;
    private readonly JwtService _jwtService;

    public AuthenticationController(AuthenticationService authenticationService, JwtService jwtService)
    {
        _authenticationService = authenticationService;
        _jwtService = jwtService;
    }   
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO login)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Invalid data" });
        }

        var result = await _authenticationService.ValidateUserAsync(login);

        if (result.Locked)
        {
            return BadRequest(new { message = $"Too many failed attempts. Try again after {result.LockoutTimeLeft} minutes." });
        }

        if (result.User == null)
        {
            return BadRequest(new { message = "Invalid Email or password" });
        }

        if (!result.User.EmailVerified)
        {
            return BadRequest(
                new { message = "Email not verified. Please check your email for the verification code." });
        }

        var accessToken = _jwtService.GenerateAccessToken(result.User.Id.ToString(), result.User.Email, result.User.Role);

        var UserSend = new User
        {
            Email = result.User.Email,
            Role = result.User.Role
        };

        Response.Cookies.Append("refreshToken", result.User.refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        return Ok(new { AccessToken = accessToken, RefreshToken = result.User.refreshToken, User = new { result.User.Email, result.User.Role } });;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Invalid data" });
        }

        var isRegister = await _authenticationService.RegisterUserAsync(request);

        if (isRegister == false)
        {
            return BadRequest(new { message = "Email already exists" });
        }

        return Ok(new { message = "Register successfully. Check your email for verification code." });
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDTO request)
    {
        var result = await _authenticationService.VerifyEmailAsync(request);
        if (!result)
        {
            return BadRequest(new { message = "Invalid verification code" });
        }

        return Ok(new { message = "Email verified successfully" });
    }
}
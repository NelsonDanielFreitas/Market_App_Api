using System.Net;
using System.Net.Mail;
using MarkerAPI.Data;
using MarkerAPI.DTO;
using MarkerAPI.Models;
//using Market_App_API.Helper;
using Microsoft.EntityFrameworkCore;

namespace MarkerAPI.Services;

public class AuthenticationService
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtTokenService;
    private readonly IConfiguration _config;
    
    public AuthenticationService(AppDbContext context, JwtService jwtTokenService, IConfiguration configuration)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _config = configuration;
    }
    
    public async Task<bool> RegisterUserAsync(RegisterDTO registerDto)
    {
        //Validação se já existe um Utilizador com o email criado
        if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
        {
            return false;
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
        var verificationCode = new Random().Next(100000, 999999).ToString();
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Name = registerDto.Name,
            Email = registerDto.Email,
            Password = passwordHash,
            Role = "User",
            EmailVerificationCode = verificationCode,
            EmailVerified = false
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        //var testEnv = _config["Email:Username"];
        SendVerificationEmail(user.Email, verificationCode);

        return true;
    }
    
    //Função para enviar o email com o código
    private void  SendVerificationEmail(string email, string code)
    {
        var fromAddress = new MailAddress(_config["Email:Username"], "Your App");
        var toAddress = new MailAddress(email);
        const string subject = "Email Verification";
        string body = $"Your verification code is: {code}";

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            Credentials = new NetworkCredential(_config["Email:Username"], _config["Email:Password"])
        };

        using var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body,
        };
        smtp.Send(message);
    }
    
    //Função para validar o email
    public async Task<bool> VerifyEmailAsync(VerifyEmailDTO verifyEmail)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == verifyEmail.Email);
        if (user == null || user.EmailVerificationCode != verifyEmail.Code)
        {
            return false;
        }

        user.EmailVerified = true;
        user.EmailVerificationCode = null;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<AuthenticationResult> ValidateUserAsync(UserLoginDTO login)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == login.Email);

        if (user == null)
        {
            return new AuthenticationResult { User = null };
        }

        if (user.LockoutEndTime.HasValue && user.LockoutEndTime > DateTime.UtcNow)
        {
            return new AuthenticationResult
            {
                Locked = true,
                LockoutTimeLeft = (user.LockoutEndTime.Value - DateTime.UtcNow).Minutes
            };
        }

        if (!BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
        {
            user.FailedLoginAttempts++;

            if (user.FailedLoginAttempts >= user.MaxFailedAttempts)
            {
                user.LockoutEndTime = DateTime.UtcNow.Add(user.LockoutDuration);
                await _context.SaveChangesAsync();
                return new AuthenticationResult
                {
                    Locked = true,
                    LockoutTimeLeft = user.LockoutDuration.TotalMinutes
                };
            }

            await _context.SaveChangesAsync();
            return new AuthenticationResult { User = null };
        }

        user.FailedLoginAttempts = 0;
        user.LockoutEndTime = null;

        user.refreshToken = _jwtTokenService.GenerateRefreshToken();
        user.refreshTokenExpiryTime = _jwtTokenService.GetRefreshTokenExpiry();
        await _context.SaveChangesAsync();

        return new AuthenticationResult { User = user };
    }
}
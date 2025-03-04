using System.Net;
using System.Net.Mail;
using System.Text;
using MarkerAPI.Data;
using MarkerAPI.DTO;
using MarkerAPI.Models;
using Market_App_Api.Helper;
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
        if (await _context.User.AnyAsync(u => u.Email == registerDto.Email))
        {
            return false;
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
        var verificationCode = new Random().Next(100000, 999999).ToString();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = registerDto.Name,
            Email = registerDto.Email,
            Password = passwordHash,
            Role = "User",
            EmailVerificationCode = verificationCode,
            EmailVerified = false
        };

        _context.User.Add(user);
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
        var user = await _context.User.SingleOrDefaultAsync(u => u.Email == verifyEmail.Email);
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
        var user = await _context.User.SingleOrDefaultAsync(u => u.Email == login.Email);

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
        
        var plainRefreshToken = _jwtTokenService.GenerateRefreshToken();
        
        string encryptionKeyString = _config["Encryption:RefreshTokenKey"];
        byte[] encryptionKey = Encoding.UTF8.GetBytes(encryptionKeyString);
            
        // Encrypt the refresh token using AES-256
        user.refreshToken = AesEncryption.Encrypt(plainRefreshToken, encryptionKey);
        user.refreshTokenExpiryTime = _jwtTokenService.GetRefreshTokenExpiry();
        /*user.refreshToken = _jwtTokenService.GenerateRefreshToken();
        user.refreshTokenExpiryTime = _jwtTokenService.GetRefreshTokenExpiry();*/
        await _context.SaveChangesAsync();

        return new AuthenticationResult { User = user };
    }
    
    
    public async Task<RefreshTokenDTO?> RefreshTokenAsync(string providedEncryptedRefreshToken)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.refreshToken == providedEncryptedRefreshToken);
        if (user == null)
        {
            return null; 
        }
            
        if (user.refreshTokenExpiryTime < DateTime.UtcNow)
        {
            return null; 
        }
            
        var newAccessToken = _jwtTokenService.GenerateAccessToken(user.Id.ToString(), user.Email, user.Role);
            
        var newPlainRefreshToken = _jwtTokenService.GenerateRefreshToken();
            
        string encryptionKeyString = _config["Encryption:RefreshTokenKey"];
        byte[] encryptionKey = Encoding.UTF8.GetBytes(encryptionKeyString);
        /*var newEncryptedRefreshToken = AesEncryption.Encrypt(newPlainRefreshToken, encryptionKey);
            
        user.refreshToken = newEncryptedRefreshToken;*/
        user.refreshTokenExpiryTime = _jwtTokenService.GetRefreshTokenExpiry();
        await _context.SaveChangesAsync();
            
        return new RefreshTokenDTO
        {
            AccessToken = newAccessToken,
            //EncryptedRefreshToken = newEncryptedRefreshToken
            EncryptedRefreshToken = user.refreshToken
        };
    }
}
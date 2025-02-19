using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkerAPI.Models;

public class User
{
    [Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.None)] 
    public int Id { get; set; }
    
    public Guid UserId { get; set; }
    
    [Required]
    [MaxLength(100), EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Password { get; set; }
    
    public string? Role { get; set; }
    
    public string? refreshToken { get; set; }
    
    public DateTime? refreshTokenExpiryTime { get; set; }
    // Track failed login attempts
    public int FailedLoginAttempts { get; set; } = 0;

    // Lockout end time
    public DateTime? LockoutEndTime { get; set; }

    // Maximum allowed failed attempts
    public int MaxFailedAttempts { get; set; } = 5;

    // Lockout duration
    public TimeSpan LockoutDuration { get; set; } = TimeSpan.FromMinutes(15);
    
    public string? EmailVerificationCode { get; set; }
    
    public bool EmailVerified { get; set; } = false;
}
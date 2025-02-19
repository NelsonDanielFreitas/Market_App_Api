using MarkerAPI.Models;

namespace MarkerAPI.DTO;

public class AuthenticationResult
{
    public User? User { get; set; }
    public bool Locked { get; set; } = false;
    public double LockoutTimeLeft { get; set; } = 0;
}
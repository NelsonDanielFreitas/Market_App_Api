using System.ComponentModel.DataAnnotations;

namespace MarkerAPI.DTO;

public class RegisterDTO
{
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    
    [Required]
    public string Name { get; set; }
    
}
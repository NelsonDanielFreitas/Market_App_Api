using System.ComponentModel.DataAnnotations;

namespace MarkerAPI.DTO;

public class VerifyEmailDTO
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Code { get; set; }
}
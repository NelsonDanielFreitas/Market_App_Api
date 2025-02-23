using System.ComponentModel.DataAnnotations;

namespace MarkerAPI.Models;

public class TesteDatabase
{
    [Key]
    public Guid Id { get; set; }
}
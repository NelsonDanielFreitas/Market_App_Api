using System.ComponentModel.DataAnnotations;

namespace MarkerAPI.Models;

public class Category
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public List<Product> Products { get; set; } = new();
}
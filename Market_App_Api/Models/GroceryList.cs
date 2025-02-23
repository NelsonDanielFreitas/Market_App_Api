using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkerAPI.Models;

public class GroceryList
{
    [Key]
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public Guid UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; }
    
    public List<GroceryItem> Items { get; set; } = new();
    
    public decimal TotalPrice { get; set; }
    
    public string Status { get; set; }
}
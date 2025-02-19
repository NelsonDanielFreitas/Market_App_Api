using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkerAPI.Models;

public class GroceryList
{
    [Key]
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public int UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; }
    
    public List<GroceryItem> Items { get; set; } = new();
}
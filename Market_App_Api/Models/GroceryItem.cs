using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkerAPI.Models;

public class GroceryItem
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid GroceryListId { get; set; }
    
    [ForeignKey("GroceryListId")]
    public GroceryList GroceryList { get; set; }
    
    public Guid ProductId { get; set; }
    
    [ForeignKey("ProductId")]
    public Product Product { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal Price { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkerAPI.Models;

public class GroceryItem
{
    [Key]
    public int Id { get; set; }
    
    public int GroceryListId { get; set; }
    
    [ForeignKey("GroceryListId")]
    public GroceryList GroceryList { get; set; }
    
    public int ProductId { get; set; }
    
    [ForeignKey("ProductId")]
    public Product Product { get; set; }
    
    public int Quantity { get; set; }
}
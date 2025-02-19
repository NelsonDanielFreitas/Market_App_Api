using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkerAPI.Models;

public class Expense
{
    [Key]
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; }
    
    public decimal Amount { get; set; }
    
    public string Category { get; set; }
    
    public DateTime Date { get; set; }
}
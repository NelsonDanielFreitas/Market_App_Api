using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkerAPI.Models;

public class Expense
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    [ForeignKey("Id")]
    public User User { get; set; }
    
    public decimal Amount { get; set; }
    
    public string Category { get; set; }
    
    public DateTime Date { get; set; }
}
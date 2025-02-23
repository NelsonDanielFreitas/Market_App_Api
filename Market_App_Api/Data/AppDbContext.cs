using MarkerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MarkerAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    
    public DbSet<User> User { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<GroceryList> GroceryLists { get; set; }
    public DbSet<GroceryItem> GroceryItems { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<TesteDatabase> TesteDatabase { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Barcode)
            .IsUnique();
    }
}
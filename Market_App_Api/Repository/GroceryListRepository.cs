using MarkerAPI.Data;
using MarkerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Market_App_Api.Repository;

public interface IGroceryListRepository
{
    Task<GroceryList> AddGroceryListAsync(GroceryList groceryList);
    /*Task<GroceryList> GetGroceryListByIdAsync(int id);*/
    Task<List<GroceryList>> GetGroceryListsByUserIdAsync(Guid userId);
}

public class GroceryListRepository : IGroceryListRepository
{
    private readonly AppDbContext _context;

    public GroceryListRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GroceryList> AddGroceryListAsync(GroceryList groceryList)
    {
        _context.GroceryLists.Add(groceryList);
        await _context.SaveChangesAsync();
        return groceryList;
    }

    /*public async Task<GroceryList> GetGroceryListByIdAsync(int id)
    {
        return await _context.GroceryLists
            .Include(gl => gl.Items)
            .ThenInclude(gi => gi.Product)
            .FirstOrDefaultAsync(gl => gl.Id == id);
    }*/

    public async Task<List<GroceryList>> GetGroceryListsByUserIdAsync(Guid userId)
    {
        return await _context.GroceryLists
            .Include(gl => gl.Items)
            .ThenInclude(item => item.Product)
            .Where(gl => gl.UserId == userId)
            .ToListAsync();
    }
}

using MarkerAPI.DTO.GroceryList;
using MarkerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarkerAPI.controllers;
[Route("api/[controller]")]
[ApiController]
public class GroceryListController : ControllerBase
{
    private readonly IGroceryListService _groceryListService;

    public GroceryListController(IGroceryListService groceryListService)
    {
        _groceryListService = groceryListService;
    }

    [HttpPost("CreateGroceryList")]
    public async Task<IActionResult> CreateGroceryList([FromBody] GroceryListDTO groceryListDTO)
    {
        if (groceryListDTO == null)
        {
            return BadRequest("Invalid data.");
        }

        var createdGroceryList = await _groceryListService.CreateGroceryListAsync(groceryListDTO);

        return Ok(new { message = "Grocery list created successfully", data = createdGroceryList });
    }
    
    [HttpGet("GetGroceryListByUserId/{userId}")]
    public async Task<IActionResult> GetGroceryListByUserId(Guid userId)
    {
        var groceryLists = await _groceryListService.GetGroceryListByUserIdAsync(userId);
        if (groceryLists == null || !groceryLists.Any())
        {
            return NotFound(new { message = "No grocery lists found for this user." });
        }

        return Ok(groceryLists);
    }
    
}
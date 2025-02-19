using MarkerAPI.DTO.Category;
using MarkerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarkerAPI.controllers;
[Route("api/category")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost("AddCategory")]
    public async Task<IActionResult> AddCategory([FromBody] AddCategoryDTO categoryDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Invalid data" });
        }

        var category = await _categoryService.AddCategoryAsync(categoryDto);

        if (category == false)
        {
            return BadRequest(new { message = "Category already exists" });
        }

        return Ok(new { message = "Category added" });
    }

    [HttpGet("GetAllCategories")]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = _categoryService.GetAllCategoriesAsync();

        return Ok(new { Category = categories });
    }
}
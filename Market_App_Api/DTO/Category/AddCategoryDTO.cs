using System.ComponentModel.DataAnnotations;

namespace MarkerAPI.DTO.Category;

public class AddCategoryDTO
{
    [Required]
    public string Name { get; set; }
}
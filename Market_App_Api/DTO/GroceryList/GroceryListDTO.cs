namespace MarkerAPI.DTO.GroceryList;

public class GroceryListDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid UserId { get; set; }
    public List<GroceryItemDTO> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
}

public class GroceryItemDTO
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
using MarkerAPI.DTO.GroceryList;
using MarkerAPI.Models;
using Market_App_Api.Repository;

namespace MarkerAPI.Services;

public interface IGroceryListService
{
    Task<GroceryListDTO> CreateGroceryListAsync(GroceryListDTO groceryListDTO);
    Task<List<GroceryListDTO>> GetGroceryListByUserIdAsync(Guid userId);
}

public class GroceryListService : IGroceryListService
{
    private readonly IGroceryListRepository _groceryListRepository;
    private readonly IProductRepository _productRepository;

    public GroceryListService(IGroceryListRepository groceryListRepository, IProductRepository productRepository)
    {
        _groceryListRepository = groceryListRepository;
        _productRepository = productRepository;
    }

    public async Task<GroceryListDTO> CreateGroceryListAsync(GroceryListDTO groceryListDTO)
    {
        var groceryList = new GroceryList
        {
            Name = groceryListDTO.Name,
            UserId = groceryListDTO.UserId,
            Status = "Pending"
        };

        foreach (var itemDTO in groceryListDTO.Items)
        {
            var product = await _productRepository.GetProductByIdAsync(itemDTO.ProductId);
            if (product != null)
            {
                groceryList.Items.Add(new GroceryItem
                {
                    Product = product,
                    ProductId = product.Id,
                    Quantity = itemDTO.Quantity,
                    // The price can be provided or pulled from the product itself
                    // If frontend sends price, we will use that one.
                    Price = itemDTO.Price
                });
            }
        }

        groceryList.TotalPrice = groceryList.Items.Sum(item => item.Price * item.Quantity);

        await _groceryListRepository.AddGroceryListAsync(groceryList);

        return groceryListDTO;
    }
    
    //Para obter as listas por Id de utilizador
    public async Task<List<GroceryListDTO>> GetGroceryListByUserIdAsync(Guid userId)
    {
        var groceryLists = await _groceryListRepository.GetGroceryListsByUserIdAsync(userId);

        var groceryListDTOs = groceryLists.Select(gl => new GroceryListDTO
        {
            Id = gl.Id,
            Name = gl.Name,
            UserId = gl.UserId,
            TotalPrice = gl.TotalPrice,
            Status = gl.Status,
            Items = gl.Items.Select(item => new GroceryItemDTO
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price
            }).ToList()
        }).ToList();

        return groceryListDTOs;
    }
}
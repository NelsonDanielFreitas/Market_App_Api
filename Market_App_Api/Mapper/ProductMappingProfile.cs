using AutoMapper;
using MarkerAPI.DTO.Category;
using MarkerAPI.DTO.Product;
using MarkerAPI.Models;
namespace Market_App_Api.Mapper;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDTO>();
        CreateMap<Category, CategoryDTO>();
    }
}
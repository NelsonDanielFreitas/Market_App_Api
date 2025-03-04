using AutoMapper;
using MarkerAPI.DTO.Product;
using MarkerAPI.Models;
using MarkerAPI.Services;
using MarkerAPI.Services.Caching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace MarkerAPI.controllers;

[Route("api/product")]
//[Authorize]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly IRedisCacheService _cache;
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;

    public ProductController(ProductService produdctService, IRedisCacheService cache, IMapper mapper, HttpClient httpClient)
    {
        _productService = produdctService;
        _cache = cache;
        _mapper = mapper;
        _httpClient = httpClient;
    }

    [HttpPost("AddPrroduct")]
    public async Task<IActionResult> AddProduct([FromBody] AddProductDTO addProductDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Invalid data" });
        }

        var checkCategorie = _productService.CheckCategorie(addProductDto);

        if (checkCategorie.Result == true)
        {
            return BadRequest(new { message = "Category does not exist!!" });
        }

        var checkBarcode = _productService.CheckBarcode(addProductDto.Barcode);

        if (checkBarcode.Result)
        {
            return BadRequest(new { message = "Barcode already exists!!" });
        }

        var product = await _productService.AddProductAsync(addProductDto);
        
        _cache.RemoveData("product");
        
        return Ok(new { message = "Product added!!", Product = product});
    }

    [HttpGet("GetAllProducts")]
    public async Task<IActionResult> GetAllProducts()
    {
        ////////////////////////////////////////////
        /// Para se quiser guardar em cache produtos por utilizador
        /// ////////////////////////////////////////
        //var userId = Request.Headers["UserId"];
        //var cachingKey = $"product_{userId}";
        
        var cacheKey = "product";
        var products = _cache.GetData<IEnumerable<Product>>(cacheKey);

        if (products is not null)
        {
            var cachedProductsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);
            return Ok(new { message = "Get all products success", product = cachedProductsDto });
        }

        products = await _productService.GetAllProductsAsync();

        if (!products.Any())
        {
            return NotFound(new { message = "No products found" });
        }

        var productDtos = _mapper.Map<IEnumerable<ProductDTO>>(products);
    
        _cache.SetData(cacheKey, productDtos);

        return Ok(new { message = "Get all products success", product = productDtos });
    }

    [HttpDelete("DeleteProduct/{productId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(Guid productId)
    {
        var result = await _productService.DeleteProductAsync(productId);

        if (!result)
        {
            return BadRequest(new { message = "Product not found" });
        }
        
        _cache.RemoveData("product");

        return Ok(new { message = "Product deleted" });
    }

    [HttpGet("GetProductById/{productId}")]
    public async Task<IActionResult> GetProductById(Guid productId)
    {
        var cacheKey = $"product_{productId}";
        var product = _cache.GetData<Product>(cacheKey);

        if (product is not null)
        {
            return Ok(new { message = "Get product success", product = product });
        }

        product = await _productService.GetProductByIdAsync(productId);

        if (product is null)
        {
            return NotFound(new { message = "Product not found" });
        }

        var productDtoObj = _mapper.Map<ProductDTO>(product);
        
        _cache.SetData(cacheKey, productDtoObj);

        return Ok(new { message = "Product retrieved", product = productDtoObj });
    }

    //5601151131459
    [HttpPost("CreateProductByBarcode/{barcode}")]
    public async Task<IActionResult> CreateProductByBarcode(string barcode)
    {
        if (barcode == null)
        {
            return BadRequest(new { message = "Invalid Data" });
        }
        
        var checkBarcode = _productService.CheckBarcode(barcode);

        if (checkBarcode.Result)
        {
            return BadRequest(new { message = "Barcode already exists!!" });
        }

        Product product = await _productService.CreateProductWithBarcode(barcode);

        if (product.Barcode == String.Empty)
        {
            return BadRequest(new { message = "Error in adding product" });
        }
        _cache.RemoveData("product");
        return Ok(new { message = "Product added", product = product });
    }

    [HttpGet("GetProductByBarcode/{barcode}")]
    public async Task<IActionResult> GetProductByBarcode(string barcode)
    {
        var cacheKey = $"product_{barcode}";
        var product = _cache.GetData<Product>(cacheKey);

        if (product is not null)
        {
            return Ok(new { message = "Get product success", product = product });
        }

        product = await _productService.GetProductByBarcodeAsync(barcode);

        if (product is null)
        {
            return NotFound(new { message = "Product not found" });
        }

        var productDtoObj = _mapper.Map<ProductDTO>(product);
        
        _cache.SetData(cacheKey, productDtoObj);

        return Ok(new { message = "Product retrieved", product = productDtoObj });
    }
}
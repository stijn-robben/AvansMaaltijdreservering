using Microsoft.AspNetCore.Mvc;
using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Interfaces;

namespace AvansMaaltijdreservering.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        try
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }

    /// <summary>
    /// Get specific product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            
            if (product == null)
                return NotFound(new { message = $"Product with ID {id} not found" });

            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }

    /// <summary>
    /// Get products for a specific package
    /// </summary>
    [HttpGet("package/{packageId}")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByPackage(int packageId)
    {
        try
        {
            var products = await _productRepository.GetProductsByPackageIdAsync(packageId);
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }
}
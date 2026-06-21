using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService) => _productService = productService;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetProducts(
        [FromQuery] string? search,
        [FromQuery] string? manufacturer,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? sortBy,
        [FromQuery] bool desc = false,
        CancellationToken cancellationToken = default)
    {
        var products = await _productService.GetProductsAsync(
            new ProductQuery(search, manufacturer, minPrice, maxPrice, sortBy, desc),
            cancellationToken);

        return Ok(products.Select(ProductMappings.ToResponse).ToList());
    }

    [HttpGet("manufacturers")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<string>>> GetManufacturers(CancellationToken cancellationToken = default)
    {
        return Ok(await _productService.GetManufacturersAsync(cancellationToken));
    }
}

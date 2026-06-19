using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Entities;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDatabaseContext _context;

        public ProductsController(AppDatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить список товаров с фильтрацией и сортировкой
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] string? search,
            [FromQuery] string? manufacturer,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? sortBy,
            [FromQuery] bool desc = false)
        {
            try
            {
                var query = _context.Products.AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(p => p.Name.Contains(search));

                if (!string.IsNullOrWhiteSpace(manufacturer))
                    query = query.Where(p => p.Manufacturer == manufacturer);

                if (minPrice.HasValue)
                    query = query.Where(p => p.Price >= minPrice.Value);
                if (maxPrice.HasValue)
                    query = query.Where(p => p.Price <= maxPrice.Value);

                query = sortBy?.ToLower() switch
                {
                    "name" => desc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                    "price" => desc ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                    _ => query.OrderBy(p => p.Name)
                };

                var products = await query.ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить список всех производителей
        /// </summary>
        [HttpGet("manufacturers")]
        public async Task<IActionResult> GetManufacturers()
        {
            try
            {
                var manufacturers = await _context.Products
                    .Select(p => p.Manufacturer)
                    .Distinct()
                    .OrderBy(m => m)
                    .ToListAsync();
                return Ok(manufacturers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }
    }
}
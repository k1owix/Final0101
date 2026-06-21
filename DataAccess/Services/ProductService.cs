using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Services;

public sealed class ProductService : IProductService
{
    private readonly AppDatabaseContext _context;

    public ProductService(AppDatabaseContext context) => _context = context;

    public async Task<IReadOnlyList<Product>> GetProductsAsync(ProductQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.MinPrice.HasValue && query.MaxPrice.HasValue && query.MinPrice > query.MaxPrice)
        {
            return [];
        }

        IQueryable<Product> productsQuery = _context.Products
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            productsQuery = productsQuery.Where(product => product.ProductName.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.Manufacturer))
        {
            var manufacturer = query.Manufacturer.Trim();
            productsQuery = productsQuery.Where(product => product.ManufacturerName == manufacturer);
        }

        if (query.MinPrice.HasValue)
        {
            productsQuery = productsQuery.Where(product => product.Price >= query.MinPrice.Value);
        }

        if (query.MaxPrice.HasValue)
        {
            productsQuery = productsQuery.Where(product => product.Price <= query.MaxPrice.Value);
        }

        var sortBy = query.SortBy?.Trim().ToLowerInvariant();
        productsQuery = sortBy switch
        {
            "price" => query.Descending
                ? productsQuery.OrderByDescending(product => product.Price).ThenBy(product => product.ProductName)
                : productsQuery.OrderBy(product => product.Price).ThenBy(product => product.ProductName),
            _ => query.Descending
                ? productsQuery.OrderByDescending(product => product.ProductName)
                : productsQuery.OrderBy(product => product.ProductName)
        };

        return await productsQuery.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetManufacturersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Select(product => product.ManufacturerName)
            .Distinct()
            .OrderBy(name => name)
            .ToListAsync(cancellationToken);
    }
}

using DataAccess.Models;

namespace DataAccess.Services;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetProductsAsync(ProductQuery query, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetManufacturersAsync(CancellationToken cancellationToken = default);
}

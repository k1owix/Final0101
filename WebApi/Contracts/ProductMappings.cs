using DataAccess.Models;

namespace WebApi.Contracts;

public static class ProductMappings
{
    public static ProductResponse ToResponse(Product product)
    {
        return new ProductResponse(
            product.ProductId,
            product.Article,
            product.ProductName,
            product.Unit,
            product.Price,
            product.Author,
            product.ManufacturerName,
            product.CategoryName,
            product.Discount,
            product.StockQuantity,
            product.Description,
            product.PhotoFileName);
    }
}

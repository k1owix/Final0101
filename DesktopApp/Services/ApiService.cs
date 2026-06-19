using DesktopApp.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace DesktopApp.Services
{
    public interface IApiService
    {
        Task<List<Product>> GetProductsAsync(string? search = null, string? manufacturer = null,
            decimal? minPrice = null, decimal? maxPrice = null,
            string? sortBy = null, bool desc = false);

        Task<List<string>> GetManufacturersAsync();
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(string baseUrl = "https://localhost:5001")
        {
            _httpClient = new HttpClient { BaseAddress = new System.Uri(baseUrl) };
        }

        public async Task<List<Product>> GetProductsAsync(string? search = null, string? manufacturer = null,
            decimal? minPrice = null, decimal? maxPrice = null,
            string? sortBy = null, bool desc = false)
        {
            var query = new List<string>();
            if (!string.IsNullOrEmpty(search)) query.Add($"search={Uri.EscapeDataString(search)}");
            if (!string.IsNullOrEmpty(manufacturer)) query.Add($"manufacturer={Uri.EscapeDataString(manufacturer)}");
            if (minPrice.HasValue) query.Add($"minPrice={minPrice.Value}");
            if (maxPrice.HasValue) query.Add($"maxPrice={maxPrice.Value}");
            if (!string.IsNullOrEmpty(sortBy)) query.Add($"sortBy={sortBy}");
            if (desc) query.Add("desc=true");

            var url = "api/products";
            if (query.Count > 0) url += "?" + string.Join("&", query);

            return await _httpClient.GetFromJsonAsync<List<Product>>(url) ?? new();
        }

        public async Task<List<string>> GetManufacturersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<string>>("api/products/manufacturers") ?? new();
        }
    }
}
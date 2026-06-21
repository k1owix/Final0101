using DesktopApp.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace DesktopApp.Services;

public sealed class ApiService
{
    private static readonly HttpClient[] Clients =
    [
        CreateClient("https://localhost:7046/"),
        CreateClient("http://localhost:5022/")
    ];

    public static async Task<IReadOnlyList<Product>> GetProductsAsync() => await SendAndReadAsync<List<Product>>(
                   (client, token) => client.GetAsync("api/products", token),
                   "Could not load products.")
               ?? [];

    public static async Task<AuthenticatedUser> LoginAsync(string login, string password)
    {
        using var response = await SendAsync(
            (client, token) => client.PostAsJsonAsync("api/users/login", new { Login = login, Password = password }, token),
            "Login failed.");

        return await ReadJsonAsync<AuthenticatedUser>(response, "Server returned an empty login response.");
    }

    public static async Task<CreatedOrder> CreateOrderAsync(OrderCreateRequest request)
    {
        using var response = await SendAsync(
            (client, token) => client.PostAsJsonAsync("api/orders", request, token),
            "Order creation failed.");

        return await ReadJsonAsync<CreatedOrder>(response, "Server returned an empty order response.");
    }

    public static async Task<IReadOnlyList<OrderSummary>> GetOrdersAsync() => await SendAndReadAsync<List<OrderSummary>>(
                   (client, token) => client.GetAsync("api/orders", token),
                   "Could not load orders.")
               ?? [];

    public async Task UpdateOrderAsync(int orderId, OrderUpdateRequest request)
    {
        using var response = await SendAsync(
            (client, token) => client.PutAsJsonAsync($"api/orders/{orderId}", request, token),
            "Could not update order.");

        await EnsureSuccessAsync(response, "Could not update order.");
    }

    private static async Task<T?> SendAndReadAsync<T>(
        Func<HttpClient, CancellationToken, Task<HttpResponseMessage>> send,
        string fallbackMessage)
    {
        using var response = await SendAsync(send, fallbackMessage);
        return await ReadJsonAsync<T>(response, fallbackMessage);
    }

    private static async Task<HttpResponseMessage> SendAsync(
        Func<HttpClient, CancellationToken, Task<HttpResponseMessage>> send,
        string fallbackMessage)
    {
        Exception? lastException = null;

        foreach (var client in Clients)
        {
            try
            {
                var response = await send(client, CancellationToken.None);
                await EnsureSuccessAsync(response, fallbackMessage);
                return response;
            }
            catch (HttpRequestException ex)
            {
                lastException = ex;
            }
            catch (TaskCanceledException ex)
            {
                lastException = ex;
            }
        }

        throw new InvalidOperationException("Could not connect to the Web API over HTTPS or HTTP.", lastException);
    }

    private static async Task<T> ReadJsonAsync<T>(HttpResponseMessage response, string fallbackMessage)
    {
        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidOperationException(fallbackMessage);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, string fallbackMessage)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        throw new InvalidOperationException(await ReadErrorMessageAsync(response, fallbackMessage));
    }

    private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response, string fallbackMessage)
    {
        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ApiProblem>();
            if (!string.IsNullOrWhiteSpace(problem?.Detail))
            {
                return problem.Detail;
            }

            if (!string.IsNullOrWhiteSpace(problem?.Message))
            {
                return problem.Message;
            }
        }
        catch
        {
        }

        var serverMessage = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(serverMessage) ? fallbackMessage : serverMessage;
    }

    private static HttpClient CreateClient(string baseAddress)
    {
        return new HttpClient
        {
            BaseAddress = new Uri(baseAddress)
        };
    }

    private sealed class ApiProblem
    {
        public string? Detail { get; init; }
        public string? Message { get; init; }
    }
}

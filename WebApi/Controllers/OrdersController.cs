using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService) => _orderService = orderService;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OrderResponse>>> GetAll(CancellationToken cancellationToken = default)
    {
        var orders = await _orderService.GetOrdersAsync(cancellationToken);
        return Ok(orders.Select(OrderMappings.ToResponse).ToList());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var order = await _orderService.GetOrderAsync(id, cancellationToken);
        if (order is null)
        {
            return NotFound($"Order {id} was not found.");
        }

        return Ok(OrderMappings.ToResponse(order));
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreatedOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreatedOrderResponse>> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Items.Count == 0)
        {
            return BadRequest("Add at least one product to the order.");
        }

        var createdOrder = await _orderService.CreateOrderAsync(
            new CreateOrderModel(
                request.UserId,
                [.. request.Items.Select(item => new CreateOrderItemModel(item.ProductId, item.Quantity))]),
            cancellationToken);

        return Ok(new CreatedOrderResponse(
            createdOrder.OrderId,
            createdOrder.OrderNumber,
            createdOrder.OrderDate,
            createdOrder.ReceiptCode,
            createdOrder.StatusName));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> Update(int id, [FromBody] UpdateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var updatedOrder = await _orderService.UpdateOrderAsync(
            id,
            new UpdateOrderModel(request.DeliveryDate, request.StatusName),
            cancellationToken);

        if (updatedOrder is null)
        {
            return NotFound($"Order {id} was not found.");
        }

        return Ok(OrderMappings.ToResponse(updatedOrder));
    }
}

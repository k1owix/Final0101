using DataAccess;
using DataAccess.Entities;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDatabaseContext _context;

        public OrdersController(AppDatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить все заказы
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                    .ToListAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить заказ по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (order == null)
                    return NotFound($"Заказ #{id} не найден");

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Создать новый заказ
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Order order)
        {
            order.PickupCode = new Random().Next(100, 999).ToString();
            order.OrderDate = DateTime.Now;
            order.Status = "Новый";

            foreach (var item in order.OrderProducts ?? new List<OrderProduct>())
            {
                item.Order = null;
                item.Product = null;
                item.Id = 0;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new 
            { 
                order.OrderId, 
                order.OrderDate, 
                order.PickupCode, 
                order.Status 
            });
        }

        /// <summary>
        /// Обновить заказ (дата доставки, статус)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Order order)
        {
            try
            {
                var existing = await _context.Orders.FindAsync(id);
                if (existing == null)
                    return NotFound($"Заказ #{id} не найден");

                existing.DeliveryDate = order.DeliveryDate;
                existing.Status = order.Status;

                await _context.SaveChangesAsync();
                return Ok(existing);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Удалить заказ
        /// </summary>
        [HttpDelete("{orderId}/product/{productId}")]
        public async Task<IActionResult> RemoveProduct(int orderId, string productId)
        {
            var item = await _context.OrderProducts
                .FirstOrDefaultAsync(op => op.OrderId == orderId && op.ProductId == productId);

            if (item == null) return NotFound();

            _context.OrderProducts.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Товар удален" });
        }
    }
}
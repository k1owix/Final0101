using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess;

namespace WebAPI.ControllersMVC
{
    public class HomeController : Controller
    {
        private readonly AppDatabaseContext _context;

        public HomeController(AppDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        [HttpPost]
        public IActionResult AddToCart(string productId, string productName)
        {
            TempData["Message"] = $"✅ Товар \"{productName}\" добавлен в заказ!";
            return RedirectToAction("Index");
        }
    }
}
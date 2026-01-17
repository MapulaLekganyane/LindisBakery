// Controllers/AdminController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using LindisBakery.Models;
using LindisBakery.Data;

namespace LindisBakery.Controllers
{
    //[Authorize] // Add authentication later
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IOrderRepository orderRepository,
            ApplicationDbContext context,
            ILogger<AdminController> logger)
        {
            _orderRepository = orderRepository;
            _context = context;
            _logger = logger;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var stats = await _orderRepository.GetOrderStatisticsAsync();
            var recentOrders = await _orderRepository.GetRecentOrdersAsync(5);

            ViewBag.Statistics = stats;
            ViewBag.RecentOrders = recentOrders;

            return View();
        }

        [HttpGet("orders")]
        public async Task<IActionResult> Orders(string status = null)
        {
            IQueryable<Order> ordersQuery = _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.OrderDate);

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                ordersQuery = ordersQuery.Where(o => o.Status == status);
                ViewBag.SelectedStatus = status;
            }

            var orders = await ordersQuery.ToListAsync();

            // Get status counts for filter
            ViewBag.StatusCounts = new
            {
                All = await _context.Orders.CountAsync(),
                Pending = await _context.Orders.CountAsync(o => o.Status == "Pending"),
                Processing = await _context.Orders.CountAsync(o => o.Status == "Processing"),
                Completed = await _context.Orders.CountAsync(o => o.Status == "Completed"),
                Cancelled = await _context.Orders.CountAsync(o => o.Status == "Cancelled")
            };

            return View(orders);
        }

        [HttpGet("orders/{id}")]
        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet("orders/number/{orderNumber}")]
        public async Task<IActionResult> OrderDetailByNumber(string orderNumber)
        {
            var order = await _orderRepository.GetOrderByNumberAsync(orderNumber);

            if (order == null)
            {
                return NotFound();
            }

            return View("OrderDetail", order);
        }

        [HttpPost("orders/{id}/update-status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status, string notes = null)
        {
            try
            {
                await _orderRepository.UpdateOrderStatusAsync(id, status, notes);
                TempData["SuccessMessage"] = $"Order status updated to {status}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status");
                TempData["ErrorMessage"] = "Error updating order status";
            }

            return RedirectToAction("OrderDetail", new { id });
        }

        [HttpGet("products")]
        public async Task<IActionResult> Products()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        [HttpGet("analytics")]
        public async Task<IActionResult> Analytics()
        {
            var stats = await _orderRepository.GetOrderStatisticsAsync();

            // Get top selling products
            var topProducts = await _context.OrderItems
                .Include(oi => oi.Product)
                .GroupBy(oi => new { oi.ProductId, oi.ProductName })
                .Select(g => new
                {
                    ProductName = g.Key.ProductName,
                    TotalQuantity = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.TotalPrice)
                })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(10)
                .ToListAsync();

            ViewBag.Statistics = stats;
            ViewBag.TopProducts = topProducts;

            return View();
        }
    }
}
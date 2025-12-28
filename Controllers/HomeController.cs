using LindisBakery.Data;
using LindisBakery.Helpers;
using LindisBakery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace LindisBakery.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ShoppingCart _cart;
        private readonly EmailService _emailService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ApplicationDbContext context,
            ShoppingCart cart,
            EmailService emailService,
            ILogger<HomeController> logger)
        {
            _context = context;
            _cart = cart;
            _emailService = emailService;
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.CartCount = _cart.Items.Sum(i => i.Quantity);
        }
        // ===========================
        // HOME / MENU
        // ===========================
        public IActionResult Index()
        {
            var products = _context.Products.Where(p => p.IsAvailable).ToList();
            return View(products);
        }

        public IActionResult Menu(string? category = null)
        {
            var products = _context.Products.Where(p => p.IsAvailable).ToList();

            // Set categories for filter buttons
            var categories = _context.Products
                .Where(p => p.IsAvailable)
                .Select(p => p.Category)
                .Distinct()
                .ToList();

            ViewBag.Categories = categories;
            ViewBag.Category = category;

            // Filter by category if specified
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category).ToList();
            }

            return View(products);
        }

        // ===========================
        // ABOUT / CONTACTS
        // ===========================
        public IActionResult About() => View();
        public IActionResult Contacts() => View();

        // ===========================
        // CART METHODS
        // ===========================

        // Display full shopping cart page
        public IActionResult Cart()
        {
            // Pass your ShoppingCart model to the view
            return View(_cart);
        }

        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null) return NotFound();

            _cart.AddItem(product.Id, product.Name, product.Price, quantity, product.ImageUrl);
            return RedirectToAction("Menu");
        }


        public IActionResult RemoveFromCart(int productId)
        {
            _cart.RemoveItem(productId);
            return RedirectToAction("Cart");
        }

        public IActionResult ClearCart()
        {
            _cart.Clear();
            return RedirectToAction("Cart");
        }

        [HttpGet]
        public IActionResult GetCartSummary()
        {
            int totalItems = _cart.Items.Sum(i => i.Quantity);
            return Json(new { totalItems });
        }





        // ===========================
        // CHECKOUT
        // ===========================
        [HttpGet]
        public IActionResult Checkout()
        {
            if (!_cart.Items.Any())
                return RedirectToAction("Menu");

            return View(new Order());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            try
            {
                if (!_cart.Items.Any())
                    ModelState.AddModelError("", "Your cart is empty.");

                if (!ModelState.IsValid)
                    return View(order);

                // Calculate totals
                order.Subtotal = _cart.Items.Sum(i => i.Price * i.Quantity);
                order.Total = order.Subtotal + order.DeliveryFee;
                order.Status = "Pending";

                // Save Order
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Save Order Items
                foreach (var item in _cart.Items)
                {
                    _context.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        UnitPrice = item.Price,
                        Quantity = item.Quantity
                    });
                }

                await _context.SaveChangesAsync();

                // Send email confirmation
                await _emailService.SendOrderConfirmationAsync(
                    order.CustomerEmail,
                    order.CustomerName,
                    order.PaymentMethod,
                    order.Total
                );

                // Clear cart
                _cart.Clear();

                // Pass data to confirmation view
                ViewBag.SuccessMessage = "Your order has been placed successfully!";
                ViewBag.OrderNumber = order.OrderNumber;

                return View("OrderConfirmation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Checkout failed");
                ModelState.AddModelError("", "Something went wrong while processing your order.");
                return View(order);
            }
        }

        // ===========================
        // ORDER CONFIRMATION
        // ===========================
        public IActionResult OrderConfirmation() => View();

        // ===========================
        // ERROR
        // ===========================
        public IActionResult Error() => View();
    }
}

using LindisBakery.Data;
using LindisBakery.Helpers;
using LindisBakery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LindisBakery.Controllers
{
    public class HomeController : Controller
    {
        // Run Home page
        public IActionResult Index()
        {
            return View();
        }

        // RUn About Us 
        public IActionResult About()
        {
            return View();
        }

        // RUn Contacts Page
        public IActionResult Contacts()
        {
            return View();
        }

        // MENU Controller

        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Menu(string category = null)
        {
            IQueryable<Product> products = _context.Products.Where(p => p.IsAvailable);

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category);
                ViewBag.Category = category;
            }

            var categories = await _context.Products
                .Where(p => p.IsAvailable)
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();

            ViewBag.Categories = categories;

            return View(await products.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.IsAvailable);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        // CART Controller
        
        private const string CartSessionKey = "ShoppingCart";


        private ShoppingCart GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return cartJson == null ? new ShoppingCart() :
                JsonSerializer.Deserialize<ShoppingCart>(cartJson);
        }

        private void SaveCart(ShoppingCart cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }

        public IActionResult Cart()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId && p.IsAvailable);

            if (product == null)
            {
                return NotFound();
            }

            var cart = GetCart();
            cart.AddItem(product, quantity);
            SaveCart(cart);

            TempData["SuccessMessage"] = $"{product.Name} added to cart!";
            return RedirectToAction("Menu", "Home");
        }

        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            var cart = GetCart();
            cart.UpdateQuantity(productId, quantity);
            SaveCart(cart);

            return RedirectToAction("Menu");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            cart.RemoveItem(productId);
            SaveCart(cart);

            return RedirectToAction("Menu");
        }

        public IActionResult GetCartSummary()
        {
            var cart = GetCart();
            return Json(new
            {
                totalItems = cart.GetTotalItems(),
                totalPrice = CurrencyHelper.FormatAmount(cart.GetTotal())
            });
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            var cart = GetCart();
            cart.Clear();
            SaveCart(cart);

            return RedirectToAction("Menu");
        }

        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty!";
                return RedirectToAction("Menu");
            }

            // Here you would typically integrate with payment gateway
            // For now, we'll just show a confirmation
            return View(cart);
        }

        [HttpPost]
        public IActionResult ProcessOrder()
        {
            var cart = GetCart();
            if (!cart.Items.Any())
            {
                return RedirectToAction("Menu");
            }

            // Process order logic here
            // Save to database, send confirmation email, etc.

            TempData["OrderSuccess"] = "Thank you for your order! Order #" +
                Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            cart.Clear();
            SaveCart(cart);

            return RedirectToAction("OrderConfirmation");
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }
    }
}

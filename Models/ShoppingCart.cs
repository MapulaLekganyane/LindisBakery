using LindisBakery.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;


namespace LindisBakery.Models
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public class ShoppingCart
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionKey = "CartItems";

        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public ShoppingCart(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            LoadCart();
        }

        private void LoadCart()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartData = session.GetString(SessionKey);
            if (!string.IsNullOrEmpty(cartData))
            {
                Items = JsonConvert.DeserializeObject<List<CartItem>>(cartData);
            }
        }

        private void SaveCart()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString(SessionKey, JsonConvert.SerializeObject(Items));
        }

        public void AddItem(int productId, string productName, decimal price, int quantity = 1, string imageUrl = "")
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                Items.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = productName,
                    Price = price,
                    Quantity = quantity,
                    ImageUrl = imageUrl
                });
            }
            SaveCart();
        }

        public void RemoveItem(int productId)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                Items.Remove(item);
            }
            SaveCart();
        }

        public void Clear()
        {
            Items.Clear();
            SaveCart();
        }

        public decimal GetTotal()
        {
            return Items.Sum(i => i.TotalPrice);
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null) return;

            if (quantity <= 0)
            {
                Items.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }

            SaveCart();
        }
    }
}

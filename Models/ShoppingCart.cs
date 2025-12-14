using LindisBakery.Models;
using System.Collections.Generic;
using System.Linq;

namespace LindisBakery.Models
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public void AddItem(Product product, int quantity = 1)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == product.Id);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl
                });
            }
        }

        public void RemoveItem(int productId)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                if (quantity <= 0)
                    RemoveItem(productId);
                else
                    item.Quantity = quantity;
            }
        }

        public decimal GetTotal()
        {
            return Items.Sum(i => i.TotalPrice);
        }

        public int GetTotalItems()
        {
            return Items.Sum(i => i.Quantity);
        }

        public void Clear()
        {
            Items.Clear();
        }
    }
}
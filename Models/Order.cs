using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LindisBakery.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string OrderNumber { get; set; } = GenerateOrderNumber();

        [Required]
        public string CustomerName { get; set; }

        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; }

        [Required]
        public string CustomerPhone { get; set; }

        [Required]
        public string ShippingAddress { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        public decimal Subtotal { get; set; }
        public decimal DeliveryFee { get; set; } = 40.99m;
        public decimal Total { get; set; }

        public string Status { get; set; } = "Pending";
        public string Notes { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime? CompletedDate { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        }
    }
}

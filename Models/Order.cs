using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LindisBakery.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

        public decimal Subtotal { get; set; }
        public decimal DeliveryFee { get; set; } = 20;
        public decimal Total { get; set; }

        public string Status { get; set; } = "Pending";
        public string OrderNumber { get; set; } = Guid.NewGuid().ToString()[..8];
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Notes { get; set; } = string.Empty;

        public DateTime? CompletedDate { get; set; }

        // 🔴 THIS IS THE KEY FIX
        [ValidateNever]
        public List<OrderItem> Items { get; set; } = new();
    }
}
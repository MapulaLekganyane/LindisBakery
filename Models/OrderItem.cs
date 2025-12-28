using System.ComponentModel.DataAnnotations;

namespace LindisBakery.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }  // optional navigation property

        [Required]
        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }

        public decimal TotalPrice => UnitPrice * Quantity;
    }
}

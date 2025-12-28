// Data/OrderRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LindisBakery.Models;

namespace LindisBakery.Data
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> GetOrderByIdAsync(int id);
        Task<Order> GetOrderByNumberAsync(string orderNumber);
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetOrdersByStatusAsync(string status);
        Task<List<Order>> GetRecentOrdersAsync(int count = 10);
        Task UpdateOrderStatusAsync(int orderId, string status, string notes = null);
        Task<OrderStatistics> GetOrderStatisticsAsync();
    }

    public class OrderStatistics
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ProcessingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public List<MonthlyRevenue> MonthlyRevenue { get; set; } = new List<MonthlyRevenue>();
    }

    public class MonthlyRevenue
    {
        public string Month { get; set; }
        public decimal Revenue { get; set; }
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(ApplicationDbContext context, ILogger<OrderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                throw;
            }
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> GetOrderByNumberAsync(string orderNumber)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByStatusAsync(string status)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetRecentOrdersAsync(int count = 10)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status, string notes = null)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                order.Notes = notes;

                if (status == "Completed")
                    order.CompletedDate = DateTime.Now;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<OrderStatistics> GetOrderStatisticsAsync()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            var stats = new OrderStatistics
            {
                TotalOrders = await _context.Orders.CountAsync(),
                PendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending"),
                ProcessingOrders = await _context.Orders.CountAsync(o => o.Status == "Processing"),
                CompletedOrders = await _context.Orders.CountAsync(o => o.Status == "Completed"),
                TotalRevenue = await _context.Orders
                    .Where(o => o.Status == "Completed")
                    .SumAsync(o => o.Total),
                TodayRevenue = await _context.Orders
                    .Where(o => o.OrderDate.Date == today && o.Status == "Completed")
                    .SumAsync(o => o.Total)
            };

            // Get monthly revenue for the last 6 months
            for (int i = 5; i >= 0; i--)
            {
                var month = today.AddMonths(-i);
                var monthStart = new DateTime(month.Year, month.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var revenue = await _context.Orders
                    .Where(o => o.OrderDate >= monthStart && o.OrderDate <= monthEnd && o.Status == "Completed")
                    .SumAsync(o => o.Total);

                stats.MonthlyRevenue.Add(new MonthlyRevenue
                {
                    Month = month.ToString("MMM yyyy"),
                    Revenue = revenue
                });
            }

            return stats;
        }
    }
}
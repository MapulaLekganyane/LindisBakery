namespace LindisBakery.Models
{
    namespace LindisBakery.Models
    {
        public class OrderStatistics
        {
            public int TotalOrders { get; set; }
            public int PendingOrders { get; set; }
            public int ProcessingOrders { get; set; }
            public int CompletedOrders { get; set; }
            public int CancelledOrders { get; set; }
            public decimal TodayRevenue { get; set; }
            public decimal TotalRevenue { get; set; }
            public decimal AverageOrderValue { get; set; }
        }
    }
}

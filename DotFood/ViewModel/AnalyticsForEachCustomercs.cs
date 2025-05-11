using DotFood.Entity;

namespace DotFood.ViewModel
{
    public class AnalyticsForEachCustomers
    {
        public Users CustomerId { get; set; }
        public Users user { get; set; }

        public int TotalOrders { get; set; }

        public decimal TotalRevenue { get; set; }

        public string CustomerName { get; set; }
    }
}


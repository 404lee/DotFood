using DotFood.Entity;

namespace DotFood.ViewModel
{
    public class AnalyticsViewModelForEachVendor
    {
        public Users VendorId { get; set; }
        public Users user { get; set; }

        public int TotalOrders { get; set; }

        public decimal TotalRevenue { get; set; }

        public string VendorName { get; set; }

    }
}

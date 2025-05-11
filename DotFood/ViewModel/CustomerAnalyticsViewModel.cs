using System.ComponentModel.DataAnnotations;

namespace DotFood.ViewModel
{
    public class CustomerAnalyticsViewModel
    {

        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalRevenue { get; set; }

        public List<AnalyticsForEachCustomers> analyticsForEachCustomers { get; set; }
        public int TotalCustomers { get; set; }

        public int TotalOrders { get; set; }



    }
}

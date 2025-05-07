//this to view the vendor analytics data for the admin

using System.ComponentModel.DataAnnotations;

namespace DotFood.ViewModel
{

    public class VendorAnalyticsViewModel

        {
        // Display currency data field in the format $1,345.50.
        [DisplayFormat(DataFormatString = "{0:C}")] 
            public decimal TotalRevenue { get; set; }

            public int TotalVendors { get; set; }
            public int TotalOrders { get; set; }
    
    }
    }



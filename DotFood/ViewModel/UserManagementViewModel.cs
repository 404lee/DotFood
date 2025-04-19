using System.ComponentModel.DataAnnotations;

namespace DotFood.ViewModel
{
    public class UserManagementViewModel

        {
            [DisplayFormat(DataFormatString = "{0:C}")] 
            public decimal TotalRevenue { get; set; }

            public int TotalVendors { get; set; }
            public int TotalOrders { get; set; }
    }
    }



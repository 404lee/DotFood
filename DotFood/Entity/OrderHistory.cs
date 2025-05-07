
//this to take order history and view it for admin in view history 

namespace DotFood.Controllers
{
    internal class OrderHistory
    {
        public long OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string VendorName { get; internal set; }
    }
}
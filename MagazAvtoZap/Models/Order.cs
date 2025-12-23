using System;
using System.Collections.Generic;

namespace MagazAvtoZap.Models
{
    public class Order
    {
        public string OrderNumber { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public List<CartItem> Items { get; set; }
    }
}

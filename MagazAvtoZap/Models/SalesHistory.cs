using System;

namespace MagazAvtoZap.Models
{
    public class SalesHistory
    {
        public int SaleID { get; set; }
        public DateTime SaleDate { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

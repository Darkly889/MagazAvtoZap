using System;
using System.ComponentModel.DataAnnotations;

namespace MagazAvtoZap.Models
{
    public class Sale
    {
        public int SaleID { get; set; }

        [Required(ErrorMessage = "Дата продажи обязательна")]
        public DateTime SaleDate { get; set; } = DateTime.Now;

        [Range(1, int.MaxValue, ErrorMessage = "ID товара должен быть указан")]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Сумма должна быть больше 0")]
        public decimal TotalPrice { get; set; }
    }
}

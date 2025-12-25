using System.ComponentModel.DataAnnotations;

public class Product
{
    public int ProductID { get; set; }

    [Required(ErrorMessage = "Название товара обязательно для заполнения")]
    [StringLength(100, ErrorMessage = "Название товара не может быть длиннее 100 символов")]
    public string Name { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Категория должна быть выбрана")]
    public int CategoryID { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Название поставщика не может быть длиннее 100 символов")]
    public string? Supplier { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Количество на складе не может быть отрицательным")]
    public int StockQuantity { get; set; }

    public int AvailableQuantity { get; set; }

    public string? Description { get; set; }
}

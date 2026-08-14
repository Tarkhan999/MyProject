namespace FiorelloBackendPractice.ViewModels;

public class BasketItemVM
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }

    public string CategoryName { get; set; }
    public double ProductPrice { get; set; }
    public string ProductImage { get; set; }
    public int ProductCount { get; set; }
}
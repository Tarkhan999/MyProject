namespace FiorelloBackendPractice.ViewModels
{
    public class BasketItemVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public double ProductPrice { get; set; }
        public string ProductImage { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }
}
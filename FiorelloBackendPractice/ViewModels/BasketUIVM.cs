namespace FiorelloBackendPractice.ViewModels
{
    public class BasketUIVM
    {
        public List<BasketItemVM> Items { get; set; } = new List<BasketItemVM>();
        public double Total { get; set; }
    }
}
namespace FiorelloBackendPractice.ViewModels.Order;

public class OrderCreateVM
{
    public string Country { get; set; }
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
}
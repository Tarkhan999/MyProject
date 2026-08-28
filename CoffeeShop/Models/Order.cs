namespace FiorelloBackendPractice.Models;

public class Order:BaseEntity
{
    public string AppUserId { get; set; } 
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Pending"; 
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

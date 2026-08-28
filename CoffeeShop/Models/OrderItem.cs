namespace FiorelloBackendPractice.Models;

public class OrderItem:BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Count { get; set; }
    public decimal Price { get; set; }
}
namespace FiorelloBackendPractice.Models;

public class Subscriber:BaseEntity
{
    public string Email { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
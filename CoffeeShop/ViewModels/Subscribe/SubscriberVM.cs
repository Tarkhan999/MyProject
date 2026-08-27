namespace FiorelloBackendPractice.ViewModels.Subscribe;

public class SubscriberVM
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
namespace FiorelloBackendPractice.Models;

public class SubscribeInfo:BaseEntity
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string BackgroundImageUrl { get; set; } = null!;
}
namespace FiorelloBackendPractice.ViewModels.Instagram;

public class InstagramUpdateVM
{
    public int Id { get; set; }
    public string? ImageUrl { get; set; } 
    public IFormFile? Photo { get; set; }
}
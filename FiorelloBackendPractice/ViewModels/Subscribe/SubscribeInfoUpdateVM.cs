using System.ComponentModel.DataAnnotations;

namespace FiorelloBackendPractice.ViewModels.Subscribe;

public class SubscribeInfoUpdateVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Başlık zorunludur.")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Açıklama zorunludur.")]
    public string Description { get; set; } = null!;

    public string? BackgroundImageUrl { get; set; } 
    public IFormFile? Photo { get; set; }
}
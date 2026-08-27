using System.ComponentModel.DataAnnotations;

namespace FiorelloBackendPractice.ViewModels.Blog;

public class BlogCreateVM
{
    [Required(ErrorMessage = "Başlık zorunludur.")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Açıklama zorunludur.")]
    public string Description { get; set; } = null!;

    public string? Link { get; set; }

    [Required(ErrorMessage = "Görsel seçimi zorunludur.")]
    public IFormFile Photo { get; set; } = null!;
}
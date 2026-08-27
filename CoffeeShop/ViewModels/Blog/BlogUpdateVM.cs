using System.ComponentModel.DataAnnotations;

namespace FiorelloBackendPractice.ViewModels.Blog;

public class BlogUpdateVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Başlık zorunludur.")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Açıklama zorunludur.")]
    public string Description { get; set; } = null!;

    public string? Link { get; set; }
    public string? Image { get; set; }
    public IFormFile? Photo { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace FiorelloBackendPractice.ViewModels.About;

public class AboutEditVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Başlık zorunludur.")]
    public string Title { get; set; }
    
    public string HighlightedText { get; set; }
    
    [Required(ErrorMessage = "Açıklama zorunludur.")]
    public string Description { get; set; }
    
    public string PointText { get; set; }
    
    public string? ExistingImageUrl { get; set; }
    public IFormFile? Photo { get; set; }
}
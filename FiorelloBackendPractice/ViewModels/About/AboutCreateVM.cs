using System.ComponentModel.DataAnnotations;

namespace FiorelloBackendPractice.ViewModels.About;

public class AboutCreateVM
{
    [Required(ErrorMessage = "Başlık zorunludur.")]
    public string Title { get; set; }
    
    public string HighlightedText { get; set; }
    
    [Required(ErrorMessage = "Açıklama zorunludur.")]
    public string Description { get; set; }
    
    public string PointText { get; set; }
    
    [Required(ErrorMessage = "Lütfen bir resim seçin.")]
    public IFormFile Photo { get; set; }
}
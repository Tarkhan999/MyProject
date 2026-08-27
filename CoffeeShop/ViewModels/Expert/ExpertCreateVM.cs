using System.ComponentModel.DataAnnotations;

namespace FiorelloBackendPractice.ViewModels.Expert;

public class ExpertCreateVM
{
    [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
    public string FullName { get; set; }
    
    [Required(ErrorMessage = "Pozisyon alanı zorunludur.")]
    public string Position { get; set; }
    
    [Required(ErrorMessage = "Lütfen bir resim seçin.")]
    public IFormFile Photo { get; set; }
}
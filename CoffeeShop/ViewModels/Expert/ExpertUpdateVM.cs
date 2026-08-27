using System.ComponentModel.DataAnnotations;

namespace FiorelloBackendPractice.ViewModels.Expert;

public class ExpertUpdateVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
    public string FullName { get; set; }
    
    [Required(ErrorMessage = "Pozisyon alanı zorunludur.")]
    public string Position { get; set; }
    
    public string? ImageUrl { get; set; } 
    
    public IFormFile? Photo { get; set; }
}
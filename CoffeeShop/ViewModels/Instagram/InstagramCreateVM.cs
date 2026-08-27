using System.ComponentModel.DataAnnotations;

namespace FiorelloBackendPractice.ViewModels.Instagram;

public class InstagramCreateVM
{
    [Required(ErrorMessage = "Lütfen bir görsel seçin.")]
    public IFormFile Photo { get; set; } = null!;
}
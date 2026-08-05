using System.ComponentModel.DataAnnotations;

namespace FiorelloBackendPractice.ViewModels.Slider;

public class SliderCreateVM
{
    [Required]
    public IFormFile UploadImage { get; set; }
}
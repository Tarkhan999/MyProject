using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FiorelloBackendPractice.ViewModels.Slider
{
    public class SliderCreateVM
    {
        [Required] public List<IFormFile> UploadImages { get; set; }
    }
}
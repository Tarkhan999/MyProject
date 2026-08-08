using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FiorelloBackendPractice.ViewModels.Slider
{
    public class SliderCreateVM
    {
        [Required(ErrorMessage = "Lütfen bir resim seçin.")]
        public IFormFile UploadImage { get; set; } 
    }
}
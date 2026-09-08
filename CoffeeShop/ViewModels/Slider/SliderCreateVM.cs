using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FiorelloBackendPractice.ViewModels.Slider
{
    public class SliderCreateVM
    {
      
        public List<IFormFile> UploadFiles { get; set; }
    }
}
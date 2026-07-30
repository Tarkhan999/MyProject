using FiorelloBackendPractice.Models;

namespace FiorelloBackendPractice.Services.Interfaces;

public interface ISliderService
{
    Task<IEnumerable<Slider>> GetAllAsync();
    Task<SliderInfo> GetInfoAsync();
}
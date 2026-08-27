using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.ViewModels.Slider;

namespace FiorelloBackendPractice.Services.Interfaces;

public interface ISliderService
{
    Task<IEnumerable<Slider>> GetAllAsync();
    Task<SliderInfo> GetInfoAsync();
    Task EditAsync(Slider slider, SliderEditVM request);
    Task<Slider> GetByIdAsync(int id);
}
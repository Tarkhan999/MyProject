using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FiorelloBackendPractice.ViewComponents;

public class SliderViewComponent:ViewComponent
{
    private readonly ISliderService _sliderService;
    public SliderViewComponent(ISliderService sliderService)
    {
        _sliderService = sliderService;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        IEnumerable<Slider> Sliders = await _sliderService.GetAllAsync();
        SliderInfo sliderInfo = await _sliderService.GetInfoAsync();
        SliderVCVM response = new()
        {
            Sliders = Sliders,
            SliderInfo =  sliderInfo
        };
        return await Task.FromResult(View(response));
    }
}

public class SliderVCVM
{
    public IEnumerable<Slider> Sliders { get; set; }
    public SliderInfo SliderInfo { get; set; }
}
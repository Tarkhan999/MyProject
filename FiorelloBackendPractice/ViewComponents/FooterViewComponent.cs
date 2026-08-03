using FiorelloBackendPractice.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FiorelloBackendPractice.ViewComponents;

public class FooterViewComponent : ViewComponent
{
    private readonly ISettingService _settingService;

    public FooterViewComponent(ISettingService settingService)
    {
        _settingService = settingService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var setting = await _settingService.GetAsync();
        return await Task.FromResult(View(setting));
    }
}
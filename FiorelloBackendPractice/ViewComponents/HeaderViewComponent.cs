using FiorelloBackendPractice.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FiorelloBackendPractice.ViewComponents;

public class HeaderViewComponent : ViewComponent
{
    private readonly ISettingService _settingService;

    public HeaderViewComponent(ISettingService settingService)
    {
        _settingService = settingService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var setting = await _settingService.GetAsync();
        return await Task.FromResult(View(setting));
    }
}
using FiorelloBackendPractice.Services.Interfaces;
using FiorelloBackendPractice.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        List<BasketVM> basketDatas;
        if (Request.Cookies["basket"] != null)
        {
            basketDatas=JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
        }
        else
        {
            basketDatas=new List<BasketVM>();
        }

        int basketCount = basketDatas.Sum(b => b.ProductCount);
        double productPrice=basketDatas.Sum(p=>p.Price*p.ProductCount);
        var setting = await _settingService.GetAsync();
        return await Task.FromResult(View(new HeaderVM
        {
            Setting = setting,
            BasketCount = basketCount,
            ProductPrice = productPrice
            
        }));
    }
}
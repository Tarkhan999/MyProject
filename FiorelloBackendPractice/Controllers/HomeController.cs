using FiorelloBackendPractice.Services.Interfaces;
using FiorelloBackendPractice.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FiorelloBackendPractice.Controllers;

public class HomeController : Controller
{
    private readonly IBlogService _blogService;


    public HomeController(IBlogService blogService)

    {
        _blogService = blogService;
    }

    public async Task<IActionResult> Index()
    {
        var Blogs = await _blogService.GetAllAsync(3);


        HomeVM homeVM = new()
        {
            Blogs = Blogs
        };
        return View(homeVM);
    }
}
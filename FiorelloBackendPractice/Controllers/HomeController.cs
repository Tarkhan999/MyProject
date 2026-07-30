using System.Diagnostics;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.Services;
using FiorelloBackendPractice.Services.Interfaces;
using FiorelloBackendPractice.ViewModels;
using FiorelloBackendPractice.ViewModels.Blog;
using FiorelloBackendPractice.ViewModels.Category;
using FiorelloBackendPractice.Views.Product;
using Microsoft.AspNetCore.Mvc;


namespace FiorelloBackendPractice.Controllers;

public class HomeController : Controller
{
    private readonly ISliderService _sliderService;
    private readonly IBlogService _blogService;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public HomeController(ISliderService sliderService,
                          IBlogService blogService,
                          ICategoryService categoryService,
                          IProductService productService)
                         
    {
        _sliderService = sliderService;
        _blogService = blogService;
        _categoryService = categoryService;
        _productService = productService;
        
       
    }
   
    public async Task< IActionResult> Index()
    {
        IEnumerable<Slider> Sliders = await _sliderService.GetAllAsync();
        SliderInfo sliderInfo = await _sliderService.GetInfoAsync();
        IEnumerable<BlogUIVM> Blogs = await _blogService.GetAllAsync(3);
        IEnumerable<CategoryUIVM> Categories = await _categoryService.GetAllAsync();
        IEnumerable<ProductUIVM> Products = await _productService.GetAllAsync();
        
        HomeVM homeVM = new()
        { 
            Sliders = Sliders,
            SliderInfo = sliderInfo,
            Blogs = Blogs,
           Categories = Categories,
            Products = Products
        };
        return View(homeVM);
    }

   
}
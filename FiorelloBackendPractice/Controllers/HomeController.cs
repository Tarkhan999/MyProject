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
  
    private readonly IBlogService _blogService;
    

    public HomeController(IBlogService blogService)
                         
    {
       
        _blogService = blogService;
    }
   
    public async Task< IActionResult> Index()
    {
        
        IEnumerable<BlogUIVM> Blogs = await _blogService.GetAllAsync(3);
        
        
        HomeVM homeVM = new()
        { 
            
            Blogs = Blogs
           
        };
        return View(homeVM);
    }

   
}
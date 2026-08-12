using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Services.Interfaces;
using FiorelloBackendPractice.ViewModels;
using FiorelloBackendPractice.ViewModels.Blog;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FiorelloBackendPractice.Controllers;

public class HomeController : Controller
{
    private readonly IBlogService _blogService;
    
    private readonly AppDbContext _dbContext;

    public HomeController(IBlogService blogService, AppDbContext dbContext)
        
    {
        _blogService = blogService;
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        
        IEnumerable<BlogUIVM>blogs = await _blogService.GetAllAsync(3);


        HomeVM homeVM = new()
        {
            Blogs = blogs
        };
        return View(homeVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task< IActionResult> AddProducToBasket(int id)
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
        var product=await _dbContext.Products.FindAsync(id);
        
        var existProduct=basketDatas.FirstOrDefault(b=>b.ProductId==id);
        if (existProduct != null)
        {
            existProduct.ProductCount++;
        }
        else
        {
            basketDatas.Add(new BasketVM
            {
                ProductId = id,
                ProductCount = 1,
                Price = product.Price
            });
        }
        
        
        Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketDatas));
        return RedirectToAction(nameof(Index));
    }
}
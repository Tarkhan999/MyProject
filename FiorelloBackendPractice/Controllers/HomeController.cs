using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Services.Interfaces;
using FiorelloBackendPractice.ViewModels;
using FiorelloBackendPractice.ViewModels.About;
using FiorelloBackendPractice.ViewModels.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FiorelloBackendPractice.Controllers;

public class HomeController(IBlogService blogService, AppDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index()
    {
        IEnumerable<BlogUIVM> blogs = await blogService.GetAllAsync(3);
        var aboutData = await dbContext.Abouts.FirstOrDefaultAsync();

        HomeVM homeVm = new()
        {
            Blogs = blogs,
            AboutInfo = aboutData is null ? null : new AboutUIVM
            {
                Title = aboutData.Title,
                Description = aboutData.Description,
                PointText = aboutData.PointText,
                ImageUrl = aboutData.ImageUrl,
                HighlightedText = aboutData.HighlightedText
            }
        };

        return View(homeVm);
    }

    [HttpPost]
    public async Task<IActionResult> AddProductToBasket(int? id)
    {
        if (id is null) return BadRequest("Ürün ID'si bulunamadı.");

        var product = await dbContext.Products.FindAsync(id);
        if (product is null) return NotFound("Ürün bulunamadı.");

        string? cookie = Request.Cookies["basket"];
        
        List<BasketVM> basketList = string.IsNullOrEmpty(cookie) 
            ? [] 
            : JsonConvert.DeserializeObject<List<BasketVM>>(cookie) ?? [];

        var existProduct = basketList.FirstOrDefault(b => b.ProductId == id);
        if (existProduct is not null)
        {
            existProduct.ProductCount++;
        }
        else
        {
            basketList.Add(new BasketVM
            {
                ProductId = id.Value,
                Price = product.Price,
                ProductCount = 1
            });
        }

        Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketList));

        int count = basketList.Sum(b => b.ProductCount);
        double total = basketList.Sum(b => b.ProductCount * b.Price);

        return Ok(new { count, total });
    }
}
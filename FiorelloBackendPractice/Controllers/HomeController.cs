using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.Services.Interfaces;
using FiorelloBackendPractice.ViewModels;
using FiorelloBackendPractice.ViewModels.About;
using FiorelloBackendPractice.ViewModels.Blog;
using FiorelloBackendPractice.ViewModels.Expert;
using FiorelloBackendPractice.ViewModels.Subscribe;
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
        var subscribeData = await dbContext.SubscribeInfos.FirstOrDefaultAsync();
        var expertList = await dbContext.Experts.Select(e => new ExpertUIVM
        {
            FullName = e.FullName,
            Position = e.Position,
            ImageUrl = e.ImageUrl
        }).ToListAsync();

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
            },
            Experts = expertList,
            SubscribeInfo = subscribeData is null ? null : new SubscribeUIVM
            {
                Title = subscribeData.Title,
                Description = subscribeData.Description,
                BackgroundImageUrl = subscribeData.BackgroundImageUrl
            }
        };

        return View(homeVm);
    }

    [HttpPost]
    public async Task<IActionResult> CheckEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
        {
            return Json(new { success = false, message = "Lütfen geçerli bir e-posta adresi girin." });
        }

        // AspNetUsers (Identity) veritabanı tablosunda e-posta kontrolü
        bool isExist = await dbContext.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());

        if (isExist)
        {
            return Json(new { 
                success = true, 
                exists = true, 
                message = "Hesabınız bulundu! Giriş sayfasına yönlendiriliyorsunuz..." 
            });
        }

        return Json(new { 
            success = true, 
            exists = false, 
            message = "Hesabınız bulunamadı. Kayıt olma sayfasına yönlendiriliyorsunuz..." 
        });
    }

    [HttpPost]
    public async Task<IActionResult> Subscribe(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
        {
            return Json(new { success = false, message = "Lütfen geçerli bir e-posta adresi girin." });
        }

        bool isExist = await dbContext.Subscribers.AnyAsync(s => s.Email.ToLower() == email.ToLower());
        
        if (isExist)
        {
            return Json(new { success = false, message = "Bu e-posta adresi zaten kayıtlı!" });
        }

        Subscriber newSubscriber = new()
        {
            Email = email
        };

        await dbContext.Subscribers.AddAsync(newSubscriber);
        await dbContext.SaveChangesAsync();

        return Json(new { success = true, message = "Aramıza hoş geldin! Kahve kulübüne başarıyla katıldın ☕" });
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
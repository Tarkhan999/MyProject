using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.Services.Interfaces;
using FiorelloBackendPractice.ViewModels;
using FiorelloBackendPractice.ViewModels.About;
using FiorelloBackendPractice.ViewModels.Blog;
using FiorelloBackendPractice.ViewModels.Expert;
using FiorelloBackendPractice.ViewModels.Instagram;
using FiorelloBackendPractice.ViewModels.Subscribe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;

namespace FiorelloBackendPractice.Controllers;

public class HomeController(IBlogService blogService, AppDbContext dbContext,IWebHostEnvironment env) : Controller
{
    public async Task<IActionResult> Index()
    {
        IEnumerable<BlogUIVM> blogs = await blogService.GetAllAsync(3);
        var aboutData = await dbContext.Abouts.FirstOrDefaultAsync();
        var subscribeData = await dbContext.SubscribeInfos.FirstOrDefaultAsync();
        var instagramList = await dbContext.Instagrams.Select(i => new InstagramUIVM
        {
            ImageUrl = i.ImageUrl
        }).ToListAsync();
        var expertList = await dbContext.Experts.Select(e => new ExpertUIVM
        {Id=e.Id,
            FullName = e.FullName,
            Position = e.Position,
            ImageUrl = e.ImageUrl
        }).ToListAsync();

        HomeVM homeVm = new()
        {
            Blogs = blogs,
            AboutInfo = aboutData is null ? null : new AboutUIVM
            {
                Id = aboutData.Id,
                Title = aboutData.Title,
                Description = aboutData.Description,
                PointText = aboutData.PointText,
                ImageUrl = aboutData.ImageUrl,
                HighlightedText = aboutData.HighlightedText
            },
            InstagramPhotos = instagramList,
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

    [HttpGet("Home/StreamVideo{fileName}")]
    public IActionResult StreamVideo(string fileName)
    {
        if(string.IsNullOrEmpty(fileName)) return BadRequest("Video adı geçersiz.");
        var filePath=Path.Combine(env.WebRootPath,"img",fileName);
        if(!System.IO.File.Exists(filePath))
            return NotFound("Video bulunamadı.");
        return PhysicalFile(filePath, "video/mp4", enableRangeProcessing: true);
    }
    public async Task<IActionResult> ExpertDetail(int? id)
    {
        if(id is null)return BadRequest("ID gönderilmedi.");
        var expert=await dbContext.Experts.FirstOrDefaultAsync(e => e.Id == id);
        if (expert is null) return NotFound("Barista bulunamadı.");

        return View(expert);
    }
    public async Task<IActionResult> AboutDetail(int? id)
    {
        if(id is null)return BadRequest("ID gönderilmedi.");
        var aboutData=await dbContext.Abouts.FirstOrDefaultAsync(a => a.Id == id);
        if(aboutData is null)return NotFound("İlgili içerik bulunamadı.");
        return View(aboutData);
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
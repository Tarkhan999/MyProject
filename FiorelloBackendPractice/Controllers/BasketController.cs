using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FiorelloBackendPractice.Controllers;

public class BasketController : Controller
{
    private readonly AppDbContext _context;

    public BasketController(AppDbContext dbContext)
    {
        _context = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        List<BasketVM> basketDatas = new List<BasketVM>();
        if (Request.Cookies["basket"] != null)
        {
            basketDatas = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
        }

        List<BasketItemVM> basketItems = new List<BasketItemVM>();

        foreach (var item in basketDatas)
        {
            var dbProduct = await _context.Products
                .Include(m => m.Category)
                .Include(m => m.Images)
                .FirstOrDefaultAsync(m => m.Id == item.ProductId);
            
            if (dbProduct == null) continue;
            
            basketItems.Add(new BasketItemVM
            {
                ProductId = item.ProductId,
                ProductName = dbProduct.Name,
                ProductPrice = dbProduct.Price,
                ProductCount = item.ProductCount,
                ProductImage = dbProduct.Images.FirstOrDefault(m => m.IsMain)?.Image,
                CategoryName = dbProduct.Category.Name
            });
        }

        BasketUIVM response = new BasketUIVM
        {
            Items = basketItems,
            Total = basketItems.Sum(m => m.ProductPrice * m.ProductCount)
        };
        
        return View(response);
    }

    [HttpPost]
    public IActionResult RemoveFromBasket(int id)
    {
        if (Request.Cookies["basket"] != null)
        {
            List<BasketVM> basketDatas = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            var productToRemove = basketDatas.FirstOrDefault(b => b.ProductId == id);
            
            if (productToRemove != null)
            {
                basketDatas.Remove(productToRemove);
                Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketDatas));
            }
        }
        return Ok();
    }
    [HttpPost]
    public IActionResult IncreaseProductCount(int id)
    {
        if (Request.Cookies["basket"] != null)
        {
            List<BasketVM> basketDatas = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            var existProduct = basketDatas.FirstOrDefault(b => b.ProductId == id);
        
            if (existProduct != null)
            {
                existProduct.ProductCount++;
                Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketDatas));
                return Ok();
            }
        }
        return BadRequest("Ürün bulunamadı");
    }

    [HttpPost]
    public IActionResult DecreaseProductCount(int id)
    {
        if (Request.Cookies["basket"] != null)
        {
            List<BasketVM> basketDatas = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            var existProduct = basketDatas.FirstOrDefault(b => b.ProductId == id);
        
            if (existProduct != null)
            {
                if (existProduct.ProductCount > 1)
                {
                    existProduct.ProductCount--; // Sayı 1'den büyükse sadece azalt
                }
                else
                {
                    basketDatas.Remove(existProduct); // Sayı 1 ise ve eksiye basıldıysa ürünü sepetten tamamen sil
                }
            
                Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketDatas));
                return Ok();
            }
        }
        return BadRequest("Ürün bulunamadı");
    }
}
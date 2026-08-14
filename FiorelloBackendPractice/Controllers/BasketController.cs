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
        List<BasketVM> basketDatas;
        if (Request.Cookies["basket"] != null)
        {
            basketDatas=JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
        }
        else
        {
            basketDatas=new List<BasketVM>();
        }

        List<BasketItemVM> basketItems =[];

        foreach (var item in basketDatas)
        {
            var dbProduct=await _context.Products.Include(m=>m.Category).Include(m=>m.Images)
                .FirstOrDefaultAsync(m=>m.Id==item.ProductId);
            if (dbProduct == null)
            {
                return NotFound();
            }
            basketItems.Add(new BasketItemVM
            {
                ProductId = item.ProductId,
                ProductName = dbProduct.Name,
                ProductPrice = dbProduct.Price,
                ProductCount = item.ProductCount,
                ProductImage = dbProduct.Images.FirstOrDefault(m=>m.IsMain).Image,
                CategoryName =  dbProduct.Category.Name
            });
        }

        BasketUIVM response = new()
        {
            Items = basketItems,
            Total=basketItems.Sum(m=>m.ProductPrice*m.ProductCount)
        };
        return View(response);
    }
}
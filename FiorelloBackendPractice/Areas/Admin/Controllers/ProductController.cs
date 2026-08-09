using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Areas.Admin.Controllers;

[Area("Admin")] 
public class ProductController : Controller
{
    private readonly AppDbContext _dbContext;
    public ProductController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var products = await _dbContext.Products.Include(m=>m.Category).Include(m=>m.Images).ToListAsync();
        var model=products.Select(p=>new ProductVM
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Description = p.Description,
            CategoryName =  p.Category.Name,
            MainImage = p.Images.FirstOrDefault(m=>m.IsMain).Image
        }).ToList();
        return View(model);
    }
}
using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.ViewModels.Product;
using FiorelloBackendPractice.Views.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Areas.Admin.Controllers;

[Area("Admin")] 

public class ProductController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly IWebHostEnvironment _env;
    public ProductController(AppDbContext dbContext,
                             IWebHostEnvironment env)
    {
        _dbContext = dbContext;
        _env = env;
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

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var categories = await _dbContext.Categories.ToListAsync();
        ViewBag.Categories = categories.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateVM request)
    {
        if (!ModelState.IsValid)
        {
            var categories = await _dbContext.Categories.ToListAsync();
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            return View(request);
        }

        foreach (var item in request.Images)
        {
            if (!item.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("UploadImages","File must be an image");
                return View();
            }
        }

        List<ProductImage> productImages = new();

        foreach (var item in request.Images)
        {
            string fileName=Guid.NewGuid().ToString()+"-"+item.FileName;
            string path = Path.Combine(_env.WebRootPath, "img", fileName);
            using FileStream stream = new(path, FileMode.Create);
            await item.CopyToAsync(stream);
            productImages.Add(new ProductImage
            {
                Image = fileName,
            });
        }
productImages.FirstOrDefault().IsMain = true;
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            Description = request.Description,
            CategoryId = request.CategoryId,
            Images = productImages
        };
        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var product = await _dbContext.Products.Include(m => m.Category)
            .Include(m => m.Images)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product == null) return NotFound();
        var model = new ProductDetailVM
        {
            Name = product.Name,
            CategoryName = product.Category.Name,
            Description = product.Description,
            Price = product.Price,
            Images = product.Images.Select(i => new ProductImageVM
            {
                Image = i.Image,
                IsMain = i.IsMain
            }).ToList()
        };
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _dbContext.Products.Include(m => m.Category)
            .Include(m => m.Images)
            .FirstOrDefaultAsync(m => m.Id == id);
        if(product==null)return NotFound();
        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


  [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || id == 0) return NotFound();

        // Ürünü resimleriyle beraber getiriyoruz
        var product = await _dbContext.Products
            .Include(m => m.Images)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (product == null) return NotFound();

        var categories=await _dbContext.Categories.ToListAsync();
        ViewBag.Caegories=categories.Select(c=>new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();
        var model = new ProductEditVM
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            CategoryId = product.CategoryId,
            Image = product.Images.FirstOrDefault(m => m.IsMain)?.Image

        };
        return View(model);
    }

   [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, ProductEditVM request)
{
    if (id != request.Id) return BadRequest();

 
    if (!ModelState.IsValid)
    {
        var categories = await _dbContext.Categories.ToListAsync();
        ViewBag.Categories = categories.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();
        return View(request);
    }

    var product = await _dbContext.Products
        .Include(m => m.Images)
        .FirstOrDefaultAsync(m => m.Id == id);

    if (product == null) return NotFound();

  
    product.Name = request.Name;
    product.Price = request.Price;
    product.Description = request.Description;
    product.CategoryId = request.CategoryId;


    if (request.NewImage != null)
    {
        if (!request.NewImage.ContentType.Contains("image/"))
        {
            ModelState.AddModelError("NewImage", "Lütfen geçerli bir resim dosyası seçin.");
            var categoriesForError = await _dbContext.Categories.ToListAsync();
            ViewBag.Categories = categoriesForError.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
            request.Image = product.Images.FirstOrDefault(m => m.IsMain)?.Image;
            return View(request);
        }

        var oldMainImage = product.Images.FirstOrDefault(m => m.IsMain);
        if (oldMainImage != null)
        {
            string oldFilePath = Path.Combine(_env.WebRootPath, "img", oldMainImage.Image);
            if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
            product.Images.Remove(oldMainImage); 
        }

        string fileName = Guid.NewGuid().ToString() + "-" + request.NewImage.FileName;
        string newFilePath = Path.Combine(_env.WebRootPath, "img", fileName);
        
        using (FileStream stream = new(newFilePath, FileMode.Create))
        {
            await request.NewImage.CopyToAsync(stream);
        }

        product.Images.Add(new ProductImage
        {
            Image = fileName,
            IsMain = true
        });
    }

    
    await _dbContext.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
}
}
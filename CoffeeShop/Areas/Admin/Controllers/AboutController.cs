using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.ViewModels.About;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Areas.Admin.Controllers;




[Area("Admin")]
public class AboutController : Controller
{

    private readonly AppDbContext _dbContext;
    private readonly IWebHostEnvironment _env;

    public AboutController(AppDbContext dbContext, IWebHostEnvironment env)
    {
        _dbContext = dbContext;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var abouts = await _dbContext.Abouts.ToListAsync();
        return View(abouts);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AboutCreateVM request)
    {
        if (!ModelState.IsValid) return View(request);
        if (!request.Photo.ContentType.Contains("image/"))
        {
            ModelState.AddModelError("Photo", "Lütfen geçerli bir resim formatı seçin.");
            return View(request);
        }

        string fileName = Guid.NewGuid().ToString() + "_" + request.Photo.FileName;
        string path = Path.Combine(_env.WebRootPath, "img", fileName);
        await using (FileStream fileStream = new FileStream(path, FileMode.Create))
        {
            await request.Photo.CopyToAsync(fileStream);
        }

        About about = new About
        {
            Title = request.Title,
            HighlightedText = request.HighlightedText,
            Description = request.Description,
            PointText = request.PointText,
            ImageUrl = fileName
        };
        await _dbContext.Abouts.AddAsync(about);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var about = await _dbContext.Abouts.FirstOrDefaultAsync(m => m.Id == id);
        if (about == null) return NotFound();
        AboutEditVM vm = new AboutEditVM
        {
            Id = about.Id,
            Title = about.Title,
            HighlightedText = about.HighlightedText,
            Description = about.Description,
            PointText = about.PointText,
            ExistingImageUrl = about.ImageUrl
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AboutEditVM request)
    {
        if (!ModelState.IsValid) return View(request);

        var about = await _dbContext.Abouts.FirstOrDefaultAsync(m => m.Id == id);
        if (about == null) return NotFound();


        if (request.Photo != null)
        {
            if (!request.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "Lütfen geçerli bir resim formatı seçin.");
                return View(request);
            }


            string oldPath = Path.Combine(_env.WebRootPath, "img", about.ImageUrl);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }


            string fileName = Guid.NewGuid().ToString() + "-" + request.Photo.FileName;
            string newPath = Path.Combine(_env.WebRootPath, "img", fileName);


            await using (FileStream fileStream = new FileStream(newPath, FileMode.Create))
            {
                await request.Photo.CopyToAsync(fileStream);
            }


            about.ImageUrl = fileName;
        }


        about.Title = request.Title;
        about.HighlightedText = request.HighlightedText;
        about.Description = request.Description;
        about.PointText = request.PointText;


        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var about = await _dbContext.Abouts.FindAsync(id);
        if (about == null) return NotFound();

        string path = Path.Combine(_env.WebRootPath, "img", about.ImageUrl);
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }

        _dbContext.Abouts.Remove(about);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
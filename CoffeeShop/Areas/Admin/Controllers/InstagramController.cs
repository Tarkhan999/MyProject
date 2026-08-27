using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.ViewModels.Instagram;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Areas.Admin.Controllers;

[Area("Admin")]
public class InstagramController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public InstagramController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // Listeleme
    public async Task<IActionResult> Index()
    {
        List<InstagramVM> instagramPhotos = await _context.Instagrams
            .OrderByDescending(i => i.Id)
            .Select(i => new InstagramVM
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl
            }).ToListAsync();

        return View(instagramPhotos);
    }

    // Ekleme (GET)
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // Ekleme (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InstagramCreateVM request)
    {
        if (!ModelState.IsValid) return View(request);

        if (!request.Photo.ContentType.Contains("image/"))
        {
            ModelState.AddModelError("Photo", "Lütfen sadece resim formatında bir dosya yükleyin.");
            return View(request);
        }

        string fileName = Guid.NewGuid().ToString() + "-" + request.Photo.FileName;
        string path = Path.Combine(_env.WebRootPath, "img", fileName);

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            await request.Photo.CopyToAsync(stream);
        }

        Instagram instagram = new Instagram
        {
            ImageUrl = fileName
        };

        await _context.Instagrams.AddAsync(instagram);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Güncelleme (GET)
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        Instagram? instagram = await _context.Instagrams.FindAsync(id);
        if (instagram == null) return NotFound();

        InstagramUpdateVM model = new InstagramUpdateVM
        {
            Id = instagram.Id,
            ImageUrl = instagram.ImageUrl
        };

        return View(model);
    }

    // Güncelleme (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, InstagramUpdateVM request)
    {
        if (!ModelState.IsValid) return View(request);

        Instagram? instagram = await _context.Instagrams.FindAsync(id);
        if (instagram == null) return NotFound();

        // Eğer yeni bir fotoğraf seçildiyse
        if (request.Photo != null)
        {
            if (!request.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "Lütfen sadece resim formatında bir dosya seçin.");
                return View(request);
            }

            // Eski fotoğrafı sunucudan sil
            string oldPath = Path.Combine(_env.WebRootPath, "img", instagram.ImageUrl);
            if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);

            // Yeni fotoğrafı yükle
            string fileName = Guid.NewGuid().ToString() + "-" + request.Photo.FileName;
            string newPath = Path.Combine(_env.WebRootPath, "img", fileName);

            using (FileStream stream = new FileStream(newPath, FileMode.Create))
            {
                await request.Photo.CopyToAsync(stream);
            }

            instagram.ImageUrl = fileName;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // Silme (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        Instagram? instagram = await _context.Instagrams.FindAsync(id);
        if (instagram == null) return NotFound();

        // Fotoğrafı sunucudan sil
        string path = Path.Combine(_env.WebRootPath, "img", instagram.ImageUrl);
        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);

        _context.Instagrams.Remove(instagram);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
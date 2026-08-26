using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.ViewModels.Subscribe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Areas.Admin.Controllers;
[Area("Admin")]
public class SubscribeInfoController : Controller
{
    private readonly AppDbContext _context;
     private readonly IWebHostEnvironment _env;

     public SubscribeInfoController(AppDbContext context, IWebHostEnvironment env)
    {
         _context = context;
         _env = env;
     }
    public async Task< IActionResult> Index()
    {
        List<SubscribeInfoVM> info = await _context.SubscribeInfos.Select(s => new SubscribeInfoVM
        {
            Id = s.Id,
            Title = s.Title,
            Description = s.Description,
            BackgroundImageUrl = s.BackgroundImageUrl,
        }).ToListAsync();
        return View(info);
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        SubscribeInfo? info = await _context.SubscribeInfos.FindAsync(id);
        if (info == null) return NotFound();
        SubscribeInfoUpdateVM model = new SubscribeInfoUpdateVM
        {
            Id = info.Id,
            Title = info.Title,
            Description = info.Description,
            BackgroundImageUrl = info.BackgroundImageUrl
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, SubscribeInfoUpdateVM request)
    {
        if(!ModelState.IsValid)return View(request);
        SubscribeInfo? info = await _context.SubscribeInfos.FindAsync(id);
        if (info == null) return NotFound();
        if (request.Photo != null)
        {
            if (!request.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "Lütfen geçerli bir resim formatı seçin.");
                return View(request);
            }

            string oldPath = Path.Combine(_env.WebRootPath, "img", info.BackgroundImageUrl);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }

            string fileName = Guid.NewGuid().ToString() + "-" + request.Photo.FileName;
            string newPath=Path.Combine(_env.WebRootPath,"img",fileName);
            using (FileStream stream = new FileStream(newPath, FileMode.Create))
            {
                await request.Photo.CopyToAsync(stream);
            }

            info.BackgroundImageUrl = fileName;
        }
        info.Title=request.Title;
        info.Description = request.Description;
        await  _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
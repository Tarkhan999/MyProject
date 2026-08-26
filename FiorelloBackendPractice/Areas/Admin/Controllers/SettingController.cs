using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.ViewModels.Setting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Areas.Admin.Controllers;

[Area("Admin")]
public class SettingController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public SettingController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // Ayarları Görüntüleme
    public async Task<IActionResult> Index()
    {
        Setting? setting = await _context.Settings.FirstOrDefaultAsync();

        if (setting == null) return View(null);

        SettingVM model = new SettingVM
        {
            Id = setting.Id,
            HeaderLogo = setting.HeaderLogo,
            Phone = setting.Phone,
            Email = setting.Email
        };

        return View(model);
    }

    // Güncelleme (GET)
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        Setting? setting = await _context.Settings.FindAsync(id);
        if (setting == null) return NotFound();

        SettingUpdateVM model = new SettingUpdateVM
        {
            Id = setting.Id,
            ExistingHeaderLogo = setting.HeaderLogo,
            Phone = setting.Phone,
            Email = setting.Email
        };

        return View(model);
    }

    // Güncelleme (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, SettingUpdateVM request)
    {
        if (!ModelState.IsValid) return View(request);

        Setting? setting = await _context.Settings.FindAsync(id);
        if (setting == null) return NotFound();

        // Logo güncellenmek istendiyse
        if (request.HeaderLogoFile != null)
        {
            if (!request.HeaderLogoFile.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("HeaderLogoFile", "Lütfen sadece resim formatında dosya yükleyin.");
                return View(request);
            }

            // Eski logoyu sunucudan sil
            if (!string.IsNullOrEmpty(setting.HeaderLogo))
            {
                string oldPath = Path.Combine(_env.WebRootPath, "img", setting.HeaderLogo);
                if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
            }

            // Yeni logoyu kaydet
            string fileName = Guid.NewGuid().ToString() + "-" + request.HeaderLogoFile.FileName;
            string newPath = Path.Combine(_env.WebRootPath, "img", fileName);

            using (FileStream stream = new FileStream(newPath, FileMode.Create))
            {
                await request.HeaderLogoFile.CopyToAsync(stream);
            }

            setting.HeaderLogo = fileName;
        }

        setting.Phone = request.Phone;
        setting.Email = request.Email;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
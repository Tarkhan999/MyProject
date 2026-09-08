using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.Services.Interfaces;
using FiorelloBackendPractice.ViewModels.Slider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Areas.Admin.Controllers;

[Area("Admin")]
public class SliderController : Controller
{
    private readonly ISliderService _sliderService;
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public SliderController(AppDbContext context, IWebHostEnvironment env, ISliderService sliderService)
    {
        _context = context;
        _env = env;
        _sliderService = sliderService;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<SliderVM> sliders = await _context.Sliders.Select(s => new SliderVM
        {
            Id = s.Id,
            Image = s.Image,
            Video = s.Video
        }).ToListAsync();
        return View(sliders);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var slider = await _context.Sliders.FindAsync(id);
        if (slider == null) return NotFound();

        var result = new SliderVM
        {
            Id = slider.Id,
            Image = slider.Image,
            Video = slider.Video
        };
        return View(result);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SliderCreateVM request)
    {
        if (!ModelState.IsValid) return View(request);

        if (request.UploadFiles == null) 
        {
            ModelState.AddModelError("UploadFiles", "Lütfen en az bir dosya seçin.");
            return View(request);
        }

        foreach (var item in request.UploadFiles)
        {
            if (!item.ContentType.Contains("image/") && !item.ContentType.Contains("video/"))
            {
                ModelState.AddModelError("UploadFiles", "Dosyalar sadece resim veya video formatında olabilir.");
                return View(request);
            }
        }

        foreach (var item in request.UploadFiles)
        {
            bool isVideo = item.ContentType.Contains("video/");
            string fileName = Guid.NewGuid().ToString() + "-" + item.FileName;
            string path = Path.Combine(_env.WebRootPath, "img", fileName);

            using FileStream fileStream = new(path, FileMode.Create);
            await item.CopyToAsync(fileStream);

            var slider = new Slider();
            if (isVideo)
            {
                slider.Video = fileName;
            }
            else
            {
                slider.Image = fileName;
            }

            await _context.Sliders.AddAsync(slider);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest(new { message = "Geçersiz işlem: ID bulunamadı." });

        var slider = await _context.Sliders.FindAsync(id);
        if (slider == null) return NotFound(new { message = "Hata: Silinmek istenen Slider bulunamadı." });

        // Eski Resim varsa sil
        if (!string.IsNullOrEmpty(slider.Image))
        {
            string imgPath = Path.Combine(_env.WebRootPath, "img", slider.Image);
            if (System.IO.File.Exists(imgPath)) System.IO.File.Delete(imgPath);
        }

        // Eski Video varsa sil
        if (!string.IsNullOrEmpty(slider.Video))
        {
            string vidPath = Path.Combine(_env.WebRootPath, "img", slider.Video);
            if (System.IO.File.Exists(vidPath)) System.IO.File.Delete(vidPath);
        }

        _context.Sliders.Remove(slider);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Medya başarıyla silindi." });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || id == 0) 
            return Content("HATA: Hangi medyanın düzenleneceği anlaşılamadı (ID gelmedi)!");
            
        var slider = await _context.Sliders.FindAsync(id);
        
        if (slider == null) 
            return Content("HATA: Bu medya veritabanında bulunamadı! Muhtemelen daha önce sildiniz.");
            
        return View(new SliderEditVM
        {
            Image = slider.Image,
            Video = slider.Video
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [FromForm] SliderEditVM request)
    {
        if (!ModelState.IsValid) return View(request);
        
        var slider = await _sliderService.GetByIdAsync(id);
        if (slider == null) return Content("HATA: Güncellenmek istenen veri veritabanında bulunamadı!");

        if (request.NewFile != null)
        {
            bool isVideo = request.NewFile.ContentType.Contains("video/");

            if (!request.NewFile.ContentType.Contains("image/") && !isVideo)
            {
                ModelState.AddModelError("NewFile", "Lütfen geçerli bir resim veya video seçin.");
                return View(request);
            }

            string webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            
            // Eski dosyaları temizle
            if (!string.IsNullOrEmpty(slider.Image))
            {
                string oldImgPath = Path.Combine(webRootPath, "img", slider.Image);
                if (System.IO.File.Exists(oldImgPath)) System.IO.File.Delete(oldImgPath);
            }
            if (!string.IsNullOrEmpty(slider.Video))
            {
                string oldVidPath = Path.Combine(webRootPath, "img", slider.Video);
                if (System.IO.File.Exists(oldVidPath)) System.IO.File.Delete(oldVidPath);
            }

            // Yeni dosyayı kaydet
            string fileName = Guid.NewGuid().ToString() + "-" + request.NewFile.FileName;
            string newFilePath = Path.Combine(webRootPath, "img", fileName);
            
            using FileStream stream = new(newFilePath, FileMode.Create);
            await request.NewFile.CopyToAsync(stream);

            if (isVideo)
            {
                request.Video = fileName;
                request.Image = null;
            }
            else
            {
                request.Image = fileName;
                request.Video = null;
            }
        }
        else
        {
            request.Image = slider.Image;
            request.Video = slider.Video;
        }

        await _sliderService.EditAsync(slider, request);
        return RedirectToAction(nameof(Index));
    }
}
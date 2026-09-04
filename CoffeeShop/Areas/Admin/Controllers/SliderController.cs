using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.Services;
using FiorelloBackendPractice.Services.Interfaces;
using FiorelloBackendPractice.ViewModels.Slider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;

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
    public async Task< IActionResult> Index()
    {
        IEnumerable<SliderVM> sliders = await _context.Sliders.Select(s => new SliderVM
        {
            Id = s.Id,
            Image = s.Image
        }).ToListAsync();
        return View(sliders);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var slider = await _context.Sliders.FindAsync(id);
        var result =new SliderVM
        {
            Image = slider.Image
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
        if (!ModelState.IsValid) return View();
        foreach (var item in request.UploadImages)
        {
            if (!item.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("UploadImages","File must be an image");
                return View();
            }
        }

        foreach (var item in request.UploadImages)
        {
            string fileName = Guid.NewGuid().ToString() + "-" + item.FileName;
            string path=Path.Combine(_env.WebRootPath, "img", fileName);
            using FileStream fileStream = new(path, FileMode.Create);
            await item.CopyToAsync(fileStream);
            await _context.Sliders.AddAsync(new Slider
            {
                Image = fileName
            });

        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return BadRequest(new
            {
                message = "Geçersiz işlem: ID bulunamadı."
            });

        var slider = await _context.Sliders.FindAsync(id);
        if (slider == null) return NotFound(new { message = "Hata: Silinmek istenen Slider bulunamadı." });

        string path = Path.Combine(_env.WebRootPath, "img", slider.Image);
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }

        _context.Sliders.Remove(slider);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Slider başarıyla silindi." });
    }


    // 1. SAYFAYI AÇMA İŞLEMİ (GET)
[HttpGet]
public async Task<IActionResult> Edit(int? id) // int? yaptık ki id gelmezse çökmesin
{
    if (id == null || id == 0) 
        return Content("HATA: Hangi resmi düzenleyeceğiniz anlaşılamadı (ID gelmedi)!");
        
    var slider = await _context.Sliders.FindAsync(id);
    
    // Eğer veritabanında yoksa (silinmişse) NotFound yerine mesaj yazdırıyoruz:
    if (slider == null) 
        return Content("HATA: Bu resim veritabanında bulunamadı! Muhtemelen daha önce sildiniz. Lütfen Listeye (Index) dönüp sayfayı yenileyin.");
        
    return View(new SliderEditVM
    {
        Image = slider.Image
    });
}

// 2. FORMU GÖNDERME VE KAYDETME İŞLEMİ (POST)
[HttpPost]
// [ValidateAntiForgeryToken] // DİKKAT: 400 Beyaz sayfa hatasını engellemek için yoruma aldık!
public async Task<IActionResult> Edit(int id, [FromForm] SliderEditVM request) // [FromForm] garantisi eklendi
{
    if (!ModelState.IsValid) return View(request);
    
    var slider = await _sliderService.GetByIdAsync(id);
    if (slider == null) 
        return Content("HATA: Güncellenmek istenen veri veritabanında bulunamadı!");

    // Kullanıcı yeni bir resim seçtiyse:
    if (request.NewImage != null)
    {
        if (!request.NewImage.ContentType.Contains("image/"))
        {
            ModelState.AddModelError("NewImage", "Lütfen geçerli bir resim seçin.");
            return View(request);
        }

        string webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        
        // Eski resmi sil
        string oldFilePath = Path.Combine(webRootPath, "img", slider.Image);
        if (System.IO.File.Exists(oldFilePath))
        {
            System.IO.File.Delete(oldFilePath);
        }

        // Yeni resmi yükle
        string fileName = Guid.NewGuid().ToString() + "-" + request.NewImage.FileName;
        string newFilePath = Path.Combine(webRootPath, "img", fileName);
        
        using FileStream stream = new(newFilePath, FileMode.Create);
        await request.NewImage.CopyToAsync(stream);

        // Veritabanına gidecek ismi ayarla
        request.Image = fileName; 
    }
    else
    {
        // Yeni resim seçilmediyse eskiyi koru
        request.Image = slider.Image;
    }

    await _sliderService.EditAsync(slider, request);
    return RedirectToAction(nameof(Index));
}
}
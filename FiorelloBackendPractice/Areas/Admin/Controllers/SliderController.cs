using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.ViewModels.Slider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Areas.Admin.Controllers;
[Area("Admin")]
public class SliderController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public SliderController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
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
    public async Task<IActionResult> Delete(int id)
    {
        var slider = await _context.Sliders.FindAsync(id);
        if (slider == null) return NotFound();
        string path = Path.Combine(_env.WebRootPath, "img", slider.Image);
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }

        _context.Sliders.Remove(slider);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
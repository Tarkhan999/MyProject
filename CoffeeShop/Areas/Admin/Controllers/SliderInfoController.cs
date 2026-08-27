using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Areas.Admin.Controllers;

[Area("Admin")] 
public class SliderInfoController : Controller
{
    private readonly AppDbContext _context;
    private readonly ISliderService _sliderService;

    public SliderInfoController(AppDbContext context, ISliderService sliderService)
    {
        _context = context;
        _sliderService = sliderService;
    }


    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync();
        if (sliderInfo == null) return NotFound("Düzenlenecek kayıt bulunamadı.");

 
        return RedirectToAction("Edit", new { id = sliderInfo.Id }); 
    }


    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var sliderInfo = await _context.SliderInfos.FindAsync(id);
        if (sliderInfo == null) return NotFound();
        
        return View(sliderInfo);
    }

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SliderInfo sliderInfo, IFormFile file)
    {
        var existingData = await _context.SliderInfos.FindAsync(id);
        if (existingData == null) return NotFound();

        
        existingData.Title = sliderInfo.Title;
        existingData.Description = sliderInfo.Description;

    
        if (file != null && file.Length > 0)
        {
            var extension = Path.GetExtension(file.FileName);
            var newImage = Guid.NewGuid() + extension;
            var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/", newImage);
            
            using (var stream = new FileStream(location, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            existingData.SignImage = newImage;
        }

       
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}
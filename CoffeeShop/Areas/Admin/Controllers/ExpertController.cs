using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.ViewModels.Expert;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Areas.Admin.Controllers;
[Area("Admin")]
public class ExpertController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly IWebHostEnvironment _env;

    public ExpertController(AppDbContext dbContext,
                            IWebHostEnvironment env)
    {
        _dbContext = dbContext;
        _env = env;
    }
    public async Task < IActionResult> Index()
    {
        List<ExpertVM> experts = await _dbContext.Experts.Select(e => new ExpertVM
        {
            Id = e.Id,
            FullName = e.FullName,
            Position = e.Position,
            ImageUrl = e.ImageUrl
        }).ToListAsync();
        return View(experts);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExpertCreateVM request)
    {
        if(!ModelState.IsValid)return View(request);
        if (!request.Photo.ContentType.Contains("image/"))
        {
            ModelState.AddModelError("Photo", "Lütfen geçerli bir resim formatı seçin.");
            return View(request);
        }

        string fileName = Guid.NewGuid().ToString() + "-" + request.Photo.FileName;
        string path=Path.Combine(_env.WebRootPath,"img",fileName);
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            await request.Photo.CopyToAsync(stream);
        }

        Expert expert = new Expert
        {
            FullName = request.FullName,
            Position = request.Position,
            ImageUrl = fileName
        };
        await _dbContext.Experts.AddAsync(expert);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        Expert?expert=await _dbContext.Experts.FindAsync(id);
        if(expert==null)return NotFound();
        ExpertUpdateVM model = new ExpertUpdateVM
        {
            Id = expert.Id,
            FullName = expert.FullName,
            Position = expert.Position,
            ImageUrl = expert.ImageUrl
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, ExpertUpdateVM request)
    {
        if(!ModelState.IsValid)return View(request);
        Expert? expert = await _dbContext.Experts.FindAsync(id);
        if (expert == null) return NotFound();
        if (request.Photo != null)
        {
            if (!request.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo","Lütfen geçerli bir resim formatı seçin.");
                return View(request);
            }

            string oldPath = Path.Combine(_env.WebRootPath, "Img", expert.ImageUrl);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }

            string fileName = Guid.NewGuid().ToString() + "-" + request.Photo.FileName;
            string newPath = Path.Combine(_env.WebRootPath, "img", fileName);
            using (FileStream stream = new FileStream(newPath, FileMode.Create))
            {
                await request.Photo.CopyToAsync(stream);
            }

            expert.ImageUrl = fileName;
        }

        expert.FullName = request.FullName;
        expert.Position = request.Position;
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        Expert? expert = await _dbContext.Experts.FindAsync(id);
        if (expert == null) return NotFound();
        string path = Path.Combine(_env.WebRootPath, "img", expert.ImageUrl);
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
        _dbContext.Experts.Remove(expert);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Detail(int? id)
    {
        if (id is null) return BadRequest();
    
        var expert = await _dbContext.Experts.FirstOrDefaultAsync(e => e.Id == id);
        if (expert is null) return NotFound();

        return View(expert);
    }
}
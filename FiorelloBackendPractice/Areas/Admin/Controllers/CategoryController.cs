using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.Services.Interfaces;
using FiorelloBackendPractice.ViewModels.Category;
using Microsoft.AspNetCore.Mvc;

namespace FiorelloBackendPractice.Areas.Admin.Controllers;
[Area("Admin")]
public class CategoryController : Controller
{
    
    private readonly ICategoryService _categoryService;
    private readonly AppDbContext _context;
    public CategoryController(ICategoryService categoryService,AppDbContext context)
    {
        _categoryService = categoryService;
        _context = context;
    }
    public async Task< IActionResult> Index()
    {
      
        return View( await _categoryService.GetAllAsync());
    }
[HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryCreateVM request)
    {
        await _context.Categories.AddAsync(new Category
        {
            Name = request.Name
        });
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
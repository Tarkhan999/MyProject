using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.Services.Interfaces;
using FiorelloBackendPractice.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiorelloBackendPractice.Areas.Admin.Controllers;
[Authorize(Roles = "Admin,SuperAdmin")]
[Area("Admin")]

public class CategoryController : Controller
{
    
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService,AppDbContext context)
    {
        _categoryService = categoryService;
       
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
        if (!ModelState.IsValid)
        {
            return View();
        }
        await _categoryService.CreateAsync(request);
        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    
    public async Task<IActionResult> Delete(int id)
    {
        var category=await _categoryService.GetByIdAsync(id);
        await _categoryService.DeleteAsync(category);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var category=await _categoryService.GetByIdAsync(id);
        if (category == null) return NotFound();
        return View(new CategoryVM
        {
            Id = category.Id,
            Name = category.Name
        });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null) return NotFound();
        return View(new CategoryEditVM
        {
            Name = category.Name
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryEditVM request)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var category = await _categoryService.GetByIdAsync(id);
        if (category == null) return NotFound();
       await _categoryService.EditAsync(category, request);
        return RedirectToAction(nameof(Index));
    }
}
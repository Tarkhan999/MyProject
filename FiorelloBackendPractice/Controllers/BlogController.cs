using FiorelloBackendPractice.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FiorelloBackendPractice.Controllers;

public class BlogController : Controller
{
    private readonly IBlogService _blogService;

    public BlogController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var allblogs = await _blogService.GetAllAsync();

        return View(allblogs);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var blog = await _blogService.GetByIdAsync(id);
        if (blog == null) return NotFound();

        return View(blog);
    }
}
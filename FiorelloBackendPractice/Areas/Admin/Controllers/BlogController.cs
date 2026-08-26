using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.ViewModels.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Areas.Admin.Controllers;
[Area("Admin")]
public class BlogController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    public BlogController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }
    public async Task<IActionResult> Index()
    {
        List<BlogVM> blogs = await _context.Blogs
            .OrderByDescending(b => b.Id)
            .Select(b => new BlogVM
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                Image = b.Image,
                Link = b.Link,
                DateCreated = b.DateCreated,
                ViewsCount = b.ViewsCount
            }).ToListAsync();

        return View(blogs);
    }
    public async Task<IActionResult> Detail(int id)
    {
        Blog? blog = await _context.Blogs.FindAsync(id);
        if (blog == null) return NotFound();

        BlogDetailVM model = new BlogDetailVM
        {
            Id = blog.Id,
            Title = blog.Title,
            Description = blog.Description,
            Image = blog.Image,
            Link = blog.Link,
            DateCreated = blog.DateCreated,
            ViewsCount = blog.ViewsCount
        };

        return View(model);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BlogCreateVM request)
    {
        if (!ModelState.IsValid) return View(request);

        if (!request.Photo.ContentType.Contains("image/"))
        {
            ModelState.AddModelError("Photo", "Lütfen geçerli bir resim formatı yükleyin.");
            return View(request);
        }

        string fileName = Guid.NewGuid().ToString() + "-" + request.Photo.FileName;
        string path = Path.Combine(_env.WebRootPath, "img", fileName);

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            await request.Photo.CopyToAsync(stream);
        }

        Blog blog = new Blog
        {
            Title = request.Title,
            Description = request.Description,
            Link = request.Link ?? "#",
            Image = fileName,
            DateCreated = DateTime.Now,
            ViewsCount = 0
        };

        await _context.Blogs.AddAsync(blog);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        Blog? blog = await _context.Blogs.FindAsync(id);
        if (blog == null) return NotFound();

        BlogUpdateVM model = new BlogUpdateVM
        {
            Id = blog.Id,
            Title = blog.Title,
            Description = blog.Description,
            Link = blog.Link,
            Image = blog.Image
        };

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, BlogUpdateVM request)
    {
        if (!ModelState.IsValid) return View(request);

        Blog? blog = await _context.Blogs.FindAsync(id);
        if (blog == null) return NotFound();

        if (request.Photo != null)
        {
            if (!request.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "Lütfen geçerli bir resim formatı seçin.");
                return View(request);
            }

            string oldPath = Path.Combine(_env.WebRootPath, "img", blog.Image);
            if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);

            string fileName = Guid.NewGuid().ToString() + "-" + request.Photo.FileName;
            string newPath = Path.Combine(_env.WebRootPath, "img", fileName);

            using (FileStream stream = new FileStream(newPath, FileMode.Create))
            {
                await request.Photo.CopyToAsync(stream);
            }

            blog.Image = fileName;
        }

        blog.Title = request.Title;
        blog.Description = request.Description;
        blog.Link = request.Link ?? "#";

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        Blog? blog = await _context.Blogs.FindAsync(id);
        if (blog == null) return NotFound();

        string path = Path.Combine(_env.WebRootPath, "img", blog.Image);
        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);

        _context.Blogs.Remove(blog);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
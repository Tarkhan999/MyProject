using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Services.Interfaces;
using FiorelloBackendPractice.ViewModels.Blog;
using FiorelloBackendPractice.Views.Blog;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Services;

public class BlogService : IBlogService
{
    private readonly AppDbContext _dbContext;

    public BlogService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<BlogUIVM>> GetAllAsync(int? take = null)
    {
        IEnumerable<BlogUIVM> blogs = await _dbContext.Blogs
            .OrderByDescending(b => b.Id)
            .Select(b => new BlogUIVM
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                Image = b.Image,
                DateCreated = b.DateCreated
            }).ToListAsync();

        if (take is null)
            return blogs;

        return blogs.Take(take.Value);
    }

    public async Task<BlogDetailUIVM> GetByIdAsync(int id)
    {
        var blog = await _dbContext.Blogs.FirstOrDefaultAsync(m => m.Id == id);
        return new BlogDetailUIVM
        {
            Title = blog.Title,
            Description = blog.Description,
            Image = blog.Image,
            DateCreated = blog.DateCreated,
            Link = blog.Link,
            ViewsCount = blog.ViewsCount
        };
    }
}
using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.Services.Interfaces;
using FiorelloBackendPractice.ViewModels.Category;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Services;

public class CategoyService : ICategoryService
{
    private readonly AppDbContext _dbContext;

    public CategoyService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<CategoryUIVM>> GetAllUIAsync()
    {
        return await _dbContext.Categories.Select(c => new CategoryUIVM
        {
            Id = c.Id,
            Name = c.Name
        }).ToListAsync();
    }

    public async Task<IEnumerable<CategoryVM>> GetAllAsync()
    {
        return await _dbContext.Categories.OrderByDescending(m=>m.Id).Select(c => new CategoryVM
        {
            Id = c.Id,
            Name = c.Name
        }).ToListAsync();
    }

    public async Task<Category> GetByIdAsync(int id)
    {
        return await _dbContext.Categories.FindAsync(id);
    }

    public async Task CreateAsync(CategoryCreateVM request)
    {
        await _dbContext.Categories.AddAsync(new Category
        {
            Name = request.Name
        });
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditAsync(Category dbCategory, CategoryEditVM request)
    {
        dbCategory.Name = request.Name;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Category category)
    {
        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync();
        
    }
}
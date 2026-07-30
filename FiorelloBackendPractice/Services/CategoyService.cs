using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Services.Interfaces;
using FiorelloBackendPractice.ViewModels.Category;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Services;

public class CategoyService:ICategoryService
{
    private readonly AppDbContext _dbContext;
    public CategoyService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<CategoryUIVM>> GetAllAsync()
    {
        return await _dbContext.Categories.Select(c=>new CategoryUIVM
        {
            Id = c.Id,
            Name = c.Name,
        }).ToListAsync();
    }
}
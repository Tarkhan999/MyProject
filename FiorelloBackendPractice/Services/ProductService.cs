using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Services.Interfaces;
using FiorelloBackendPractice.Views.Product;
using Microsoft.EntityFrameworkCore;

namespace FiorelloBackendPractice.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _dbContext;

    public ProductService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ProductUIVM>> GetAllAsync()
    {
        var products = await _dbContext.Products.Include(p => p.Images).ToListAsync();

        var productVM = products.Select(m => new ProductUIVM
        {
            Id = m.Id,
            Name = m.Name,
            Price = m.Price,
            CategoryId = m.CategoryId,
            MainImage = m.Images.Where(m => m.IsMain).FirstOrDefault()?.Image
        });

        return productVM;
    }
}
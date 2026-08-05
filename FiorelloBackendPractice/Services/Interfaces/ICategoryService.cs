using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.ViewModels.Category;

namespace FiorelloBackendPractice.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryUIVM>> GetAllUIAsync();
    Task<IEnumerable<CategoryVM>> GetAllAsync();
    Task<Category> GetByIdAsync(int id);
    Task CreateAsync(CategoryCreateVM request);
    Task EditAsync(Category dbCategory, CategoryEditVM request);
    Task DeleteAsync(Category category);
}
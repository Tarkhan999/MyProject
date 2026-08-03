using FiorelloBackendPractice.ViewModels.Category;

namespace FiorelloBackendPractice.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryUIVM>> GetAllUIAsync();
    Task<IEnumerable<CategoryVM>> GetAllAsync();
}
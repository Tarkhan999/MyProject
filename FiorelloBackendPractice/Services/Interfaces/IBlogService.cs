using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.ViewModels.Blog;
using FiorelloBackendPractice.Views.Blog;

namespace FiorelloBackendPractice.Services.Interfaces;

public interface IBlogService
{

    Task<IEnumerable<BlogUIVM>> GetAllAsync(int? take = null);
    Task<BlogDetailUIVM>GetByIdAsync(int id);
}
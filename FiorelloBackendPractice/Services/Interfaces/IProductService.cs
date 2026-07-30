using FiorelloBackendPractice.Views.Product;

namespace FiorelloBackendPractice.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductUIVM>> GetAllAsync();
}
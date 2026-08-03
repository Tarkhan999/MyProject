using FiorelloBackendPractice.Services.Interfaces;
using FiorelloBackendPractice.ViewModels.Category;
using FiorelloBackendPractice.Views.Product;
using Microsoft.AspNetCore.Mvc;

namespace FiorelloBackendPractice.ViewComponents;

public class ProductViewComponent : ViewComponent
{
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;

    public ProductViewComponent(IProductService productService,
        ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var Categories = await _categoryService.GetAllUIAsync();
        var Products = await _productService.GetAllAsync();
        return await Task.FromResult(View(new ProductVCVM
        {
            Categories = Categories,
            Products = Products
        }));
    }
}

public class ProductVCVM
{
    public IEnumerable<CategoryUIVM> Categories { get; set; }
    public IEnumerable<ProductUIVM> Products { get; set; }
}
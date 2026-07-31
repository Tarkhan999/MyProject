using FiorelloBackendPractice.Services.Interfaces;
using FiorelloBackendPractice.ViewModels.Category;
using FiorelloBackendPractice.Views.Product;
using Microsoft.AspNetCore.Mvc;

namespace FiorelloBackendPractice.ViewComponents;

public class ProductViewComponent:ViewComponent
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    public ProductViewComponent(IProductService productService,
        ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        IEnumerable<CategoryUIVM> Categories = await _categoryService.GetAllAsync();
        IEnumerable<ProductUIVM> Products = await _productService.GetAllAsync();
        return await Task.FromResult(View(new ProductVCVM
        {
            Categories = Categories,
            Products = Products
        }));
    }
}

public class ProductVCVM
{
    public IEnumerable<CategoryUIVM>Categories { get; set; }
    public IEnumerable<ProductUIVM>  Products { get; set; }
}
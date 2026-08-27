namespace FiorelloBackendPractice.Views.Product;

public class ProductDetailVM
{
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public string CategoryName { get; set; }
    public List<ProductImageVM> Images { get; set; }
}
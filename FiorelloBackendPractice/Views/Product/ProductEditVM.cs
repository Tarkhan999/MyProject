namespace FiorelloBackendPractice.Views.Product;

public class ProductEditVM
{
    public int Id { get; set; }
    public string Image { get; set; }
    public IFormFile NewImage { get; set; }
}
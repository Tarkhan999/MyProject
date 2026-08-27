using Microsoft.AspNetCore.Http;


namespace FiorelloBackendPractice.ViewModels.Product
{
    public class ProductEditVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        
        
        public int CategoryId { get; set; } 
        
        public string? Image { get; set; } 
        public IFormFile? NewImage { get; set; } 
    }
}
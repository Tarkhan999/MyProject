using System.ComponentModel.DataAnnotations;
using FiorelloBackendPractice.Models;

namespace FiorelloBackendPractice.Views.Product;

public class ProductCreateVM
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public double Price { get; set; }
    [Required]
    public List<IFormFile> Images { get; set; }
    public int CategoryId { get; set; }
}
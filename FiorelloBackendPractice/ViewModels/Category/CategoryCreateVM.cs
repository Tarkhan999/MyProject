using System.ComponentModel.DataAnnotations;

namespace FiorelloBackendPractice.ViewModels.Category;

public class CategoryCreateVM
{
    [Required(ErrorMessage="Category name is required")]
    [MaxLength(20,ErrorMessage ="Category name cannot exceed 20 characters")]
    public string Name { get; set; }
}
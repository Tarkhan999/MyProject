using System.ComponentModel.DataAnnotations;

namespace FiorelloBackendPractice.ViewModels.Account;

public class RegisterVM
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }
    [Required(ErrorMessage = "FullName is required")]
    public string FullName { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    
    public string Password { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [Compare("Password", ErrorMessage = "Passwords do not must match")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
   
    [Required(ErrorMessage = "Username is required")]
    public string UserName { get; set; }
   
}
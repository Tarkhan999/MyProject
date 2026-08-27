using System.ComponentModel.DataAnnotations;

namespace FiorelloBackendPractice.ViewModels.Account;

public class LoginVM
{
    [Required(ErrorMessage = "Email or Username is required")]
    public string EmailOrUserName { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}
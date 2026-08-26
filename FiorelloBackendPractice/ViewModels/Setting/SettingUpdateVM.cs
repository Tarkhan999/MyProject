using System.ComponentModel.DataAnnotations;

namespace FiorelloBackendPractice.ViewModels.Setting;

public class SettingUpdateVM
{
    public int Id { get; set; }
    public string? ExistingHeaderLogo { get; set; }

    public IFormFile? HeaderLogoFile { get; set; }

    [Required(ErrorMessage = "Telefon numarası boş bırakılamaz.")]
    public string Phone { get; set; } = null!;

    [Required(ErrorMessage = "E-posta adresi boş bırakılamaz.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; } = null!;
}
using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Web.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "E-poçt mütləqdir.")]
    [EmailAddress(ErrorMessage = "E-poçt formatı yanlışdır.")]
    [Display(Name = "E-poçt")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parol mütləqdir.")]
    [DataType(DataType.Password)]
    [Display(Name = "Parol")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Məni xatırla")]
    public bool RememberMe { get; set; }
}

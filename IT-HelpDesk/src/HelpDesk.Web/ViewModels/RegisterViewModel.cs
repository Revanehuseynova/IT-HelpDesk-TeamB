using System.ComponentModel.DataAnnotations;
using HelpDesk.Web.Models;

namespace HelpDesk.Web.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Ad və soyad mütləqdir.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Ad və soyad 3-100 simvol olmalıdır.")]
    [Display(Name = "Ad və soyad")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-poçt mütləqdir.")]
    [EmailAddress(ErrorMessage = "E-poçt formatı yanlışdır.")]
    [StringLength(150, ErrorMessage = "E-poçt 150 simvoldan uzun ola bilməz.")]
    [Display(Name = "E-poçt")]
    public string Email { get; set; } = string.Empty;

    // Self-registration is limited to Student / Employee. IT specialists are created by an administrator.
    [Required(ErrorMessage = "Rol seçilməlidir.")]
    [RegularExpression("^(Student|Employee)$", ErrorMessage = "Yanlış rol seçilib.")]
    [Display(Name = "Rol")]
    public string Role { get; set; } = Roles.Student;

    [Required(ErrorMessage = "Parol mütləqdir.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Parol ən azı 8 simvol olmalıdır.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Parolda ən azı bir kiçik hərf, bir böyük hərf və bir rəqəm olmalıdır.")]
    [DataType(DataType.Password)]
    [Display(Name = "Parol")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parolun təkrarı mütləqdir.")]
    [Compare(nameof(Password), ErrorMessage = "Parollar uyğun gəlmir.")]
    [DataType(DataType.Password)]
    [Display(Name = "Parolun təkrarı")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

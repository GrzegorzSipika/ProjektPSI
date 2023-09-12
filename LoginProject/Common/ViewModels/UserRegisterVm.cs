using System.ComponentModel.DataAnnotations;

namespace LoginProject.Common.ViewModels;

public class UserRegisterVm
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*[#$^+=!*()@%&]).{8,}$",ErrorMessage ="Minimalna długość 8 i musi zawierać 1 wielką literę, 1 małą literę, 1 znak specjalny i 1 cyfrę.")]
    public string Password { get; set; }
    [Required]
    [Compare("Password", ErrorMessage = "Hasła nie są takie same")]
    public string PasswordConfirm { get; set; }
    public string? Role { get; set; } = "user";
}
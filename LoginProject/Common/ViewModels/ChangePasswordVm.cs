using System.ComponentModel.DataAnnotations;

namespace LoginProject.Common.ViewModels;

public class ChangePasswordVm
{
    [Required]
    public string ? CurrentPassword { get; set; }

    [Required]
    [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*[#$^+=!*()@%&]).{8,}$",ErrorMessage ="Minimalna długość 8 i musi zawierać 1 wielką literę, 1 małą literę, 1 znak specjalny i 1 cyfrę.")]
    public string? NewPassword { get; set; }
    [Required]
    [Compare("NewPassword", ErrorMessage = "Hasła nie są takie same")]
    public string ? PasswordConfirm { get; set; }
}
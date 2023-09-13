using System.ComponentModel.DataAnnotations;

namespace LoginProject.Common.ViewModels;

public class UserLoginVm
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}
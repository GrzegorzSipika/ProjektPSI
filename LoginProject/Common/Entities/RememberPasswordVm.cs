using System.ComponentModel.DataAnnotations;

namespace LoginProject.Common.Entities;

public class RememberPasswordVm
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
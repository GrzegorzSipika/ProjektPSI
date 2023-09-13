using Microsoft.AspNetCore.Identity;

namespace LoginProject.Common.Entities;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BlockedTo { get; set; }
    public bool IsBlocked => BlockedTo > DateTime.Now;

    public List<UserPasswordHistory> AllPasswords { get; set; } = new();
    public List<UserActionInfo> UserActionInfos { get; set; } = new();
}
namespace LoginProject.Common.Entities;

public class UserPasswordHistory : EntityBase
{
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string Password { get; set; }
}
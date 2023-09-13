using LoginProject.Common.Enums;

namespace LoginProject.Common.Entities;

public class UserActionInfo : EntityBase
{
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public ActionType ActionType { get; set; }
    public bool IsSuccess { get; set; }
}
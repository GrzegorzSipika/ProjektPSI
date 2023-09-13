using LoginProject.Common.Entities;

namespace LoginProject.Common.ViewModels;

public class HomeVm
{
    public List<UserActionInfo> UserActionInfos { get; set; } = new();
}
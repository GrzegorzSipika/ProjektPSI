using LoginProject.Common.Entities;

namespace LoginProject.Interfaces;

public interface IUserActionInfoRepository
{
    Task AddUserActionInfo(UserActionInfo userActionInfo);
    Task<List<UserActionInfo>> GetUserActionInfos(string userId);
    Task<bool> IsLastThreeLoginActionWasFailed(ApplicationUser user);
}
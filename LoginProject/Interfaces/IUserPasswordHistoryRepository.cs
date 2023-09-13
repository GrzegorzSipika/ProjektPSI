using LoginProject.Common.Entities;

namespace LoginProject.Interfaces;

public interface IUserPasswordHistoryRepository
{
    Task AddPasswordToHistory(string userId, string password);
    Task<bool> IsPasswordWasUsedInLastTwentyPasswords(ApplicationUser user, string password);
    Task<bool> IsPasswordWasChangedLessThenXDays(ApplicationUser user, int days);
    Task<bool> IsPasswordExpired(string userName);
}
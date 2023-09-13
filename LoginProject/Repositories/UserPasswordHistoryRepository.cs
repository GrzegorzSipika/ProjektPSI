using LoginProject.Common.Entities;
using LoginProject.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoginProject.Repositories;

public class UserPasswordHistoryRepository : BaseRepository, IUserPasswordHistoryRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserPasswordHistoryRepository(IDatabaseContext databaseContext, UserManager<ApplicationUser> userManager) : base(databaseContext)
    {
        _userManager = userManager;
    }
    
    public async Task AddPasswordToHistory(string userId, string password)
    {
        try
        {
            var userPasswordHistory = new UserPasswordHistory
            {
                UserId = userId,
                Password = password
            };

            await _databaseContext.UserPasswordsHistory.AddAsync(userPasswordHistory);
            await _databaseContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> IsPasswordWasUsedInLastTwentyPasswords(ApplicationUser user, string password)
    {
        try
        {
            var lastTwentyPasswords = await _databaseContext
                .UserPasswordsHistory
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.CreatedBy)
                .Take(20)
                .ToListAsync();
            
            return lastTwentyPasswords.Any(x => x.Password == password);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> IsPasswordWasChangedLessThenXDays(ApplicationUser user, int days)
    {
        var lastPasswordChange = await _databaseContext
            .UserPasswordsHistory
            .Where(x => x.UserId == user.Id)
            .OrderByDescending(x => x.CreatedBy)
            .FirstOrDefaultAsync();
        
        if (lastPasswordChange == null)
        {
            return false;
        }
        
        return lastPasswordChange.Created.AddDays(days) > DateTime.Now;
    }

    public async Task<bool> IsPasswordExpired(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        
        var userPasswords = await _databaseContext
            .UserPasswordsHistory
            .Where(x => x.UserId == user.Id)
            .OrderByDescending(x => x.Created)
            .ToListAsync();
        
        var lastPasswordChange = userPasswords.FirstOrDefault();

        if (lastPasswordChange == null)
            return false;
        
        if (lastPasswordChange.Created.AddDays(90) > DateTime.Now)
            return false;
        
        return true;
    }
}
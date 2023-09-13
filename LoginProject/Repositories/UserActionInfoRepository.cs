using LoginProject.Common.Entities;
using LoginProject.Common.Enums;
using LoginProject.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoginProject.Repositories;

public class UserActionInfoRepository : BaseRepository, IUserActionInfoRepository
{
    
    public UserActionInfoRepository(IDatabaseContext databaseContext) : base(databaseContext)
    {
    }

    public async Task AddUserActionInfo(UserActionInfo userActionInfo)
    {
        try
        {
            await _databaseContext.UserActionInfos.AddAsync(userActionInfo);
            await _databaseContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<UserActionInfo>> GetUserActionInfos(string userId)
    {
        try
        {
            return await _databaseContext
                .UserActionInfos
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedBy)
                .ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<bool> IsLastThreeLoginActionWasFailed(ApplicationUser user)
    {
        try
        {
            var userActionInfos = await _databaseContext
                .UserActionInfos
                .Where(x => x.UserId == user.Id && x.Created >= user.BlockedTo)
                .OrderByDescending(x => x.CreatedBy)
                .Take(3)
                .ToListAsync();
            

            return userActionInfos.Where(x => x.IsSuccess == false).Count() == 3;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
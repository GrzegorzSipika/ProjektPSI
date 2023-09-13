using LoginProject.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoginProject.Interfaces;

public interface IDatabaseContext
{
    DbSet<UserActionInfo> UserActionInfos { get; set; }
    DbSet<UserPasswordHistory> UserPasswordsHistory { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}
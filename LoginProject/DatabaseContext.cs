using System.Reflection;
using LoginProject.Common;
using LoginProject.Common.Entities;
using LoginProject.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoginProject;

public class DatabaseContext : IdentityDbContext<ApplicationUser>, IDatabaseContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        
    }
    
    public DatabaseContext(DbContextOptions<DatabaseContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<UserPasswordHistory>()
            .HasOne(x => x.User)
            .WithMany(x => x.AllPasswords)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<UserActionInfo>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserActionInfos)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }

    public DbSet<UserActionInfo> UserActionInfos { get; set; }
    public DbSet<UserPasswordHistory> UserPasswordsHistory { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            switch (entry.State)
            {
                case EntityState.Deleted:
                    entry.Entity.StatusId = 0;
                    entry.State = EntityState.Modified;
                    break;
                case EntityState.Modified:
                    break;
                case EntityState.Added:
                    entry.Entity.CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name ?? "";
                    entry.Entity.Created = DateTime.Now;
                    entry.Entity.StatusId = 1;
                    break;
                default:
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
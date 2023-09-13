using LoginProject.Common.Entities;
using LoginProject.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LoginProject.Repositories;

public class BaseRepository
{
    protected readonly IDatabaseContext _databaseContext;

    public BaseRepository(IDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
}
using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FunEvents.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly FunEventsDbContext _dbContext;

    public UserRepository(FunEventsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();

        return _dbContext.Users
            .SingleOrDefaultAsync(
                x => x.Code == normalizedCode,
                cancellationToken);
    }
}
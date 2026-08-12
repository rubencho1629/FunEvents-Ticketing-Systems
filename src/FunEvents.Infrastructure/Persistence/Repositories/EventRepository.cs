using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FunEvents.Infrastructure.Persistence.Repositories;

public sealed class EventRepository : IEventRepository
{
    private readonly FunEventsDbContext _dbContext;

    public EventRepository(FunEventsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Event?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();

        return _dbContext.Events
            .SingleOrDefaultAsync(
                x => x.Code == normalizedCode,
                cancellationToken);
    }
}
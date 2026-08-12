using FunEvents.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FunEvents.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        FunEventsDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Events.AnyAsync(cancellationToken))
        {
            var events = new[]
            {
                new Event(
                    "EVENT-001",
                    "Rock Legends Live",
                    DateTimeOffset.UtcNow.AddMonths(1),
                    100),

                new Event(
                    "EVENT-002",
                    "Theatre Night",
                    DateTimeOffset.UtcNow.AddMonths(2),
                    50)
            };

            await dbContext.Events.AddRangeAsync(
                events,
                cancellationToken);
        }

        if (!await dbContext.Users.AnyAsync(cancellationToken))
        {
            var users = new[]
            {
                new User(
                    "USER-001",
                    "Test User"),

                new User(
                    "USER-002",
                    "Guest User")
            };

            await dbContext.Users.AddRangeAsync(
                users,
                cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FunEvents.Infrastructure.Persistence;

public sealed class FunEventsDbContextFactory
    : IDesignTimeDbContextFactory<FunEventsDbContext>
{
    public FunEventsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<FunEventsDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=funevents;Username=postgres;Password=postgres");

        return new FunEventsDbContext(optionsBuilder.Options);
    }
}
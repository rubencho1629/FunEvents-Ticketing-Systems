using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Domain.Entities;
using FunEvents.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FunEvents.Infrastructure.Persistence;

public sealed class FunEventsDbContext : DbContext, IUnitOfWork
{
    public FunEventsDbContext(
        DbContextOptions<FunEventsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(FunEventsDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException(
                "The event availability changed while the reservation was being processed. Please try again.");
        }
    }
}
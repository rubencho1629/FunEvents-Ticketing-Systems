using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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
}
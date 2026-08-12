using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Domain.Entities;

namespace FunEvents.Infrastructure.Persistence.Repositories;

public sealed class BookingRepository : IBookingRepository
{
    private readonly FunEventsDbContext _dbContext;

    public BookingRepository(FunEventsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(
        Booking booking,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Bookings
            .AddAsync(booking, cancellationToken)
            .AsTask();
    }
}
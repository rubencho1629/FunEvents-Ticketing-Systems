using FunEvents.Domain.Entities;

namespace FunEvents.Application.Abstractions.Persistence;

public interface IBookingRepository
{
    Task AddAsync(
        Booking booking,
        CancellationToken cancellationToken = default);
}
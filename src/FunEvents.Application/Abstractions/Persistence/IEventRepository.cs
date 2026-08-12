using FunEvents.Domain.Entities;

namespace FunEvents.Application.Abstractions.Persistence;

public interface IEventRepository
{
    Task<Event?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);
}
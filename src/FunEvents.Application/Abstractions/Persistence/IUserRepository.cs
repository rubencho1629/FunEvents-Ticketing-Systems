using FunEvents.Domain.Entities;

namespace FunEvents.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<User?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);
}
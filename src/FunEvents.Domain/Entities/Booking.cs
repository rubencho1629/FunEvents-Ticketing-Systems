namespace FunEvents.Domain.Entities;

public sealed class Booking
{
    public Guid Id { get; private set; }

    public Guid EventId { get; private set; }

    public Guid UserId { get; private set; }

    public int Quantity { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    private Booking()
    {
        // Required by EF Core
    }

    public Booking(
        Guid eventId,
        Guid userId,
        int quantity)
    {
        if (eventId == Guid.Empty)
        {
            throw new ArgumentException(
                "Event id is required.",
                nameof(eventId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User id is required.",
                nameof(userId));
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Ticket quantity must be greater than zero.");
        }

        Id = Guid.NewGuid();
        EventId = eventId;
        UserId = userId;
        Quantity = quantity;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}
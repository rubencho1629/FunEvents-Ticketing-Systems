namespace FunEvents.Domain.Entities;

public sealed class Event
{
    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public DateTimeOffset Date { get; private set; }
    public int Capacity { get; private set; }
    public int AvailableTickets { get; private set; }

    private Event()
    {
        // Required by EF Core
    }

    public Event(
        string code,
        string name,
        DateTimeOffset date,
        int capacity)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Event code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Event name is required.", nameof(name));

        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(capacity),
                "Event capacity must be greater than zero.");

        Id = Guid.NewGuid();
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Date = date;
        Capacity = capacity;
        AvailableTickets = capacity;
    }

    public void ReserveTickets(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Ticket quantity must be greater than zero.");

        if (quantity > AvailableTickets)
            throw new InvalidOperationException(
                "There are not enough tickets available.");

        AvailableTickets -= quantity;
    }
}
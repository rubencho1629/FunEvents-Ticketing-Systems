namespace FunEvents.Application.Bookings.ReserveTickets;

public sealed record ReserveTicketsResult(
    Guid BookingId,
    string EventCode,
    string UserCode,
    int Quantity,
    int RemainingTickets,
    DateTimeOffset CreatedAt);
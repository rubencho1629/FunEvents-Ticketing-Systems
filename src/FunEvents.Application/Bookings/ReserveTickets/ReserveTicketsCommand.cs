namespace FunEvents.Application.Bookings.ReserveTickets;

public sealed record ReserveTicketsCommand(
    string EventCode,
    string UserCode,
    int Quantity);
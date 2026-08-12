using FunEvents.Application.Bookings.ReserveTickets;

namespace FunEvents.Api.Endpoints;

public static class BookingsEndpoints
{
    public static IEndpointRouteBuilder MapBookingsEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/v1/bookings")
            .WithTags("Bookings");

        group.MapPost("/", ReserveTickets)
            .WithName("ReserveTickets")
            .WithSummary("Reserve tickets for an event")
            .Produces<ReserveTicketsResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return endpoints;
    }

    private static async Task<IResult> ReserveTickets(
        ReserveTicketsRequest request,
        ReserveTicketsHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new ReserveTicketsCommand(
            request.EventCode,
            request.UserCode,
            request.Quantity);

        var result = await handler.HandleAsync(
            command,
            cancellationToken);

        return Results.Created(
            $"/api/v1/bookings/{result.BookingId}",
            result);
    }

    private sealed record ReserveTicketsRequest(
        string EventCode,
        string UserCode,
        int Quantity);
}
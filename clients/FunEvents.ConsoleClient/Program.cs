using System.Net.Http.Json;
using System.Text.Json;

const string baseUrl = "https://localhost:7228";

using var httpClient = new HttpClient
{
    BaseAddress = new Uri(baseUrl)
};

Console.WriteLine("=================================");
Console.WriteLine("        FunEvents Client");
Console.WriteLine("=================================");
Console.WriteLine();

Console.Write("Event code: ");
var eventCode = Console.ReadLine();

Console.Write("User code: ");
var userCode = Console.ReadLine();

Console.Write("Number of tickets: ");
var quantityInput = Console.ReadLine();

if (!int.TryParse(quantityInput, out var quantity))
{
    Console.WriteLine();
    Console.WriteLine("Invalid ticket quantity.");
    return;
}

var request = new ReserveTicketsRequest(
    eventCode ?? string.Empty,
    userCode ?? string.Empty,
    quantity);

try
{
    var response = await httpClient.PostAsJsonAsync(
        "/api/v1/bookings",
        request);

    if (response.IsSuccessStatusCode)
    {
        var reservation = await response.Content
            .ReadFromJsonAsync<ReserveTicketsResponse>();

        if (reservation is null)
        {
            Console.WriteLine("The API returned an empty response.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Reservation successful!");
        Console.WriteLine();
        Console.WriteLine($"Booking ID:        {reservation.BookingId}");
        Console.WriteLine($"Event:             {reservation.EventCode}");
        Console.WriteLine($"User:              {reservation.UserCode}");
        Console.WriteLine($"Tickets:           {reservation.Quantity}");
        Console.WriteLine($"Remaining tickets: {reservation.RemainingTickets}");
        Console.WriteLine($"Created at:         {reservation.CreatedAt}");
    }
    else
    {
        var errorContent = await response.Content.ReadAsStringAsync();

        Console.WriteLine();
        Console.WriteLine($"Reservation failed ({(int)response.StatusCode}).");

        try
        {
            var problem = JsonSerializer.Deserialize<ProblemDetailsResponse>(
                errorContent,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            Console.WriteLine(
                problem?.Detail
                ?? "An error occurred while processing the reservation.");
        }
        catch (JsonException)
        {
            Console.WriteLine(errorContent);
        }
    }
}
catch (HttpRequestException ex)
{
    Console.WriteLine();
    Console.WriteLine("Could not connect to the FunEvents API.");
    Console.WriteLine(ex.Message);
}

Console.WriteLine();
Console.WriteLine("Press ENTER to exit...");
Console.ReadLine();

public sealed record ReserveTicketsRequest(
    string EventCode,
    string UserCode,
    int Quantity);

public sealed record ReserveTicketsResponse(
    Guid BookingId,
    string EventCode,
    string UserCode,
    int Quantity,
    int RemainingTickets,
    DateTimeOffset CreatedAt);

public sealed record ProblemDetailsResponse(
    string? Title,
    int? Status,
    string? Detail);
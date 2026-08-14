using System.Net;
using System.Net.Http.Json;
using FunEvents.Domain.Entities;
using FunEvents.Infrastructure.Persistence;
using FunEvents.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FunEvents.IntegrationTests.Bookings;

public sealed class BookingsEndpointsTests : IAsyncDisposable
{
    private readonly FunEventsWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public BookingsEndpointsTests()
    {
        _factory = new FunEventsWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task ReserveTickets_WithValidRequest_ReturnsCreated()
    {
        await SeedDatabaseAsync();

        var request = new
        {
            eventCode = "EVENT-001",
            userCode = "USER-001",
            quantity = 2
        };

        var response = await _client.PostAsJsonAsync(
            "/api/v1/bookings",
            request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content
            .ReadFromJsonAsync<ReserveTicketsResponse>();

        Assert.NotNull(result);
        Assert.Equal("EVENT-001", result.EventCode);
        Assert.Equal("USER-001", result.UserCode);
        Assert.Equal(2, result.Quantity);
        Assert.Equal(8, result.RemainingTickets);
        Assert.NotEqual(Guid.Empty, result.BookingId);
    }

    [Fact]
    public async Task ReserveTickets_WhenEventDoesNotExist_ReturnsNotFound()
    {
        await SeedDatabaseAsync();

        var request = new
        {
            eventCode = "EVENT-NOT-FOUND",
            userCode = "USER-001",
            quantity = 1
        };

        var response = await _client.PostAsJsonAsync(
            "/api/v1/bookings",
            request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ReserveTickets_WhenQuantityIsInvalid_ReturnsBadRequest()
    {
        await SeedDatabaseAsync();

        var request = new
        {
            eventCode = "EVENT-001",
            userCode = "USER-001",
            quantity = 0
        };

        var response = await _client.PostAsJsonAsync(
            "/api/v1/bookings",
            request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReserveTickets_WhenTicketsAreInsufficient_ReturnsConflict()
    {
        await SeedDatabaseAsync();

        var request = new
        {
            eventCode = "EVENT-001",
            userCode = "USER-001",
            quantity = 20
        };

        var response = await _client.PostAsJsonAsync(
            "/api/v1/bookings",
            request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    private async Task SeedDatabaseAsync()
    {
        await using var scope = _factory.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<FunEventsDbContext>();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var eventEntity = new Event(
            "EVENT-001",
            "Integration Test Event",
            DateTimeOffset.UtcNow.AddDays(30),
            10);

        var user = new User(
            "USER-001",
            "Integration Test User");

        dbContext.Events.Add(eventEntity);
        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    private sealed record ReserveTicketsResponse(
        Guid BookingId,
        string EventCode,
        string UserCode,
        int Quantity,
        int RemainingTickets,
        DateTimeOffset CreatedAt);
}
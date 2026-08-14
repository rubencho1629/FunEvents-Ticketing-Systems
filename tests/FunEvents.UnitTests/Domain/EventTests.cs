using FunEvents.Domain.Entities;
using FunEvents.Domain.Exceptions;

namespace FunEvents.UnitTests.Domain;

public sealed class EventTests
{
    [Fact]
    public void ReserveTickets_WhenTicketsAreAvailable_DecreasesAvailability()
    {
        var eventEntity = new Event(
            "EVENT-001",
            "Test Event",
            DateTimeOffset.UtcNow.AddDays(10),
            10);

        eventEntity.ReserveTickets(3);

        Assert.Equal(7, eventEntity.AvailableTickets);
    }

    [Fact]
    public void ReserveTickets_WhenQuantityIsZero_ThrowsArgumentOutOfRangeException()
    {
        var eventEntity = new Event(
            "EVENT-001",
            "Test Event",
            DateTimeOffset.UtcNow.AddDays(10),
            10);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => eventEntity.ReserveTickets(0));
    }

    [Fact]
    public void ReserveTickets_WhenQuantityExceedsAvailability_ThrowsConflictException()
    {
        var eventEntity = new Event(
            "EVENT-001",
            "Test Event",
            DateTimeOffset.UtcNow.AddDays(10),
            2);

        Assert.Throws<ConflictException>(
            () => eventEntity.ReserveTickets(3));
    }

    [Fact]
    public void Constructor_NormalizesEventCode()
    {
        var eventEntity = new Event(
            " event-001 ",
            "Test Event",
            DateTimeOffset.UtcNow.AddDays(10),
            10);

        Assert.Equal("EVENT-001", eventEntity.Code);
    }
}
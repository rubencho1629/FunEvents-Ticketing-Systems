using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Domain.Entities;

namespace FunEvents.Application.Bookings.ReserveTickets;

public sealed class ReserveTicketsHandler
{
    private readonly IEventRepository _eventRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReserveTicketsHandler(
        IEventRepository eventRepository,
        IUserRepository userRepository,
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _userRepository = userRepository;
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReserveTicketsResult> HandleAsync(
        ReserveTicketsCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.EventCode))
        {
            throw new ArgumentException(
                "Event code is required.",
                nameof(command.EventCode));
        }

        if (string.IsNullOrWhiteSpace(command.UserCode))
        {
            throw new ArgumentException(
                "User code is required.",
                nameof(command.UserCode));
        }

        if (command.Quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(command.Quantity),
                "Ticket quantity must be greater than zero.");
        }

        var eventEntity = await _eventRepository.GetByCodeAsync(
            command.EventCode,
            cancellationToken);

        if (eventEntity is null)
        {
            throw new InvalidOperationException(
                $"Event '{command.EventCode}' was not found.");
        }

        var user = await _userRepository.GetByCodeAsync(
            command.UserCode,
            cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException(
                $"User '{command.UserCode}' was not found.");
        }

        eventEntity.ReserveTickets(command.Quantity);

        var booking = new Booking(
            eventEntity.Id,
            user.Id,
            command.Quantity);

        await _bookingRepository.AddAsync(
            booking,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new ReserveTicketsResult(
            booking.Id,
            eventEntity.Code,
            user.Code,
            booking.Quantity,
            eventEntity.AvailableTickets,
            booking.CreatedAt);
    }
}
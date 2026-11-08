using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Bookings.ReserveBooking;
using Bookify.Application.Exceptions;
using Bookify.Application.UnitTests.Apartments;
using Bookify.Application.UnitTests.Users;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
namespace Bookify.Application.UnitTests.Bookings;

public sealed class ReserveBookingTests
{
    private static readonly DateTime UtcNow = DateTime.UtcNow;
    private static readonly ReserveBookingCommand command = new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        new DateOnly(2026, 11, 7),
        new DateOnly(2026, 11, 8));

    private readonly ReserveBookingCommandHandler _handler; // InternalVisibleTo

    private readonly IUserRepository userRepositoryMock;
    private readonly IApartmentRepository apartmentRepositoryMock;
    private readonly IBookingRepository bookingRepositoryMock;
    private readonly IUnitOfWork unitOfWorkMock;
    private readonly IDateTimeProvider dateTimeProviderMock;

    public ReserveBookingTests()
    {
        userRepositoryMock = Substitute.For<IUserRepository>();
        apartmentRepositoryMock = Substitute.For<IApartmentRepository>();
        bookingRepositoryMock = Substitute.For<IBookingRepository>();
        unitOfWorkMock = Substitute.For<IUnitOfWork>();

        dateTimeProviderMock = Substitute.For<IDateTimeProvider>();
        dateTimeProviderMock.UtcNow.Returns(UtcNow);

        _handler = new ReserveBookingCommandHandler(
            userRepositoryMock,
            apartmentRepositoryMock,
            bookingRepositoryMock,
            unitOfWorkMock,
            new PricingService(),
            dateTimeProviderMock);
    }

    [Fact]
    public async Task Handler_Should_RetrunFailure_WhenUserIsNull()
    {
        // Arrange
        userRepositoryMock
            .GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Error.Should().Be(UserErrors.NotFound);
    }

    [Fact]
    public async Task Handler_Should_RetrunFailure_WhenApartmentIsNull()
    {
        // Arrange
        var user = UserData.Create();
        userRepositoryMock
            .GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        apartmentRepositoryMock
            .GetByIdAsync(command.ApartmentId, Arg.Any<CancellationToken>())
            .Returns((Apartment?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Error.Should().Be(ApartmentErrors.NotFound);
    }

    [Fact]
    public async Task Handler_Should_RetrunFailure_WhenApartmentIsBooked()
    {
        // Arrange
        var user = UserData.Create();
        userRepositoryMock
            .GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        var apartment = ApartmentData.Create();
        apartmentRepositoryMock
            .GetByIdAsync(command.ApartmentId, Arg.Any<CancellationToken>())
            .Returns(apartment);

        var duration = DateRange.Create(command.StartDate, command.EndDate);
        bookingRepositoryMock
            .IsOverlappingAsync(apartment, duration, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Error.Should().Be(BookingErrors.Overlap);
    }

    [Fact]
    public async Task Handler_Should_RetrunFailure_WhenUnitOfWorkThrows()
    {
        // Arrange
        var user = UserData.Create();
        userRepositoryMock
            .GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        var apartment = ApartmentData.Create();
        apartmentRepositoryMock
            .GetByIdAsync(command.ApartmentId, Arg.Any<CancellationToken>())
            .Returns(apartment);

        var duration = DateRange.Create(command.StartDate, command.EndDate);
        bookingRepositoryMock
            .IsOverlappingAsync(apartment, duration, Arg.Any<CancellationToken>())
            .Returns(true);

        unitOfWorkMock.SaveChangesAsync().ThrowsAsync(new ConcurrencyException("Concurrency", new Exception()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Error.Should().Be(BookingErrors.Overlap);
    }

    [Fact]
    public async Task Handler_Should_RetrunSuccess_WhenBookingIsReserved()
    {
        // Arrange
        var user = UserData.Create();
        userRepositoryMock
            .GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        var apartment = ApartmentData.Create();
        apartmentRepositoryMock
            .GetByIdAsync(command.ApartmentId, Arg.Any<CancellationToken>())
            .Returns(apartment);

        var duration = DateRange.Create(command.StartDate, command.EndDate);
        bookingRepositoryMock
            .IsOverlappingAsync(apartment, duration, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handler_Should_CallBookingRepo_WhenBookingIsReserved()
    {
        // Arrange
        var user = UserData.Create();
        userRepositoryMock
            .GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        var apartment = ApartmentData.Create();
        apartmentRepositoryMock
            .GetByIdAsync(command.ApartmentId, Arg.Any<CancellationToken>())
            .Returns(apartment);

        var duration = DateRange.Create(command.StartDate, command.EndDate);
        bookingRepositoryMock
            .IsOverlappingAsync(apartment, duration, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        bookingRepositoryMock.Received(1).Add(Arg.Is<Booking>(b => b.Id == result.Value));
    }
}

using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Shared;
using Bookify.Domain.UnitTests.Apartments;
using Bookify.Domain.UnitTests.Infrastructure;
using Bookify.Domain.UnitTests.Users;
using Bookify.Domain.Users;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Bookings;

public sealed class BookingTests : BaseTest
{
    [Fact]
    public void Reserve_Should_RaiseBookingReservedDomainEvent()
    {
        // Arrange
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);
        var price = new Money(10.0m, Currency.USD);
        var period = DateRange.Create(new DateOnly(2026, 11, 7), new DateOnly(2026, 11, 8));
        var expectedTotalPrice = new Money(price.Amount * period.LengthInDays, price.Currency);
        var aparment = ApartmentData.Create(price);
        var pricingSerice = new PricingService();

        // Act
        var booking = Booking.Reserve(aparment, user.Id, period, DateTime.UtcNow, pricingSerice);

        // Assert
        var bookingReservedDomainEvent = AssertDomainEventWasPublished<BookingReservedDomainEvent>(booking);
        bookingReservedDomainEvent.BookingId.Should().Be(booking.Id);
    }
}
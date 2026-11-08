using Bookify.Application.Bookings.GetBooking;
using Bookify.Application.IntergrationTests.Infrastructure;
using Bookify.Domain.Bookings;
using FluentAssertions;

namespace Bookify.Application.IntergrationTests.Bookings;

public class ConfirmBookingTests : BaseIntergrationTest
{
    private static readonly Guid BookingId = Guid.NewGuid();
    public ConfirmBookingTests(IntergrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task ConfirmBooking_Should_ReturnFailure_WhenBookingIsNotFound()
    {
        // Arrange
        var query = new GetBookingQuery(BookingId);

        // Act
        var result = await sender.Send(query);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.NotFound);
    }
}
using Bookify.Application.Bookings.ConfirmBooking;
using Bookify.Application.IntergrationTests.Infrastructure;
using Bookify.Domain.Bookings;
using FluentAssertions;

namespace Bookify.Application.IntergrationTests.Bookings;

public class GetBookingTests : BaseIntergrationTest
{
    private static readonly Guid BookingId = Guid.NewGuid();
    public GetBookingTests(IntergrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetBooking_Should_ReturnFailure_WhenBookingIsNotFound()
    {
        // Arrange
        var comman = new ConfirmBookingCommand(BookingId);

        // Act
        var result = await sender.Send(comman);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.NotFound);
    }
}
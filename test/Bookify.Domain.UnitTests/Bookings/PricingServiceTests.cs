
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Shared;
using Bookify.Domain.UnitTests.Apartments;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Bookings;

public sealed class PricingServiceTests
{
    [Fact]
    public void CalculatePrice_Should_ReturnCorrectTotalPrice()
    {
        // Arrange
        var price = new Money(10.0m, Currency.USD);
        var period = DateRange.Create(new DateOnly(2026, 11, 7), new DateOnly(2026, 11, 8));
        var expectedTotalPrice = new Money(price.Amount * period.LengthInDays, price.Currency);
        var aparment = ApartmentData.Create(price);
        var pricingSerice = new PricingService();

        // Act
        var priceDetails = pricingSerice.CalculatePrice(aparment, period);

        // Assert
        priceDetails.TotalPrice.Should().Be(expectedTotalPrice);
    }
}
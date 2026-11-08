using Bookify.Application.Arpartments;
using Bookify.Application.IntergrationTests.Infrastructure;
using FluentAssertions;

namespace Bookify.Application.IntergrationTests.Apartments;

public class SearchApartmentTests : BaseIntergrationTest
{
    public SearchApartmentTests(IntergrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task SearchApartments_Should_ReturnEmpytList_WhenDateRangeIsValid()
    {
        // Arrange
        var query = new SearchApartmentsQuery(
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1))); // startDate > endDate

        // Act
        var result = await sender.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchApartments_Should_ReturnApartments_WhenDateRangeIsValid()
    {
        // Arrange
        var query = new SearchApartmentsQuery(
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow));

        // Act
        var result = await sender.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }
}
using Bookify.Application.Arpartments;
using MediatR;

namespace Bookify.Api.Controllers.Apartments;

public static class ApartmentEndpoints
{
    public static IEndpointRouteBuilder MapApartmentsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/v{version:apiVersion}/bookings/{id}", SearchApartments)
            .RequireAuthorization();

        return builder;
    }

    public static async Task<IResult> SearchApartments(
        DateOnly startDate,
        DateOnly endDate,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new SearchApartmentsQuery(startDate, endDate);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result.Value);
    }
}

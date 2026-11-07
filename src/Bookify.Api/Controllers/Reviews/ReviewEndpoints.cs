using Bookify.Application.Reviews.AddReview;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Reviews;

public static class ReviewEndpoints
{
    public static IEndpointRouteBuilder MapReviewEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/reviews", AddReview)
            .RequireAuthorization()
            .HasApiVersion(1);
        return builder;
    }
    [HttpPost]
    public static async Task<IResult> AddReview(
        AddReviewRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new AddReviewCommand(request.BookingId, request.Rating, request.Comment);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Results.BadRequest(result.Error);
        }

        return Results.Ok();
    }
}

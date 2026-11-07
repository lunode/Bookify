using Asp.Versioning;
using Asp.Versioning.Builder;
using Bookify.Application.Users.GetLoggedInUser;
using Bookify.Application.Users.LogInUser;
using Bookify.Application.Users.RegisterUser;
using MediatR;

namespace Bookify.Api.Controllers.Users;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder builder)
    {
        ApiVersionSet apiVersionSet = builder.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        builder.MapGet("/api/v{version:apiVersion}users/me", GetLoggedInUser)
            // .RequireAuthorization(new HasPermissionAttribute(Permissions.UsersRead))
            .RequireAuthorization(Permissions.UsersRead)
            .WithApiVersionSet(apiVersionSet);

        builder.MapGet("/api/v{version:apiVersion}/users/register", RegisterUser)
            .HasApiVersion(1);

        builder.MapGet("/api/v{version:apiVersion}/users/login", LogIn)
            .WithApiVersionSet(apiVersionSet);

        return builder;
    }
    public static async Task<IResult> GetLoggedInUser(
        ISender sender, CancellationToken cancellationToken)
    {
        var query = new GetLoggedInUserQuery();
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result.Value);
    }

    public static async Task<IResult> RegisterUser(
        RegisterUserRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var cmd = new RegisterUserCommand(
            request.Email,
            request.FirstName,
            request.LastName,
            request.Password);
        var result = await sender.Send(cmd, cancellationToken);
        return Results.Ok(result.Value);
    }

    public static async Task<IResult> LogIn(
        LogInUserRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new LogInUserCommand(request.Email, request.Password);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(result.Value);
    }
}

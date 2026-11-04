using Bookify.Application.Users.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Users;

[Route("api/[controller]")]
[ApiController]
public class UsersController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var cmd = new RegisterUserCommand(
            request.Email,
            request.FirstName,
            request.LastName,
            request.Password);
        var result = await sender.Send(cmd, cancellationToken);
        return Ok(result.Value);
    }
}

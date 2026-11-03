using Bookify.Application.Arpartments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Apartments;


[Route("api/[controller]")]
[ApiController]
public class ApartmentsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> SearchApartments(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var query = new SearchApartmentsQuery(startDate, endDate);
        var result = await sender.Send(query, cancellationToken);
        return Ok(result.Value);
    }
}

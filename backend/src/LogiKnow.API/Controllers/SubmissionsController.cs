using LogiKnow.Application.Submissions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LogiKnow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubmissionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SubmissionsController> _logger;

    public SubmissionsController(IMediator mediator, ILogger<SubmissionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // User submissions are handled through domain-specific controllers
    // (AcademicController.Submit, etc.) This controller is reserved
    // for future generic submission management endpoints.

    [HttpGet("my")]
    public async Task<IActionResult> GetMySubmissions(
        [FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        _logger.LogDebug("GetMySubmissions for User {UserId}", userId);
        
        var query = new GetMySubmissionsQuery(userId, page, size);
        var response = await _mediator.Send(query, ct);
        
        return Ok(response);
    }
}

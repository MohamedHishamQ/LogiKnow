using LogiKnow.Application.Common.DTOs;
using LogiKnow.Application.Moderation.Commands;
using LogiKnow.Application.Moderation.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LogiKnow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Moderator")]
public class ModerationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ModerationController> _logger;

    public ModerationController(IMediator mediator, ILogger<ModerationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(
        [FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
    {
        _logger.LogDebug("GetPending submissions");
        
        var query = new GetPendingSubmissionsQuery(page, size);
        var response = await _mediator.Send(query, ct);
        
        return Ok(response);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] ReviewSubmissionRequest? request, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogDebug("Approve submission: {Id} by {UserId}", id, userId);
        
        var command = new ReviewSubmissionCommand(id, true, request?.Reason, userId ?? "System");
        var response = await _mediator.Send(command, ct);
        
        return Ok(new { message = "Submission approved.", data = response });
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] ReviewSubmissionRequest request,
        CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogDebug("Reject submission: {Id} by {UserId}", id, userId);
        
        var command = new ReviewSubmissionCommand(id, false, request?.Reason, userId ?? "System");
        var response = await _mediator.Send(command, ct);
        
        return Ok(new { message = "Submission rejected.", data = response });
    }
}

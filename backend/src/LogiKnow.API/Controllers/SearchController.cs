using LogiKnow.Application.Books.Queries;
using LogiKnow.Application.Common.DTOs;
using LogiKnow.Application.Search.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LogiKnow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SearchController> _logger;

    public SearchController(IMediator mediator, ILogger<SearchController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GlobalSearch(
        [FromQuery] string q, [FromQuery] string? types,
        [FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
    {
        _logger.LogDebug("Global search: q={Query}, types={Types}", q, types);
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { error = "Query parameter 'q' is required." });

        var typeArray = types?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var result = await _mediator.Send(new GlobalSearchQuery(q, typeArray, page, size), ct);
        return Ok(result);
    }

    [HttpGet("quotes")]
    public async Task<IActionResult> SearchQuotes(
        [FromQuery] string q, [FromQuery] Guid? bookId,
        [FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
    {
        _logger.LogDebug("Quote search: q={Query}, bookId={BookId}", q, bookId);
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { error = "Query parameter 'q' is required." });

        var result = await _mediator.Send(new SearchQuotesQuery(q, bookId, page, size), ct);
        return Ok(result);
    }
}

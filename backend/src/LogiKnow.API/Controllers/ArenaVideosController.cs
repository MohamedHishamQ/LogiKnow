using LogiKnow.Application.ArenaVideos.Commands;
using LogiKnow.Application.ArenaVideos.Queries;
using LogiKnow.Application.Common.DTOs;
using LogiKnow.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiKnow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArenaVideosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IArenaVideoRepository _repo;
    private readonly ILogger<ArenaVideosController> _logger;

    public ArenaVideosController(IMediator mediator, IArenaVideoRepository repo, ILogger<ArenaVideosController> logger)
    {
        _mediator = mediator;
        _repo = repo;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
    {
        _logger.LogDebug("GetAll arena videos: page={Page}, size={Size}", page, size);
        var result = await _mediator.Send(new GetArenaVideosQuery(page, size), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        _logger.LogDebug("GetById arena video: {Id}", id);
        var video = await _repo.GetByIdAsync(id, ct);
        if (video is null) return NotFound();

        return Ok(new SingleResponse<ArenaVideoDto>
        {
            Data = new ArenaVideoDto
            {
                Id = video.Id,
                Title = video.Title,
                Url = video.Url,
                Author = video.Author,
                Description = video.Description,
                ThumbnailUrl = video.ThumbnailUrl,
                Platform = video.Platform.ToString(),
                Views = video.Views,
                IsPublished = video.IsPublished,
                CreatedAt = video.CreatedAt,
                UpdatedAt = video.UpdatedAt
            }
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Create([FromBody] CreateArenaVideoRequest request, CancellationToken ct = default)
    {
        _logger.LogDebug("Create arena video: {Title}", request.Title);
        var result = await _mediator.Send(new CreateArenaVideoCommand(request), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new SingleResponse<ArenaVideoDto> { Data = result });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        _logger.LogDebug("Delete arena video: {Id}", id);
        await _repo.DeleteAsync(id, ct);
        return NoContent();
    }
}

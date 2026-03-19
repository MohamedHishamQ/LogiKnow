using LogiKnow.Application.Common.DTOs;
using LogiKnow.Domain.Entities;
using LogiKnow.Domain.Enums;
using LogiKnow.Domain.Interfaces;
using MediatR;

namespace LogiKnow.Application.ArenaVideos.Commands;

public record CreateArenaVideoCommand(CreateArenaVideoRequest Data) : IRequest<ArenaVideoDto>;

public class CreateArenaVideoHandler : IRequestHandler<CreateArenaVideoCommand, ArenaVideoDto>
{
    private readonly IArenaVideoRepository _repo;

    public CreateArenaVideoHandler(IArenaVideoRepository repo)
    {
        _repo = repo;
    }

    public async Task<ArenaVideoDto> Handle(CreateArenaVideoCommand request, CancellationToken ct)
    {
        var video = new ArenaVideo
        {
            Title = request.Data.Title,
            Url = request.Data.Url,
            Author = request.Data.Author,
            Description = request.Data.Description,
            ThumbnailUrl = request.Data.ThumbnailUrl,
            Platform = Enum.TryParse<VideoPlatform>(request.Data.Platform, true, out var p)
                ? p : VideoPlatform.YouTube,
            Views = "0",
            IsPublished = request.Data.IsPublished
        };

        var created = await _repo.CreateAsync(video, ct);

        return new ArenaVideoDto
        {
            Id = created.Id,
            Title = created.Title,
            Url = created.Url,
            Author = created.Author,
            Description = created.Description,
            ThumbnailUrl = created.ThumbnailUrl,
            Platform = created.Platform.ToString(),
            Views = created.Views,
            IsPublished = created.IsPublished,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };
    }
}

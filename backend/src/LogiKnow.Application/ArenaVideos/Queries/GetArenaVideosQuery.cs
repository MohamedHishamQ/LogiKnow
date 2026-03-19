using LogiKnow.Application.Common.DTOs;
using LogiKnow.Domain.Interfaces;
using MediatR;

namespace LogiKnow.Application.ArenaVideos.Queries;

public record GetArenaVideosQuery(int Page = 1, int Size = 20)
    : IRequest<PaginatedResponse<ArenaVideoDto>>;

public class GetArenaVideosHandler : IRequestHandler<GetArenaVideosQuery, PaginatedResponse<ArenaVideoDto>>
{
    private readonly IArenaVideoRepository _repo;

    public GetArenaVideosHandler(IArenaVideoRepository repo)
    {
        _repo = repo;
    }

    public async Task<PaginatedResponse<ArenaVideoDto>> Handle(GetArenaVideosQuery request, CancellationToken ct)
    {
        var size = Math.Clamp(request.Size, 1, 100);
        var (items, total) = await _repo.GetAllAsync(request.Page, size, ct);

        var dtos = items.Select(v => new ArenaVideoDto
        {
            Id = v.Id,
            Title = v.Title,
            Url = v.Url,
            Author = v.Author,
            Description = v.Description,
            ThumbnailUrl = v.ThumbnailUrl,
            Platform = v.Platform.ToString(),
            Views = v.Views,
            IsPublished = v.IsPublished,
            CreatedAt = v.CreatedAt,
            UpdatedAt = v.UpdatedAt
        }).ToList();

        return new PaginatedResponse<ArenaVideoDto>
        {
            Data = dtos,
            Meta = new PaginationMeta { Page = request.Page, Size = size, Total = total }
        };
    }
}

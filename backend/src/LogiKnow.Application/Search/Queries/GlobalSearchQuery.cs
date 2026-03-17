using AutoMapper;
using LogiKnow.Application.Common.DTOs;
using LogiKnow.Domain.Interfaces;
using MediatR;

namespace LogiKnow.Application.Search.Queries;

public record GlobalSearchQuery(string Query, string[]? Types, int Page = 1, int Size = 20)
    : IRequest<PaginatedResponse<SearchResultDto>>;

public class GlobalSearchHandler : IRequestHandler<GlobalSearchQuery, PaginatedResponse<SearchResultDto>>
{
    private readonly ISearchService _searchService;
    private readonly IMapper _mapper;

    public GlobalSearchHandler(ISearchService searchService, IMapper mapper)
    {
        _searchService = searchService;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<SearchResultDto>> Handle(GlobalSearchQuery request, CancellationToken ct)
    {
        var size = Math.Clamp(request.Size, 1, 100);
        var (items, total) = await _searchService.GlobalSearchAsync(request.Query, request.Types, request.Page, size, ct);
        return new PaginatedResponse<SearchResultDto>
        {
            Data = _mapper.Map<IReadOnlyList<SearchResultDto>>(items),
            Meta = new PaginationMeta { Page = request.Page, Size = size, Total = total }
        };
    }
}

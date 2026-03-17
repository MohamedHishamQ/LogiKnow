using AutoMapper;
using LogiKnow.Application.Common.DTOs;
using LogiKnow.Domain.Interfaces;
using MediatR;

namespace LogiKnow.Application.Books.Queries;

public record SearchQuotesQuery(string Query, Guid? BookId, int Page = 1, int Size = 20)
    : IRequest<PaginatedResponse<QuoteSearchResultDto>>;

public class SearchQuotesHandler : IRequestHandler<SearchQuotesQuery, PaginatedResponse<QuoteSearchResultDto>>
{
    private readonly ISearchService _searchService;
    private readonly IMapper _mapper;

    public SearchQuotesHandler(ISearchService searchService, IMapper mapper)
    {
        _searchService = searchService;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<QuoteSearchResultDto>> Handle(SearchQuotesQuery request, CancellationToken ct)
    {
        var size = Math.Clamp(request.Size, 1, 100);
        var (items, total) = await _searchService.SearchQuotesAsync(request.Query, request.BookId, request.Page, size, ct);
        return new PaginatedResponse<QuoteSearchResultDto>
        {
            Data = _mapper.Map<IReadOnlyList<QuoteSearchResultDto>>(items),
            Meta = new PaginationMeta { Page = request.Page, Size = size, Total = total }
        };
    }
}

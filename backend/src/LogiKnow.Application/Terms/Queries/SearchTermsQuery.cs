using AutoMapper;
using LogiKnow.Application.Common.DTOs;
using LogiKnow.Domain.Interfaces;
using MediatR;

namespace LogiKnow.Application.Terms.Queries;

public record SearchTermsQuery(
    string? Category, string? Lang, int Page = 1, int Size = 20
) : IRequest<PaginatedResponse<TermDto>>;

public class SearchTermsHandler : IRequestHandler<SearchTermsQuery, PaginatedResponse<TermDto>>
{
    private readonly ITermRepository _repo;
    private readonly IMapper _mapper;

    public SearchTermsHandler(ITermRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<TermDto>> Handle(SearchTermsQuery request, CancellationToken ct)
    {
        var size = Math.Clamp(request.Size, 1, 100);
        var (items, total) = await _repo.GetAllAsync(request.Category, request.Lang, request.Page, size, ct);
        return new PaginatedResponse<TermDto>
        {
            Data = _mapper.Map<IReadOnlyList<TermDto>>(items),
            Meta = new PaginationMeta { Page = request.Page, Size = size, Total = total }
        };
    }
}

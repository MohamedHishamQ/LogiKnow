using AutoMapper;
using LogiKnow.Application.Common.DTOs;
using LogiKnow.Domain.Enums;
using LogiKnow.Domain.Interfaces;
using MediatR;

namespace LogiKnow.Application.Academic.Queries;

public record GetAcademicEntriesQuery(
    AcademicEntryType? Type, string? Tag, int? Year, int Page = 1, int Size = 20
) : IRequest<PaginatedResponse<AcademicEntryDto>>;

public class GetAcademicEntriesHandler : IRequestHandler<GetAcademicEntriesQuery, PaginatedResponse<AcademicEntryDto>>
{
    private readonly IAcademicRepository _repo;
    private readonly IMapper _mapper;

    public GetAcademicEntriesHandler(IAcademicRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<AcademicEntryDto>> Handle(GetAcademicEntriesQuery request, CancellationToken ct)
    {
        var size = Math.Clamp(request.Size, 1, 100);
        var (items, total) = await _repo.GetAllAsync(request.Type, request.Tag, request.Year, request.Page, size, ct);
        return new PaginatedResponse<AcademicEntryDto>
        {
            Data = _mapper.Map<IReadOnlyList<AcademicEntryDto>>(items),
            Meta = new PaginationMeta { Page = request.Page, Size = size, Total = total }
        };
    }
}

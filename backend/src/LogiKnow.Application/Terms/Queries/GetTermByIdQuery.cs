using AutoMapper;
using LogiKnow.Application.Common.DTOs;
using LogiKnow.Domain.Interfaces;
using MediatR;

namespace LogiKnow.Application.Terms.Queries;

// ===== GetTermByIdQuery =====
public record GetTermByIdQuery(Guid Id) : IRequest<TermDto>;

public class GetTermByIdHandler : IRequestHandler<GetTermByIdQuery, TermDto>
{
    private readonly ITermRepository _repo;
    private readonly IMapper _mapper;

    public GetTermByIdHandler(ITermRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<TermDto> Handle(GetTermByIdQuery request, CancellationToken ct)
    {
        var term = await _repo.GetByIdAsync(request.Id, ct);
        if (term is null)
            throw new KeyNotFoundException($"Term with ID {request.Id} not found.");
        return _mapper.Map<TermDto>(term);
    }
}

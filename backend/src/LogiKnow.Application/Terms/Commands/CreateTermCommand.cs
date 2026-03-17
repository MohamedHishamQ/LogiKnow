using AutoMapper;
using LogiKnow.Application.Common.DTOs;
using LogiKnow.Domain.Entities;
using LogiKnow.Domain.Interfaces;
using MediatR;

namespace LogiKnow.Application.Terms.Commands;

public record CreateTermCommand(CreateTermRequest Data) : IRequest<TermDto>;

public class CreateTermHandler : IRequestHandler<CreateTermCommand, TermDto>
{
    private readonly ITermRepository _repo;
    private readonly ISearchService _searchService;
    private readonly IMapper _mapper;

    public CreateTermHandler(ITermRepository repo, ISearchService searchService, IMapper mapper)
    {
        _repo = repo;
        _searchService = searchService;
        _mapper = mapper;
    }

    public async Task<TermDto> Handle(CreateTermCommand request, CancellationToken ct)
    {
        var term = _mapper.Map<Term>(request.Data);
        var created = await _repo.CreateAsync(term, ct);

        if (created.IsPublished)
            await _searchService.IndexTermAsync(created.Id, ct);

        return _mapper.Map<TermDto>(created);
    }
}

using LogiKnow.Application.Common.DTOs;
using LogiKnow.Domain.Interfaces;
using MediatR;

namespace LogiKnow.Application.Terms.Queries;

public record ExplainTermQuery(Guid Id, string Lang = "ar", string Style = "formal")
    : IRequest<ExplanationResponse>;

public class ExplainTermHandler : IRequestHandler<ExplainTermQuery, ExplanationResponse>
{
    private readonly ITermRepository _repo;
    private readonly IAIService _aiService;

    public ExplainTermHandler(ITermRepository repo, IAIService aiService)
    {
        _repo = repo;
        _aiService = aiService;
    }

    public async Task<ExplanationResponse> Handle(ExplainTermQuery request, CancellationToken ct)
    {
        var term = await _repo.GetByIdAsync(request.Id, ct);
        if (term is null)
            throw new KeyNotFoundException($"Term with ID {request.Id} not found.");

        var explanation = await _aiService.GenerateExplanationAsync(
            term.NameEn, term.NameAr, term.Category,
            term.DefinitionEn, request.Lang, request.Style, ct);

        return new ExplanationResponse
        {
            Explanation = explanation,
            Language = request.Lang,
            Style = request.Style
        };
    }
}

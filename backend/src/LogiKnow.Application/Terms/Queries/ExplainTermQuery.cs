using LogiKnow.Application.Common.DTOs;
using LogiKnow.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace LogiKnow.Application.Terms.Queries;

public record ExplainTermQuery(Guid Id, string Lang = "ar", string Style = "formal")
    : IRequest<ExplanationResponse>;

public class ExplainTermHandler : IRequestHandler<ExplainTermQuery, ExplanationResponse>
{
    private readonly ITermRepository _repo;
    private readonly IAIService _aiService;
    private readonly IMemoryCache _cache;

    public ExplainTermHandler(ITermRepository repo, IAIService aiService, IMemoryCache cache)
    {
        _repo = repo;
        _aiService = aiService;
        _cache = cache;
    }

    public async Task<ExplanationResponse> Handle(ExplainTermQuery request, CancellationToken ct)
    {
        var term = await _repo.GetByIdAsync(request.Id, ct);
        if (term is null)
            throw new KeyNotFoundException($"Term with ID {request.Id} not found.");

        var cacheKey = $"explanation_{request.Id}_{request.Lang}_{request.Style}";

        if (!_cache.TryGetValue(cacheKey, out string? explanation))
        {
            explanation = await _aiService.GenerateExplanationAsync(
                term.NameEn, term.NameAr, term.Category,
                term.DefinitionEn, request.Lang, request.Style, ct);

            _cache.Set(cacheKey, explanation, TimeSpan.FromHours(24));
        }

        return new ExplanationResponse
        {
            Explanation = explanation ?? string.Empty,
            Language = request.Lang,
            Style = request.Style
        };
    }
}

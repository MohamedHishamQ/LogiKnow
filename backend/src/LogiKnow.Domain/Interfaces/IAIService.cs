namespace LogiKnow.Domain.Interfaces;

public interface IAIService
{
    Task<string> GenerateExplanationAsync(
        string termNameEn, string termNameAr, string category,
        string definitionEn, string lang, string style,
        CancellationToken ct = default);
}

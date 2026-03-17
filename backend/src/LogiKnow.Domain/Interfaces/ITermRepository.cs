using LogiKnow.Domain.Entities;

namespace LogiKnow.Domain.Interfaces;

public interface ITermRepository
{
    Task<Term?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<Term> Items, int Total)> GetAllAsync(
        string? category = null, string? lang = null, int page = 1, int size = 20,
        CancellationToken ct = default);
    Task<Term> CreateAsync(Term term, CancellationToken ct = default);
    Task<Term> UpdateAsync(Term term, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

using LogiKnow.Domain.Entities;

namespace LogiKnow.Domain.Interfaces;

public interface ISubmissionRepository
{
    Task<Submission?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Submission>> GetPendingAsync(int skip, int take, CancellationToken ct = default);
    Task<int> CountPendingAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Submission>> GetByUserAsync(string userId, int skip, int take, CancellationToken ct = default);
    Task<int> CountByUserAsync(string userId, CancellationToken ct = default);
    Task<Submission> CreateAsync(Submission submission, CancellationToken ct = default);
    Task UpdateAsync(Submission submission, CancellationToken ct = default);
}

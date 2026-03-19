using LogiKnow.Domain.Entities;

namespace LogiKnow.Domain.Interfaces;

public interface IArenaVideoRepository
{
    Task<ArenaVideo?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<ArenaVideo> Items, int Total)> GetAllAsync(
        int page = 1, int size = 20, CancellationToken ct = default);
    Task<ArenaVideo> CreateAsync(ArenaVideo video, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

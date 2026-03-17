using LogiKnow.Domain.Entities;
using LogiKnow.Domain.Enums;

namespace LogiKnow.Domain.Interfaces;

public interface IAcademicRepository
{
    Task<AcademicEntry?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<AcademicEntry> Items, int Total)> GetAllAsync(
        AcademicEntryType? type = null, string? tag = null, int? year = null,
        int page = 1, int size = 20, CancellationToken ct = default);
    Task<AcademicEntry> CreateAsync(AcademicEntry entry, CancellationToken ct = default);
    Task<AcademicEntry> UpdateAsync(AcademicEntry entry, CancellationToken ct = default);
}

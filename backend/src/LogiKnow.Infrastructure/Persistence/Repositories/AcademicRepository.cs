using LogiKnow.Domain.Entities;
using LogiKnow.Domain.Enums;
using LogiKnow.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LogiKnow.Infrastructure.Persistence.Repositories;

public class AcademicRepository : IAcademicRepository
{
    private readonly AppDbContext _context;

    public AcademicRepository(AppDbContext context) => _context = context;

    public async Task<AcademicEntry?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.AcademicEntries.Include(e => e.Tags)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<(IReadOnlyList<AcademicEntry> Items, int Total)> GetAllAsync(
        AcademicEntryType? type = null, string? tag = null, int? year = null,
        int page = 1, int size = 20, CancellationToken ct = default)
    {
        var query = _context.AcademicEntries.Include(e => e.Tags)
            .Where(e => e.Status == SubmissionStatus.Approved)
            .AsQueryable();

        if (type.HasValue)
            query = query.Where(e => e.Type == type.Value);
        if (!string.IsNullOrEmpty(tag))
            query = query.Where(e => e.Tags.Any(t => t.Name == tag));
        if (year.HasValue)
            query = query.Where(e => e.Year == year.Value);

        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * size).Take(size)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<AcademicEntry> CreateAsync(AcademicEntry entry, CancellationToken ct = default)
    {
        _context.AcademicEntries.Add(entry);
        await _context.SaveChangesAsync(ct);
        return entry;
    }

    public async Task<AcademicEntry> UpdateAsync(AcademicEntry entry, CancellationToken ct = default)
    {
        _context.AcademicEntries.Update(entry);
        await _context.SaveChangesAsync(ct);
        return entry;
    }
}

using LogiKnow.Domain.Entities;
using LogiKnow.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LogiKnow.Infrastructure.Persistence.Repositories;

public class TermRepository : ITermRepository
{
    private readonly AppDbContext _context;

    public TermRepository(AppDbContext context) => _context = context;

    public async Task<Term?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Terms.Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<(IReadOnlyList<Term> Items, int Total)> GetAllAsync(
        string? category = null, string? lang = null, int page = 1, int size = 20,
        CancellationToken ct = default)
    {
        var query = _context.Terms.Include(t => t.Tags)
            .Where(t => t.IsPublished)
            .AsQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(t => t.Category == category);

        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * size).Take(size)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<Term> CreateAsync(Term term, CancellationToken ct = default)
    {
        _context.Terms.Add(term);
        await _context.SaveChangesAsync(ct);
        return term;
    }

    public async Task<Term> UpdateAsync(Term term, CancellationToken ct = default)
    {
        _context.Terms.Update(term);
        await _context.SaveChangesAsync(ct);
        return term;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var term = await _context.Terms.FindAsync(new object[] { id }, ct);
        if (term is null) throw new KeyNotFoundException($"Term {id} not found.");
        _context.Terms.Remove(term);
        await _context.SaveChangesAsync(ct);
    }
}

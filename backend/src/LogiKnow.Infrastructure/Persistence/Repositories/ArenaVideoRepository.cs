using LogiKnow.Domain.Entities;
using LogiKnow.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LogiKnow.Infrastructure.Persistence.Repositories;

public class ArenaVideoRepository : IArenaVideoRepository
{
    private readonly AppDbContext _context;

    public ArenaVideoRepository(AppDbContext context) => _context = context;

    public async Task<ArenaVideo?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.ArenaVideos.FirstOrDefaultAsync(v => v.Id == id, ct);

    public async Task<(IReadOnlyList<ArenaVideo> Items, int Total)> GetAllAsync(
        int page = 1, int size = 20, CancellationToken ct = default)
    {
        var query = _context.ArenaVideos
            .Where(v => v.IsPublished)
            .AsQueryable();

        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * size).Take(size)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<ArenaVideo> CreateAsync(ArenaVideo video, CancellationToken ct = default)
    {
        _context.ArenaVideos.Add(video);
        await _context.SaveChangesAsync(ct);
        return video;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var video = await _context.ArenaVideos.FindAsync(new object[] { id }, ct);
        if (video is null) throw new KeyNotFoundException($"ArenaVideo {id} not found.");
        _context.ArenaVideos.Remove(video);
        await _context.SaveChangesAsync(ct);
    }
}

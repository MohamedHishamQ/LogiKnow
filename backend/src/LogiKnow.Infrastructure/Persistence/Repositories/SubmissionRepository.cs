using LogiKnow.Domain.Entities;
using LogiKnow.Domain.Interfaces;
using LogiKnow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LogiKnow.Infrastructure.Persistence.Repositories;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly AppDbContext _context;

    public SubmissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Submission?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Submissions.FindAsync(new object[] { id }, ct);
    }

    public async Task<IReadOnlyList<Submission>> GetPendingAsync(int skip, int take, CancellationToken ct = default)
    {
        return await _context.Submissions
            .Where(s => s.Status == Domain.Enums.SubmissionStatus.Pending)
            .OrderByDescending(s => s.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<int> CountPendingAsync(CancellationToken ct = default)
    {
        return await _context.Submissions
            .Where(s => s.Status == Domain.Enums.SubmissionStatus.Pending)
            .CountAsync(ct);
    }

    public async Task<IReadOnlyList<Submission>> GetByUserAsync(string userId, int skip, int take, CancellationToken ct = default)
    {
        return await _context.Submissions
            .Where(s => s.SubmittedBy == userId)
            .OrderByDescending(s => s.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<int> CountByUserAsync(string userId, CancellationToken ct = default)
    {
        return await _context.Submissions
            .Where(s => s.SubmittedBy == userId)
            .CountAsync(ct);
    }

    public async Task<Submission> CreateAsync(Submission submission, CancellationToken ct = default)
    {
        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync(ct);
        return submission;
    }

    public async Task UpdateAsync(Submission submission, CancellationToken ct = default)
    {
        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(ct);
    }
}

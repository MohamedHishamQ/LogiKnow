using LogiKnow.Domain.Interfaces;
using LogiKnow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LogiKnow.Infrastructure.Search;

public class MockSearchService : ISearchService
{
    private readonly AppDbContext _context;

    public MockSearchService(AppDbContext context)
    {
        _context = context;
    }

    public Task IndexTermAsync(Guid termId, CancellationToken ct = default) => Task.CompletedTask;
    public Task IndexBookPagesAsync(Guid bookId, CancellationToken ct = default) => Task.CompletedTask;
    public Task IndexAcademicEntryAsync(Guid entryId, CancellationToken ct = default) => Task.CompletedTask;

    public Task<(IReadOnlyList<SearchResult> Items, int Total)> GlobalSearchAsync(
        string query, string[]? types = null, int page = 1, int size = 20, CancellationToken ct = default)
    {
        var items = new List<SearchResult>
        {
            new SearchResult { Id = Guid.NewGuid(), Title = "Demo Search Result 1", Type = "term", Score = 1.0, Snippet = "This is a mock search result since ES is not running." },
            new SearchResult { Id = Guid.NewGuid(), Title = "Demo Search Result 2", Type = "book", Score = 0.9, Snippet = "Another mock search result." }
        };
        return Task.FromResult(((IReadOnlyList<SearchResult>)items, items.Count));
    }

    public async Task<(IReadOnlyList<QuoteSearchResult> Items, int Total)> SearchQuotesAsync(
        string query, Guid? bookId = null, int page = 1, int size = 20, CancellationToken ct = default)
    {
        var dbQuery = _context.BookPages
            .Include(p => p.Book)
            .Where(p => p.Content.Contains(query));

        if (bookId.HasValue)
            dbQuery = dbQuery.Where(p => p.BookId == bookId.Value);

        var total = await dbQuery.CountAsync(ct);
        var paged = await dbQuery
            .OrderBy(p => p.Book!.Title)
            .ThenBy(p => p.PageNumber)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);

        var results = paged.Select(p => new QuoteSearchResult
        {
            BookId = p.BookId,
            BookTitle = p.Book?.Title ?? "Unknown Book",
            PageNumber = p.PageNumber,
            Highlight = p.Content.Length > 200 ? p.Content.Substring(0, 200) + "..." : p.Content,
            SurroundingContext = p.Content.Length > 500 ? p.Content.Substring(0, 500) + "..." : p.Content
        }).ToList();

        return (results, total);
    }

    public Task EnsureIndicesCreatedAsync(CancellationToken ct = default) => Task.CompletedTask;
}

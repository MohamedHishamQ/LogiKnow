using LogiKnow.Domain.Interfaces;

namespace LogiKnow.Infrastructure.Search;

public class MockSearchService : ISearchService
{
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

    public Task<(IReadOnlyList<QuoteSearchResult> Items, int Total)> SearchQuotesAsync(
        string query, Guid? bookId = null, int page = 1, int size = 20, CancellationToken ct = default)
    {
        var items = new List<QuoteSearchResult>();
        return Task.FromResult(((IReadOnlyList<QuoteSearchResult>)items, 0));
    }

    public Task EnsureIndicesCreatedAsync(CancellationToken ct = default) => Task.CompletedTask;
}

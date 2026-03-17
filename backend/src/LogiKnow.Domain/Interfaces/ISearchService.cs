namespace LogiKnow.Domain.Interfaces;

public class SearchResult
{
    public string Type { get; set; } = string.Empty;  // "term", "book", "academic", "quote"
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Snippet { get; set; }
    public double Score { get; set; }
    public Dictionary<string, object?> Metadata { get; set; } = new();
}

public class QuoteSearchResult
{
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public int PageNumber { get; set; }
    public string Highlight { get; set; } = string.Empty;
    public string SurroundingContext { get; set; } = string.Empty;
}

public interface ISearchService
{
    Task IndexTermAsync(Guid termId, CancellationToken ct = default);
    Task IndexBookPagesAsync(Guid bookId, CancellationToken ct = default);
    Task IndexAcademicEntryAsync(Guid entryId, CancellationToken ct = default);
    Task<(IReadOnlyList<SearchResult> Items, int Total)> GlobalSearchAsync(
        string query, string[]? types = null, int page = 1, int size = 20,
        CancellationToken ct = default);
    Task<(IReadOnlyList<QuoteSearchResult> Items, int Total)> SearchQuotesAsync(
        string query, Guid? bookId = null, int page = 1, int size = 20,
        CancellationToken ct = default);
    Task EnsureIndicesCreatedAsync(CancellationToken ct = default);
}

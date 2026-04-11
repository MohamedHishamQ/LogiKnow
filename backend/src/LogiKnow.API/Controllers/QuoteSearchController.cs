using LogiKnow.Infrastructure.Persistence;
using LogiKnow.Infrastructure.Search.IndexModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace LogiKnow.API.Controllers;

[ApiController]
[Route("api/quotesearch")]
public class QuoteSearchController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<QuoteSearchController> _logger;

    private const string BOOKPAGES_INDEX = "logiknow-bookpages";

    public QuoteSearchController(AppDbContext db, IElasticClient elasticClient, ILogger<QuoteSearchController> logger)
    {
        _db = db;
        _elasticClient = elasticClient;
        _logger = logger;
    }

    /// <summary>
    /// Search inside book pages for quotes / phrases using production Elasticsearch.
    /// This implementation is isolated from global services to ensure reliability.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] Guid? bookId,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { error = "Query parameter 'q' is required." });

        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, 50);

        _logger.LogDebug("QuoteSearch ES: q={Query}, bookId={BookId}, page={Page}, size={Size}", q, bookId, page, size);

        try 
        {
            // Query Elasticsearch directly (isolated production path)
            var searchResponse = await _elasticClient.SearchAsync<BookPageIndexDocument>(s => s
                .Index(BOOKPAGES_INDEX)
                .Query(query => query
                    .Bool(b => 
                    {
                        var mustQueries = new List<Func<QueryContainerDescriptor<BookPageIndexDocument>, QueryContainer>>();
                        
                        // Phrase match for the search string
                        mustQueries.Add(m => m.MatchPhrase(mp => mp.Field(f => f.Content).Query(q)));
                        
                        if (bookId.HasValue)
                        {
                            mustQueries.Add(m => m.Term(t => t.Field(f => f.BookId).Value(bookId.Value.ToString())));
                        }

                        return b.Must(mustQueries);
                    })
                )
                .Highlight(h => h
                    .Fields(f => f
                        .Field(ff => ff.Content)
                        .PreTags("<mark>")
                        .PostTags("</mark>")
                        .FragmentSize(300)
                        .NumberOfFragments(1)
                    )
                )
                .From((page - 1) * size)
                .Size(size), ct);

            if (!searchResponse.IsValid)
            {
                _logger.LogWarning("ES Search Error: {Msg}", searchResponse.ServerError?.Error?.Reason);
                return await SearchDatabaseFallback(q, bookId, page, size, ct);
            }

            var results = searchResponse.Hits.Select(h => new QuoteSearchItem
            {
                BookId = Guid.Parse(h.Source.BookId),
                BookTitle = h.Source.BookTitle,
                // PageNumber from source or metadata if available
                PageNumber = h.Source.PageNumber,
                Snippet = h.Highlight.ContainsKey("content") ? h.Highlight["content"].FirstOrDefault() ?? "" : h.Source.Content.Length > 300 ? h.Source.Content[..300] : h.Source.Content,
                Highlight = h.Highlight.ContainsKey("content") ? h.Highlight["content"].FirstOrDefault() ?? "" : h.Source.Content.Length > 300 ? h.Source.Content[..300] : h.Source.Content
            }).ToList();

            // Fetch missing metadata (authors/category) from DB for the displayed results
            var bookIds = results.Select(r => r.BookId).Distinct().ToList();
            var booksMetadata = await _db.Books.Where(b => bookIds.Contains(b.Id)).ToDictionaryAsync(b => b.Id, ct);

            foreach (var res in results)
            {
                if (booksMetadata.TryGetValue(res.BookId, out var b))
                {
                    res.BookAuthors = b.Authors;
                    res.BookCategory = b.Category;
                    res.BookCoverUrl = b.CoverUrl;
                }
            }

            return Ok(new
            {
                data = results,
                meta = new { page, size, total = (int)searchResponse.Total }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ES Search failed, falling back to database");
            return await SearchDatabaseFallback(q, bookId, page, size, ct);
        }
    }

    /// <summary>
    /// Fallback to direct SQL query if Elasticsearch is unavailable or not indexed yet.
    /// </summary>
    private async Task<IActionResult> SearchDatabaseFallback(string q, Guid? bookId, int page, int size, CancellationToken ct)
    {
        var words = q.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var query = _db.BookPages.Include(p => p.Book).Where(p => p.Book.IsPublished).AsQueryable();

        foreach (var word in words)
        {
            var w = word;
            query = query.Where(p => p.Content.Contains(w));
        }

        if (bookId.HasValue)
            query = query.Where(p => p.BookId == bookId.Value);

        var total = await query.CountAsync(ct);
        var dbResults = await query
            .OrderBy(p => p.Book.Title)
            .ThenBy(p => p.PageNumber)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(p => new
            {
                p.BookId,
                BookTitle = p.Book.Title,
                BookAuthors = p.Book.Authors,
                BookCategory = p.Book.Category,
                BookCoverUrl = p.Book.CoverUrl,
                p.PageNumber,
                Content = p.Content
            })
            .ToListAsync(ct);

        var data = dbResults.Select(r => new QuoteSearchItem
        {
            BookId = r.BookId,
            BookTitle = r.BookTitle,
            BookAuthors = r.BookAuthors,
            BookCategory = r.BookCategory,
            BookCoverUrl = r.BookCoverUrl,
            PageNumber = r.PageNumber,
            Snippet = BuildSnippet(r.Content, words, 300),
            Highlight = HighlightWords(BuildSnippet(r.Content, words, 300), words)
        }).ToList();

        return Ok(new { data, meta = new { page, size, total } });
    }

    /// <summary>
    /// Explicitly re-index a book's pages into Elasticsearch for Deep Search.
    /// Use this if the Admin 'Index' button doesn't work due to service locks.
    /// </summary>
    [HttpPost("reindex/{id:guid}")]
    public async Task<IActionResult> ReindexBook(Guid id, CancellationToken ct = default)
    {
        var book = await _db.Books.Include(b => b.Pages).FirstOrDefaultAsync(b => b.Id == id, ct);
        if (book == null) return NotFound("Book not found.");
        if (!book.Pages.Any()) return BadRequest("Book has no pages in the database. Please upload the PDF first.");

        var docs = book.Pages.Select(p => new BookPageIndexDocument
        {
            Id = p.Id.ToString(),
            BookId = book.Id.ToString(),
            BookTitle = book.Title,
            PageNumber = p.PageNumber,
            Content = p.Content
        });

        var bulkResponse = await _elasticClient.BulkAsync(b => b.Index(BOOKPAGES_INDEX).IndexMany(docs), ct);

        if (!bulkResponse.IsValid)
            return StatusCode(500, new { error = "Indexing failed", details = bulkResponse.ServerError?.Error?.Reason });

        return Ok(new { message = $"Successfully indexed {book.Pages.Count} pages for '{book.Title}'." });
    }

    [HttpGet("books")]
    public async Task<IActionResult> GetBooksForFilter(CancellationToken ct = default)
    {
        var books = await _db.Books
            .Where(b => b.IsPublished)
            .OrderBy(b => b.Title)
            .Select(b => new
            {
                b.Id,
                b.Title,
                b.Category,
                b.Language,
                HasPages = b.Pages.Any()
            })
            .ToListAsync(ct);

        return Ok(new { data = books });
    }

    private static string BuildSnippet(string content, string[] words, int maxLength)
    {
        if (string.IsNullOrEmpty(content)) return string.Empty;
        int earliest = content.Length;
        foreach (var w in words) {
            var idx = content.IndexOf(w, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0 && idx < earliest) earliest = idx;
        }
        if (earliest == content.Length) earliest = 0;
        int start = Math.Max(0, earliest - maxLength / 4);
        int end = Math.Min(content.Length, start + maxLength);
        var snippet = content[start..end].Trim();
        if (start > 0) snippet = "…" + snippet;
        if (end < content.Length) snippet += "…";
        return snippet;
    }

    private static string HighlightWords(string text, string[] words)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        var result = text;
        foreach (var word in words) {
            var idx = 0;
            while (idx < result.Length) {
                var pos = result.IndexOf(word, idx, StringComparison.OrdinalIgnoreCase);
                if (pos < 0) break;
                var matched = result.Substring(pos, word.Length);
                var replacement = $"<mark>{matched}</mark>";
                result = result[..pos] + replacement + result[(pos + word.Length)..];
                idx = pos + replacement.Length;
            }
        }
        return result;
    }

    private class QuoteSearchItem
    {
        public Guid BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string? BookAuthors { get; set; }
        public string? BookCategory { get; set; }
        public string? BookCoverUrl { get; set; }
        public int PageNumber { get; set; }
        public string Snippet { get; set; } = string.Empty;
        public string Highlight { get; set; } = string.Empty;
    }
}

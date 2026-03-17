using LogiKnow.Domain.Entities;
using LogiKnow.Domain.Interfaces;
using LogiKnow.Infrastructure.Persistence;
using LogiKnow.Infrastructure.Search.IndexModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;

namespace LogiKnow.Infrastructure.Search;

public class ElasticsearchService : ISearchService
{
    private readonly IElasticClient _client;
    private readonly AppDbContext _context;
    private readonly ILogger<ElasticsearchService> _logger;

    private const string TERMS_INDEX = "logiknow-terms";
    private const string BOOKPAGES_INDEX = "logiknow-bookpages";
    private const string ACADEMIC_INDEX = "logiknow-academic";

    public ElasticsearchService(IElasticClient client, AppDbContext context, ILogger<ElasticsearchService> logger)
    {
        _client = client;
        _context = context;
        _logger = logger;
    }

    public async Task EnsureIndicesCreatedAsync(CancellationToken ct = default)
    {
        // Terms index
        if (!(await _client.Indices.ExistsAsync(TERMS_INDEX, ct: ct)).Exists)
        {
            await _client.Indices.CreateAsync(TERMS_INDEX, c => c
                .Map<TermIndexDocument>(m => m
                    .Properties(p => p
                        .Keyword(k => k.Name(n => n.Id))
                        .Text(t => t.Name(n => n.NameEn).Analyzer("english"))
                        .Text(t => t.Name(n => n.NameAr).Analyzer("arabic"))
                        .Text(t => t.Name(n => n.NameFr).Analyzer("french"))
                        .Keyword(k => k.Name(n => n.Category))
                        .Text(t => t.Name(n => n.DefinitionEn).Analyzer("english"))
                        .Text(t => t.Name(n => n.DefinitionAr).Analyzer("arabic"))
                        .Keyword(k => k.Name(n => n.Tags))
                        .Boolean(b => b.Name(n => n.IsPublished))
                    )
                ), ct);
            _logger.LogInformation("Created Elasticsearch index: {Index}", TERMS_INDEX);
        }

        // Book pages index
        if (!(await _client.Indices.ExistsAsync(BOOKPAGES_INDEX, ct: ct)).Exists)
        {
            await _client.Indices.CreateAsync(BOOKPAGES_INDEX, c => c
                .Map<BookPageIndexDocument>(m => m
                    .Properties(p => p
                        .Keyword(k => k.Name(n => n.Id))
                        .Keyword(k => k.Name(n => n.BookId))
                        .Text(t => t.Name(n => n.BookTitle))
                        .Number(n => n.Name(nn => nn.PageNumber).Type(NumberType.Integer))
                        .Text(t => t.Name(n => n.Content))
                    )
                ), ct);
            _logger.LogInformation("Created Elasticsearch index: {Index}", BOOKPAGES_INDEX);
        }

        // Academic index
        if (!(await _client.Indices.ExistsAsync(ACADEMIC_INDEX, ct: ct)).Exists)
        {
            await _client.Indices.CreateAsync(ACADEMIC_INDEX, c => c
                .Map<AcademicIndexDocument>(m => m
                    .Properties(p => p
                        .Keyword(k => k.Name(n => n.Id))
                        .Text(t => t.Name(n => n.Title))
                        .Text(t => t.Name(n => n.Author))
                        .Text(t => t.Name(n => n.University))
                        .Text(t => t.Name(n => n.Abstract))
                        .Number(n => n.Name(nn => nn.Year).Type(NumberType.Integer))
                        .Keyword(k => k.Name(n => n.Type))
                        .Keyword(k => k.Name(n => n.Tags))
                        .Keyword(k => k.Name(n => n.Status))
                    )
                ), ct);
            _logger.LogInformation("Created Elasticsearch index: {Index}", ACADEMIC_INDEX);
        }
    }

    public async Task IndexTermAsync(Guid termId, CancellationToken ct = default)
    {
        var term = await _context.Terms.Include(t => t.Tags).FirstOrDefaultAsync(t => t.Id == termId, ct);
        if (term is null || !term.IsPublished) return;

        var doc = new TermIndexDocument
        {
            Id = term.Id.ToString(),
            NameEn = term.NameEn,
            NameAr = term.NameAr,
            NameFr = term.NameFr,
            Category = term.Category,
            DefinitionEn = term.DefinitionEn,
            DefinitionAr = term.DefinitionAr,
            Tags = term.Tags.Select(t => t.Name).ToArray(),
            IsPublished = term.IsPublished
        };

        await _client.IndexAsync(doc, i => i.Index(TERMS_INDEX).Id(doc.Id), ct);
        _logger.LogDebug("Indexed term {TermId} in Elasticsearch", termId);
    }

    public async Task IndexBookPagesAsync(Guid bookId, CancellationToken ct = default)
    {
        var book = await _context.Books.Include(b => b.Pages)
            .FirstOrDefaultAsync(b => b.Id == bookId, ct);
        if (book is null || !book.IsIndexedForSearch) return;

        var docs = book.Pages.Select(p => new BookPageIndexDocument
        {
            Id = p.Id.ToString(),
            BookId = book.Id.ToString(),
            BookTitle = book.Title,
            PageNumber = p.PageNumber,
            Content = p.Content
        });

        var bulkResponse = await _client.BulkAsync(b => b.Index(BOOKPAGES_INDEX).IndexMany(docs), ct);
        _logger.LogInformation("Indexed {Count} pages for book {BookId}", book.Pages.Count, bookId);
    }

    public async Task IndexAcademicEntryAsync(Guid entryId, CancellationToken ct = default)
    {
        var entry = await _context.AcademicEntries.Include(e => e.Tags)
            .FirstOrDefaultAsync(e => e.Id == entryId, ct);
        if (entry is null || entry.Status != Domain.Enums.SubmissionStatus.Approved) return;

        var doc = new AcademicIndexDocument
        {
            Id = entry.Id.ToString(),
            Title = entry.Title,
            Author = entry.Author,
            University = entry.University,
            Abstract = entry.Abstract,
            Year = entry.Year,
            Type = entry.Type.ToString(),
            Tags = entry.Tags.Select(t => t.Name).ToArray(),
            Status = entry.Status.ToString()
        };

        await _client.IndexAsync(doc, i => i.Index(ACADEMIC_INDEX).Id(doc.Id), ct);
        _logger.LogDebug("Indexed academic entry {EntryId} in Elasticsearch", entryId);
    }

    public async Task<(IReadOnlyList<SearchResult> Items, int Total)> GlobalSearchAsync(
        string query, string[]? types = null, int page = 1, int size = 20,
        CancellationToken ct = default)
    {
        var results = new List<SearchResult>();
        var total = 0;
        var searchTypes = types ?? new[] { "terms", "books", "academic", "quotes" };

        if (searchTypes.Contains("terms"))
        {
            var termResults = await _client.SearchAsync<TermIndexDocument>(s => s
                .Index(TERMS_INDEX)
                .Query(q => q.MultiMatch(mm => mm
                    .Fields(f => f.Field(ff => ff.NameEn).Field(ff => ff.NameAr).Field(ff => ff.DefinitionEn).Field(ff => ff.DefinitionAr))
                    .Query(query)))
                .From((page - 1) * size).Size(size), ct);

            results.AddRange(termResults.Hits.Select(h => new SearchResult
            {
                Type = "term",
                Id = Guid.Parse(h.Source.Id),
                Title = h.Source.NameEn,
                Snippet = h.Source.DefinitionEn,
                Score = h.Score ?? 0
            }));
            total += (int)(termResults.Total);
        }

        if (searchTypes.Contains("academic"))
        {
            var acadResults = await _client.SearchAsync<AcademicIndexDocument>(s => s
                .Index(ACADEMIC_INDEX)
                .Query(q => q.MultiMatch(mm => mm
                    .Fields(f => f.Field(ff => ff.Title).Field(ff => ff.Author).Field(ff => ff.Abstract))
                    .Query(query)))
                .From((page - 1) * size).Size(size), ct);

            results.AddRange(acadResults.Hits.Select(h => new SearchResult
            {
                Type = "academic",
                Id = Guid.Parse(h.Source.Id),
                Title = h.Source.Title,
                Snippet = h.Source.Abstract.Length > 200 ? h.Source.Abstract[..200] + "..." : h.Source.Abstract,
                Score = h.Score ?? 0
            }));
            total += (int)(acadResults.Total);
        }

        if (searchTypes.Contains("quotes"))
        {
            var quoteResults = await _client.SearchAsync<BookPageIndexDocument>(s => s
                .Index(BOOKPAGES_INDEX)
                .Query(q => q.MatchPhrase(mp => mp.Field(f => f.Content).Query(query)))
                .Highlight(h => h.Fields(f => f.Field(ff => ff.Content)))
                .From((page - 1) * size).Size(size), ct);

            results.AddRange(quoteResults.Hits.Select(h => new SearchResult
            {
                Type = "quote",
                Id = Guid.Parse(h.Source.Id),
                Title = h.Source.BookTitle,
                Snippet = h.Highlight.ContainsKey("content") ? string.Join(" ", h.Highlight["content"]) : "",
                Score = h.Score ?? 0,
                Metadata = new Dictionary<string, object?>
                {
                    ["bookId"] = h.Source.BookId,
                    ["pageNumber"] = h.Source.PageNumber
                }
            }));
            total += (int)(quoteResults.Total);
        }

        return (results.OrderByDescending(r => r.Score).Take(size).ToList(), total);
    }

    public async Task<(IReadOnlyList<QuoteSearchResult> Items, int Total)> SearchQuotesAsync(
        string query, Guid? bookId = null, int page = 1, int size = 20,
        CancellationToken ct = default)
    {
        var searchResponse = await _client.SearchAsync<BookPageIndexDocument>(s =>
        {
            s.Index(BOOKPAGES_INDEX)
                .Query(q =>
                {
                    var must = q.MatchPhrase(mp => mp.Field(f => f.Content).Query(query));
                    if (bookId.HasValue)
                        return q.Bool(b => b
                            .Must(must)
                            .Filter(f => f.Term(t => t.Field(ff => ff.BookId).Value(bookId.Value.ToString()))));
                    return must;
                })
                .Highlight(h => h.Fields(f => f.Field(ff => ff.Content).PreTags("<em>").PostTags("</em>")))
                .From((page - 1) * size).Size(size);
            return s;
        }, ct);

        var results = searchResponse.Hits.Select(h => new QuoteSearchResult
        {
            BookId = Guid.Parse(h.Source.BookId),
            BookTitle = h.Source.BookTitle,
            PageNumber = h.Source.PageNumber,
            Highlight = h.Highlight.ContainsKey("content") ? string.Join(" ", h.Highlight["content"]) : "",
            SurroundingContext = h.Source.Content.Length > 300 ? h.Source.Content[..300] : h.Source.Content
        }).ToList();

        return (results, (int)searchResponse.Total);
    }
}

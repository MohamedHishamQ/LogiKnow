namespace LogiKnow.Infrastructure.Search.IndexModels;

public class TermIndexDocument
{
    public string Id { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? NameFr { get; set; }
    public string Category { get; set; } = string.Empty;
    public string DefinitionEn { get; set; } = string.Empty;
    public string DefinitionAr { get; set; } = string.Empty;
    public string[] Tags { get; set; } = Array.Empty<string>();
    public bool IsPublished { get; set; }
}

public class BookPageIndexDocument
{
    public string Id { get; set; } = string.Empty;
    public string BookId { get; set; } = string.Empty;
    public string BookTitle { get; set; } = string.Empty;
    public int PageNumber { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class AcademicIndexDocument
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string University { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Type { get; set; } = string.Empty;
    public string[] Tags { get; set; } = Array.Empty<string>();
    public string Status { get; set; } = string.Empty;
}

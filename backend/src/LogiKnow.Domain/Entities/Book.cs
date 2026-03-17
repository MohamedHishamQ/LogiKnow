namespace LogiKnow.Domain.Entities;

public class Book : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Authors { get; set; } = string.Empty; // JSON array stored as string
    public int? Year { get; set; }
    public string? ISBN { get; set; }
    public string Language { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? CoverUrl { get; set; }
    public string? ExternalLink { get; set; }
    public string? BlobStoragePath { get; set; }
    public bool IsIndexedForSearch { get; set; } = false;
    public bool IsPublished { get; set; } = false;
    public ICollection<BookPage> Pages { get; set; } = new List<BookPage>();
}

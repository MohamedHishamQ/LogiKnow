namespace LogiKnow.Domain.Entities;

public class Term : BaseEntity
{
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? NameFr { get; set; }
    public string Category { get; set; } = string.Empty;
    public string DefinitionEn { get; set; } = string.Empty;
    public string DefinitionAr { get; set; } = string.Empty;
    public string? ExampleEn { get; set; }
    public string? ExampleAr { get; set; }
    public bool IsPublished { get; set; }
    public string? SubmittedBy { get; set; }
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}

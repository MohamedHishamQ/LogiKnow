using LogiKnow.Domain.Enums;

namespace LogiKnow.Domain.Entities;

public class AcademicEntry : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string University { get; set; } = string.Empty;
    public int Year { get; set; }
    public string? Supervisor { get; set; }
    public string Abstract { get; set; } = string.Empty;
    public string? DocumentUrl { get; set; }
    public AcademicEntryType Type { get; set; }
    public SubmissionStatus Status { get; set; }
    public string? SubmittedBy { get; set; }
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}

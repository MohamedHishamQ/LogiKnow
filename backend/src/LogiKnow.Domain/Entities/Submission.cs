using LogiKnow.Domain.Enums;

namespace LogiKnow.Domain.Entities;

public class Submission : BaseEntity
{
    public string EntityType { get; set; } = string.Empty; // "Term", "Book", "AcademicEntry"
    public string JsonData { get; set; } = string.Empty;   // Serialized submission data
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Pending;
    public string? ReviewNotes { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
}

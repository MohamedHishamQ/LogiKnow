namespace LogiKnow.Domain.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? NameFr { get; set; }
    public ICollection<Term> Terms { get; set; } = new List<Term>();
    public ICollection<AcademicEntry> AcademicEntries { get; set; } = new List<AcademicEntry>();
}

namespace LogiKnow.Domain.Entities;

public class BookPage : BaseEntity
{
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;
    public int PageNumber { get; set; }
    public string Content { get; set; } = string.Empty;
}

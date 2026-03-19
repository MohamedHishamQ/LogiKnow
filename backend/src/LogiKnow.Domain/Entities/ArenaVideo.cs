using LogiKnow.Domain.Enums;

namespace LogiKnow.Domain.Entities;

public class ArenaVideo : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public VideoPlatform Platform { get; set; } = VideoPlatform.YouTube;
    public string Views { get; set; } = "0";
    public bool IsPublished { get; set; } = true;
}

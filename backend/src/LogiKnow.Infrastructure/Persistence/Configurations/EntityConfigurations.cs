using LogiKnow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogiKnow.Infrastructure.Persistence.Configurations;

public class TermConfiguration : IEntityTypeConfiguration<Term>
{
    public void Configure(EntityTypeBuilder<Term> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
        builder.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
        builder.Property(e => e.NameFr).HasMaxLength(200);
        builder.Property(e => e.Category).IsRequired().HasMaxLength(100);
        builder.Property(e => e.DefinitionEn).IsRequired().HasMaxLength(2000);
        builder.Property(e => e.DefinitionAr).IsRequired().HasMaxLength(2000);
        builder.Property(e => e.ExampleEn).HasMaxLength(500);
        builder.Property(e => e.ExampleAr).HasMaxLength(500);
        builder.Property(e => e.SubmittedBy).HasMaxLength(450);

        builder.HasMany(e => e.Tags)
            .WithMany(e => e.Terms)
            .UsingEntity("TermTags");

        builder.HasIndex(e => e.Category);
        builder.HasIndex(e => e.IsPublished);
    }
}

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Authors).IsRequired();
        builder.Property(e => e.ISBN).HasMaxLength(20);
        builder.Property(e => e.Language).IsRequired().HasMaxLength(10);
        builder.Property(e => e.Category).IsRequired().HasMaxLength(100);
        builder.Property(e => e.CoverUrl).HasMaxLength(500);
        builder.Property(e => e.ExternalLink).HasMaxLength(500);
        builder.Property(e => e.BlobStoragePath).HasMaxLength(500);

        builder.HasMany(e => e.Pages)
            .WithOne(e => e.Book)
            .HasForeignKey(e => e.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.Category);
        builder.HasIndex(e => e.Language);
        builder.HasIndex(e => e.IsPublished);
    }
}

public class BookPageConfiguration : IEntityTypeConfiguration<BookPage>
{
    public void Configure(EntityTypeBuilder<BookPage> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Content).IsRequired();
        builder.HasIndex(e => new { e.BookId, e.PageNumber }).IsUnique();
    }
}

public class AcademicEntryConfiguration : IEntityTypeConfiguration<AcademicEntry>
{
    public void Configure(EntityTypeBuilder<AcademicEntry> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Author).IsRequired().HasMaxLength(200);
        builder.Property(e => e.University).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Abstract).IsRequired().HasMaxLength(5000);
        builder.Property(e => e.Supervisor).HasMaxLength(200);
        builder.Property(e => e.DocumentUrl).HasMaxLength(500);
        builder.Property(e => e.SubmittedBy).HasMaxLength(450);

        builder.HasMany(e => e.Tags)
            .WithMany(e => e.AcademicEntries)
            .UsingEntity("AcademicEntryTags");

        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.Type);
        builder.HasIndex(e => e.Year);
    }
}

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EntityType).IsRequired().HasMaxLength(50);
        builder.Property(e => e.JsonData).IsRequired();
        builder.Property(e => e.SubmittedBy).IsRequired().HasMaxLength(450);
        builder.Property(e => e.ReviewedBy).HasMaxLength(450);
        builder.Property(e => e.ReviewNotes).HasMaxLength(1000);

        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.SubmittedBy);
    }
}

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.NameAr).HasMaxLength(100);
        builder.Property(e => e.NameFr).HasMaxLength(100);
        builder.HasIndex(e => e.Name).IsUnique();
    }
}

public class ArenaVideoConfiguration : IEntityTypeConfiguration<ArenaVideo>
{
    public void Configure(EntityTypeBuilder<ArenaVideo> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Url).IsRequired().HasMaxLength(1000);
        builder.Property(e => e.Author).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(2000);
        builder.Property(e => e.ThumbnailUrl).HasMaxLength(1000);
        builder.Property(e => e.Views).HasMaxLength(50);
        builder.HasIndex(e => e.IsPublished);
        builder.HasIndex(e => e.Platform);
    }
}

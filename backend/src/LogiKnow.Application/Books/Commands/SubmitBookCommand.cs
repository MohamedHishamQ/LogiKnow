using AutoMapper;
using LogiKnow.Application.Common.DTOs;
using LogiKnow.Domain.Entities;
using LogiKnow.Domain.Enums;
using LogiKnow.Domain.Interfaces;
using MediatR;
using System.Text.Json;

namespace LogiKnow.Application.Books.Commands;

public record SubmitBookCommand(SubmitBookRequest Data, string SubmittedBy) : IRequest<BookDto>;

public class SubmitBookHandler : IRequestHandler<SubmitBookCommand, BookDto>
{
    private readonly ISubmissionRepository _submissionRepo;

    public SubmitBookHandler(ISubmissionRepository submissionRepo)
    {
        _submissionRepo = submissionRepo;
    }

    public async Task<BookDto> Handle(SubmitBookCommand request, CancellationToken ct)
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = request.Data.Title,
            Authors = JsonSerializer.Serialize(request.Data.Authors),
            Year = request.Data.Year,
            ISBN = request.Data.ISBN,
            Language = request.Data.Language,
            Category = request.Data.Category,
            CoverUrl = request.Data.CoverUrl,
            ExternalLink = request.Data.ExternalLink,
            IsIndexedForSearch = false,
            IsPublished = false
        };

        var submission = new Submission
        {
            EntityType = "Book",
            JsonData = JsonSerializer.Serialize(book),
            Status = SubmissionStatus.Pending,
            SubmittedBy = request.SubmittedBy
        };

        await _submissionRepo.CreateAsync(submission, ct);

        // Return a mock DTO just to show it was accepted
        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Authors = JsonSerializer.Deserialize<List<string>>(book.Authors) ?? new(),
            Year = book.Year,
            ISBN = book.ISBN,
            Language = book.Language,
            Category = book.Category,
            IsPublished = false
        };
    }
}

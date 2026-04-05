using LogiKnow.Application.Common.DTOs;
using LogiKnow.Domain.Entities;
using LogiKnow.Domain.Enums;
using LogiKnow.Domain.Interfaces;
using MediatR;

namespace LogiKnow.Application.Moderation.Commands;

public record ReviewSubmissionCommand(Guid Id, bool Approve, string? Reason, string ReviewedBy)
    : IRequest<SubmissionDto>;

public class ReviewSubmissionHandler : IRequestHandler<ReviewSubmissionCommand, SubmissionDto>
{
    private readonly ISubmissionRepository _repo;
    private readonly IAcademicRepository _academicRepo;
    private readonly IBookRepository _bookRepo;

    public ReviewSubmissionHandler(ISubmissionRepository repo, IAcademicRepository academicRepo, IBookRepository bookRepo)
    {
        _repo = repo;
        _academicRepo = academicRepo;
        _bookRepo = bookRepo;
    }

    public async Task<SubmissionDto> Handle(ReviewSubmissionCommand request, CancellationToken ct)
    {
        var submission = await _repo.GetByIdAsync(request.Id, ct);
        if (submission == null)
            throw new Exception("Submission not found");

        if (submission.Status != SubmissionStatus.Pending)
            throw new Exception("Submission is not pending");

        submission.Status = request.Approve ? SubmissionStatus.Approved : SubmissionStatus.Rejected;
        submission.ReviewNotes = request.Reason;
        submission.ReviewedBy = request.ReviewedBy;
        submission.ReviewedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(submission, ct);

        // If approved, insert the entity into the respected repository
        if (request.Approve)
        {
            if (submission.EntityType == "AcademicEntry")
            {
                var entry = System.Text.Json.JsonSerializer.Deserialize<AcademicEntry>(submission.JsonData);
                if (entry != null)
                {
                    entry.Status = SubmissionStatus.Approved;
                    await _academicRepo.CreateAsync(entry, ct);
                }
            }
            else if (submission.EntityType == "Book")
            {
                var book = System.Text.Json.JsonSerializer.Deserialize<Book>(submission.JsonData);
                if (book != null)
                {
                    book.IsPublished = true;
                    // Ensure the ID is not already tracked or conflict - the submission generated a Guid,
                    // but we can generate a new one or keep it. We'll keep it.
                    await _bookRepo.CreateAsync(book, ct);
                }
            }
        }

        return new SubmissionDto
        {
            Id = submission.Id,
            EntityType = submission.EntityType,
            JsonData = submission.JsonData,
            Status = submission.Status.ToString(),
            ReviewNotes = submission.ReviewNotes,
            SubmittedBy = submission.SubmittedBy,
            ReviewedBy = submission.ReviewedBy,
            ReviewedAt = submission.ReviewedAt,
            CreatedAt = submission.CreatedAt
        };
    }
}

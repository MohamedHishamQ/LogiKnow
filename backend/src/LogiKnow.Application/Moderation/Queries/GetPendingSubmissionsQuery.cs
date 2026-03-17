using LogiKnow.Application.Common.DTOs;
using LogiKnow.Domain.Interfaces;
using MediatR;

namespace LogiKnow.Application.Moderation.Queries;

public record GetPendingSubmissionsQuery(int Page = 1, int Size = 20)
    : IRequest<PaginatedResponse<SubmissionDto>>;

public class GetPendingSubmissionsHandler : IRequestHandler<GetPendingSubmissionsQuery, PaginatedResponse<SubmissionDto>>
{
    private readonly ISubmissionRepository _repo;

    public GetPendingSubmissionsHandler(ISubmissionRepository repo)
    {
        _repo = repo;
    }

    public async Task<PaginatedResponse<SubmissionDto>> Handle(GetPendingSubmissionsQuery request, CancellationToken ct)
    {
        int skip = (request.Page - 1) * request.Size;
        var submissions = await _repo.GetPendingAsync(skip, request.Size, ct);
        var total = await _repo.CountPendingAsync(ct);

        var data = submissions.Select(s => new SubmissionDto
        {
            Id = s.Id,
            EntityType = s.EntityType,
            JsonData = s.JsonData,
            Status = s.Status.ToString(),
            ReviewNotes = s.ReviewNotes,
            SubmittedBy = s.SubmittedBy,
            ReviewedBy = s.ReviewedBy,
            ReviewedAt = s.ReviewedAt,
            CreatedAt = s.CreatedAt
        }).ToList();

        return new PaginatedResponse<SubmissionDto>
        {
            Data = data,
            Meta = new PaginationMeta
            {
                Page = request.Page,
                Size = request.Size,
                Total = total
            }
        };
    }
}

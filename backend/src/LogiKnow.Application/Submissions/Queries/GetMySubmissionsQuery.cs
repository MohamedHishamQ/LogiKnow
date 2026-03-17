using LogiKnow.Application.Common.DTOs;
using LogiKnow.Domain.Interfaces;
using MediatR;

namespace LogiKnow.Application.Submissions.Queries;

public record GetMySubmissionsQuery(string UserId, int Page = 1, int Size = 20)
    : IRequest<PaginatedResponse<SubmissionDto>>;

public class GetMySubmissionsHandler : IRequestHandler<GetMySubmissionsQuery, PaginatedResponse<SubmissionDto>>
{
    private readonly ISubmissionRepository _repo;

    public GetMySubmissionsHandler(ISubmissionRepository repo)
    {
        _repo = repo;
    }

    public async Task<PaginatedResponse<SubmissionDto>> Handle(GetMySubmissionsQuery request, CancellationToken ct)
    {
        int skip = (request.Page - 1) * request.Size;
        var submissions = await _repo.GetByUserAsync(request.UserId, skip, request.Size, ct);
        var total = await _repo.CountByUserAsync(request.UserId, ct);

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

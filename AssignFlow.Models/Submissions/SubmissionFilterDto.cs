using AssignFlow.Domain.Enums;
using AssignFlow.Models.Common;

namespace AssignFlow.Models.Submissions;

public class SubmissionFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public Guid? AssignmentId { get; set; }
    public SubmissionStatus? Status { get; set; }

    public PagedRequestDto Paging => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search
    };
}

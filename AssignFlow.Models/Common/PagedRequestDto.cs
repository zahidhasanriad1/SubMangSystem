namespace AssignFlow.Models.Common;

/// <summary>
/// Defines a bounded server-side paging request.
/// </summary>
public class PagedRequestDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public int SafePage => Math.Max(1, Page);
    public int SafePageSize => Math.Clamp(PageSize, 1, 100);
}

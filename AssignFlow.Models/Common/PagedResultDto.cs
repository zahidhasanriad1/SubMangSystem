namespace AssignFlow.Models.Common;

/// <summary>
/// Returns one page of data with the metadata required by API clients.
/// </summary>
public class PagedResultDto<T>
{
    public IReadOnlyCollection<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}

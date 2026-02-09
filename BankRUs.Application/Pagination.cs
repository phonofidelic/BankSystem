namespace BankRUs.Application.Pagination;

public record BasePageQuery(
    int Page,
    int PageSize,
    SortOrder SortOrder)
{
    public int _offset { get => Page - 1; }
    public int Skip { get => PageSize * _offset; }
}

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    PagedResultMetadata Meta);

public record PagedResultMetadata(
    int Page,
    int PageSize,
    int TotalPages,
    int TotalCount);

public enum SortOrder
{
    Ascending,
    Descending,
}
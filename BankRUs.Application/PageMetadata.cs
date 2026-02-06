namespace BankRUs.Application;

public record BasePageQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public int Offset { get => Page - 1; }
    public int Skip { get => PageSize * Offset; }
    public SortOrder SortOrder { get; init; }
}

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    PageMetadata Meta);

public record PageMetadata(
    int Page,
    int PageSize,
    int TotalPages,
    int TotalCount);

public enum SortOrder
{
    Ascending,
    Descending,
}
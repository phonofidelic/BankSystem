namespace BankRUs.Application.Paginatioin;

public record BasePageQuery(
    int Page,
    int PageSize,
    SortOrder SortOrder)
{
    public int _offset { get => Page - 1; }
    public int Skip { get => PageSize * _offset; }
}

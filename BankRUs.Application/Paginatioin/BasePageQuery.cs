namespace BankRUs.Application.Paginatioin;

public record BasePageQuery(
    int Page = 1,
    int PageSize = 50,
    SortOrder SortOrder = SortOrder.Descending)
{
    public int _offset { get => Page - 1; }
    public int Skip { get => PageSize * _offset; }
}

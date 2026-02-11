namespace BankRUs.Application.Services.PaginationService;

public record BasePageQuery(
    int Page = 1,
    int Size = 50,
    SortOrder SortOrder = SortOrder.Descending)
{
    public int _offset { get => Page - 1; }
    public int Skip { get => Size * _offset; }
}

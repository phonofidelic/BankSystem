namespace BankRUs.Application.Paginatioin;

public record BasePagedResult<T>(
    IReadOnlyList<T> Items,
    PagedResultMetadata Meta);

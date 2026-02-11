namespace BankRUs.Application.Services.PaginationService;

public record BasePagedResult<T>(
    IReadOnlyList<T> Items,
    PagedResultMetadata Meta);

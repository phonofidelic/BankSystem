namespace BankSystem.Application.Services.PaginationService;

public record BasePagedResult<TItem>(
    IReadOnlyList<TItem> Items,
    PagedResultMetadata Paging);

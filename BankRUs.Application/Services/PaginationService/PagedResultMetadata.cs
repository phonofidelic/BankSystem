namespace BankRUs.Application.Services.PaginationService;

public record PagedResultMetadata(
    int Page,
    int PageSize,
    int TotalPages,
    int TotalCount,
    string? Sort);
namespace BankRUs.Application.Paginatioin;

public record PagedResultMetadata(
    int Page,
    int PageSize,
    int TotalPages,
    int TotalCount,
    string? Sort);
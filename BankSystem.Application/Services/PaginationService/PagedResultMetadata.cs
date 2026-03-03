namespace BankSystem.Application.Services.PaginationService;

public record PagedResultMetadata(
    int Page,
    int Size,
    int TotalPages,
    int TotalCount,
    string? Order);
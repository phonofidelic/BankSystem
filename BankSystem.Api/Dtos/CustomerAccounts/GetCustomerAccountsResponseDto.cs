using BankSystem.Application.Services.PaginationService;

namespace BankSystem.Api.Dtos.CustomerAccounts;

public record GetCustomerAccountsResponseDto(
    PagedResultMetadata Paging,
    IReadOnlyList<CustomerAccountsListItemDto> Items);

public record CustomerAccountsListItemDto(
    Guid CustomerId,
    string FirstName,
    string LastName,
    string Email);

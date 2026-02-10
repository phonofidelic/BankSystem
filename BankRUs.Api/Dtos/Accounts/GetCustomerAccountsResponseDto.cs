using BankRUs.Application.Paginatioin;

namespace BankRUs.Api.Dtos.Accounts;

public record GetCustomerAccountsResponseDto(
    PagedResultMetadata Paging,
    IReadOnlyList<CustomerAccountsListItemDto> Items);

public record CustomerAccountsListItemDto(
    Guid CustomerId,
    string FirstName,
    string LastName,
    string Email);

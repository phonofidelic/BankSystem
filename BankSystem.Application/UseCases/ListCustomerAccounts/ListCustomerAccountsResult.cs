using BankSystem.Application.Services.PaginationService;
using BankSystem.Domain.Entities;

namespace BankSystem.Application.UseCases.ListCustomerAccounts;

public record ListCustomerAccountsResult(IReadOnlyList<CustomerAccount> Items, PagedResultMetadata Paging) : BasePagedResult<CustomerAccount>(Items, Paging);

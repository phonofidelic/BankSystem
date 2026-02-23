using BankRUs.Application.Services.PaginationService;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.UseCases.ListCustomerAccounts;

public record ListCustomerAccountsResult(IReadOnlyList<CustomerAccount> Items, PagedResultMetadata Paging) : BasePagedResult<CustomerAccount>(Items, Paging);

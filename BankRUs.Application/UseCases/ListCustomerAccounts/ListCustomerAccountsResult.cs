using BankRUs.Application.Services.PaginationService;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.UseCases.ListCustomerAccounts;

public record ListCustomerAccountsResult(IReadOnlyList<CustomerAccount> Items, PagedResultMetadata Meta) : BasePagedResult<CustomerAccount>(Items, Meta);

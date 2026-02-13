using BankRUs.Application.Services.PaginationService;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.UseCases.ListCustomerAccounts;

public record ListCustomerAccountsResult(IReadOnlyList<Customer> Items, PagedResultMetadata Meta) : BasePagedResult<Customer>(Items, Meta);

using BankRUs.Application.Paginatioin;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.UseCases.CustomerServiceRep.ListCustomerAccounts;

public record ListCustomerAccountsResult(IReadOnlyList<Customer> Items, PagedResultMetadata Meta) : BasePagedResult<Customer>(Items, Meta);

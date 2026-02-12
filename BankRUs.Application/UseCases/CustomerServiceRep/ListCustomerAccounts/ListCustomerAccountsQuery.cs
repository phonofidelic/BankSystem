using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.Services.PaginationService;

namespace BankRUs.Application.UseCases.CustomerServiceRep.ListCustomerAccounts;

public record ListCustomerAccountsQuery(
    string? Search,
    string? FirstName,
    string? LastName,
    string? Email,
    string? Ssn) : BasePageQuery;

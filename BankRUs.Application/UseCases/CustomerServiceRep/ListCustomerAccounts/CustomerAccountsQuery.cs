using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.Services.PaginationService;

namespace BankRUs.Application.UseCases.CustomerServiceRep.ListCustomerAccounts;

public record CustomerAccountsQuery(CustomersSearchQuery? Search,
    int Page,
    int Size,
    SortOrder Sort) : BasePageQuery(Page, Size, Sort);

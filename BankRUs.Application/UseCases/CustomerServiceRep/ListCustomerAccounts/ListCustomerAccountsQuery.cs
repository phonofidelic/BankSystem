using BankRUs.Application.Services.PaginationService;

namespace BankRUs.Application.UseCases.CustomerServiceRep.ListCustomerAccounts;

public record ListCustomerAccountsQuery : BasePageQuery
{
    public ListCustomerAccountsQuery(int Page, int PageSize, SortOrder SortOrder) : base(Page, PageSize, SortOrder)
    {
    }
}
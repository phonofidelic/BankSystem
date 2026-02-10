
using BankRUs.Application.Paginatioin;
using BankRUs.Application.Services.CustomerService;

namespace BankRUs.Application.UseCases.CustomerServiceRep.ListCustomerAccounts;

public class ListCustomerAccountsHandler(ICustomerService customerService) : IHandler<ListCustomerAccountsQuery, ListCustomerAccountsResult>
{
    private readonly ICustomerService _customerService = customerService;
    public async Task<ListCustomerAccountsResult> HandleAsync(ListCustomerAccountsQuery query)
    {
        var result = await _customerService.GetCustomersAsPagedResult(new CustomersPageQuery(
            Page: query.Page,
            PageSize: query.PageSize,
            SortOrder: query.SortOrder));

        return new ListCustomerAccountsResult(
            Items: result.Items,
            Meta: result.Meta);
    }
}

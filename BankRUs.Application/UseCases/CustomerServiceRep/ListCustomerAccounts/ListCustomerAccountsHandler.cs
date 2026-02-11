using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.Services.PaginationService;

namespace BankRUs.Application.UseCases.CustomerServiceRep.ListCustomerAccounts;

public class ListCustomerAccountsHandler(
    ICustomerService customerService, 
    IPaginationService paginationService) : IHandler<ListCustomerAccountsQuery, ListCustomerAccountsResult>
{
    private readonly ICustomerService _customerService = customerService;
    private readonly IPaginationService _paginationService = paginationService;
    public async Task<ListCustomerAccountsResult> HandleAsync(ListCustomerAccountsQuery query)
    {
        var customers = await _customerService.GetCustomersQueryAsync();
        var result = _paginationService.GetPagedResult(new CustomersPageQuery(
            Page: query.Page,
            PageSize: query.Size,
            SortOrder: query.SortOrder), customers);

        return new ListCustomerAccountsResult(
            Items: result.Items,
            Meta: result.Meta);
    }
}

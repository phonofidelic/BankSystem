using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.Services.PaginationService;

namespace BankRUs.Application.UseCases.CustomerServiceRep.ListCustomerAccounts;

public class ListCustomerAccountsHandler(
    ICustomerService customerService, 
    IPaginationService paginationService) : IHandler<CustomerAccountsQuery, ListCustomerAccountsResult>
{
    private readonly ICustomerService _customerService = customerService;
    private readonly IPaginationService _paginationService = paginationService;
    public async Task<ListCustomerAccountsResult> HandleAsync(CustomerAccountsQuery query)
    {
        var customers = query.Search != null 
            ? await _customerService.SearchCustomersAsync(query.Search)
            : await _customerService.GetCustomersAsync();

        var result = _paginationService.GetPagedResult(query, customers);

        return new ListCustomerAccountsResult(
            Items: result.Items,
            Meta: result.Meta);
    }
}

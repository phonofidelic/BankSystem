using BankRUs.Application.Services.CustomerAccountService;
using BankRUs.Application.Services.PaginationService;
using Microsoft.Extensions.Logging;

namespace BankRUs.Application.UseCases.ListCustomerAccounts;

public class ListCustomerAccountsHandler(
    ILogger<ListCustomerAccountsHandler> logger,
    ICustomerAccountService customerService, 
    IPaginationService paginationService) : IHandler<ListCustomerAccountsPageQuery, ListCustomerAccountsResult>
{
    private readonly ILogger<ListCustomerAccountsHandler> _logger = logger;
    private readonly ICustomerAccountService _customerService = customerService;
    private readonly IPaginationService _paginationService = paginationService;
    public async Task<ListCustomerAccountsResult> HandleAsync(ListCustomerAccountsPageQuery query)
    {
        var customers = await _customerService.SearchCustomerAccountsAsync(query);
        var result = await _paginationService.GetPagedResultAsync(query, customers);

        return new ListCustomerAccountsResult(
            Items: result.Items,
            Paging: result.Paging);
    }
}

using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.Services.PaginationService;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BankRUs.Application.UseCases.ListCustomerAccounts;

public class ListCustomerAccountsHandler(
    ILogger<ListCustomerAccountsHandler> logger,
    ICustomerService customerService, 
    IPaginationService paginationService) : IHandler<ListCustomerAccountsQuery, ListCustomerAccountsResult>
{
    private readonly ILogger<ListCustomerAccountsHandler> _logger = logger;
    private readonly ICustomerService _customerService = customerService;
    private readonly IPaginationService _paginationService = paginationService;
    public async Task<ListCustomerAccountsResult> HandleAsync(ListCustomerAccountsQuery query)
    {
        var customers = await _customerService.SearchCustomersAsync(query);
        var result = await _paginationService.GetPagedResultAsync(query, customers);

        return new ListCustomerAccountsResult(
            Items: result.Items,
            Meta: result.Meta);
    }
}

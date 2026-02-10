
using BankRUs.Application.Paginatioin;
using BankRUs.Application.Services.CustomerService;

namespace BankRUs.Application.UseCases.CustomerServiceRep.ListCustomerAccounts;

public class ListCustomerAccountsHandler(ICustomerService customerService) : IHandler<ListCustomerAccountsQuery, ListCustomerAccountsResult>
{
    private readonly ICustomerService _customerService = customerService;
    public async Task<ListCustomerAccountsResult> HandleAsync(ListCustomerAccountsQuery query)
    {
        throw new NotImplementedException();
    }
}

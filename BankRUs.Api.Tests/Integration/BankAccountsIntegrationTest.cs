using BankRUs.Api.Dtos.BankAccounts;
using BankRUs.Api.Tests.Infrastructure;


namespace BankRUs.Api.Tests.Integration;

public class BankAccountsIntegrationTest(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetTransactionsForBankAccount_WhenBankAccountExists_ShouldReturn200AndMax50Transactions()
    {
        // Given
        await LoginClient(_testCustomerCredentials.Email, _testCustomerCredentials.Password);

        // When
        string paging = "?size=5&page=2&order=ascending";

        // Then
        await Paging_ShouldReflectPagingQuery<CustomerTransactionsListItemDto>($"/api/bank-accounts/{_testCustomerBankAccountId}/transactions{paging}");
    }
}
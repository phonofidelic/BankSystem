using System.Net;
using System.Net.Http.Json;
using BankRUs.Api.Dtos.BankAccounts;
using BankRUs.Api.Tests.Infrastructure;


namespace BankRUs.Api.Tests.Integration;

public class BankAccountsIntegrationTest(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task PostWithdrawal_WhenSufficientFunds_ShouldReturn201Created()
    {
        // Given
        await LoginClient(_testCustomerCredentials.Email, _testCustomerCredentials.Password);
        var postWithdrawalRequest = new PostWithdrawalRequestDto(
            Amount: 100,
            IsoCurrencySymbol: "SEK",
            Reference: "Test withdrawal transaction"
        );
    
        // When
        var response = await _client.PostAsJsonAsync($"/api/bank-accounts/{_testCustomerBankAccountId}/withdrawals", postWithdrawalRequest);
    
        // Then
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostWithdrawal_WhenInsufficientFunds_ShouldReturn400BadRequest()
    {
        // Given
        await LoginClient(_testCustomerCredentials.Email, _testCustomerCredentials.Password);
        var postWithdrawalRequest = new PostWithdrawalRequestDto(
            Amount: 1000000000,
            IsoCurrencySymbol: "SEK",
            Reference: "Test withdrawal transaction"
        );
    
        // When
        var response = await _client.PostAsJsonAsync($"/api/bank-accounts/{_testCustomerBankAccountId}/withdrawals", postWithdrawalRequest);
    
        // Then
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostDeposit_WhenPositiveAmount_ShouldReturn400BagRequest()
    {
        // Given
        await LoginClient(_testCustomerCredentials.Email, _testCustomerCredentials.Password);
        var postWithdrawalRequest = new PostWithdrawalRequestDto(
            Amount: 100,
            IsoCurrencySymbol: "SEK",
            Reference: "Test withdrawal transaction"
        );
    
        // When
        var response = await _client.PostAsJsonAsync($"/api/bank-accounts/{_testCustomerBankAccountId}/deposits", postWithdrawalRequest);
    
        // Then
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task PostDeposit_WhenNonPositiveAmount_ShouldReturn201Created(decimal amount)
    {
        // Given
        await LoginClient(_testCustomerCredentials.Email, _testCustomerCredentials.Password);
        var postWithdrawalRequest = new PostWithdrawalRequestDto(
            Amount: amount,
            IsoCurrencySymbol: "SEK",
            Reference: "Test withdrawal transaction"
        );
    
        // When
        var response = await _client.PostAsJsonAsync($"/api/bank-accounts/{_testCustomerBankAccountId}/deposits", postWithdrawalRequest);
    
        // Then
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


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
using System.Net;
using BankRUs.Api.Tests.Infrastructure;
using BankRUs.Application.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BankRUs.Api.Tests.Integration;

public class TransactionsIntegrationTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    private readonly DefaultAdmin _defaultAdmin = factory.Services.GetRequiredService<IOptions<DefaultAdmin>>().Value;
    
    [Fact]
    public async Task GetAllTransactions_WhenTransactionsExist_ShouldReturnReturn200AndMax50Transactions()
    {
        // Arrange:
        await LoginClient(_defaultAdmin.Email, _defaultAdmin.Password);

        // Act:
        var response = await _client.GetAsync("/api/transactions");

        // Assert;
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

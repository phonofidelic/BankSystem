using System;
using System.Net;
using BankRUs.Api.Tests.Infrastructure;

namespace BankRUs.Api.Tests.Integration;

public class CustomerAccountsIntegrationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_WhenCustomerAccountsExist_SouldReturn200AndCustomerAccounts()
    {
        var response = await _client.GetAsync("/api/accounts/customers");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

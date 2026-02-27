using System.Net;
using System.Net.Http.Json;
using BankRUs.Api.Dtos.Transactions;
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
        var content = await response.Content.ReadFromJsonAsync<GetTransactionsResponseDto>();

        // Assert;
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(50, content.Items.Count);
        Assert.True(content.Paging.TotalCount > content.Items.Count);
    }

    [Fact]
    public async Task GetAllTransactions_WhenPagingIsSpecified_ShouldReflectPagingQuery()
    {
        // Given
        await LoginClient(_defaultAdmin.Email, _defaultAdmin.Password);

        // When
        string paging = "?page=2&size=15&sortOrder=ascending";

        // THen
        await Paging_ShouldReflectPagingQuery<GetTransactionsResponseDto>($"/api/transactions{paging}");
    }
}

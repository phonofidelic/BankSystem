using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Azure;
using BankRUs.Api.Dtos.Auth;
using BankRUs.Api.Tests.Infrastructure;
using BankRUs.Application.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BankRUs.Api.Tests.Integration;

public class CustomerAccountsIntegrationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly DefaultAdmin _defaultAdmin = factory.Services.GetRequiredService<IOptions<DefaultAdmin>>().Value;

    [Fact]
    public async Task Get_WhenCustomerAccountsExist_ShouldReturn200AndCustomerAccounts()
    {
        // Arrange:
        // Log in using seeded admin credentials
        var loginResponse = await GetToken(_defaultAdmin.Email, _defaultAdmin.Password);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse?.Token);

        // Act:
        var response = await _client.GetAsync("/api/accounts/customers");

        // Assert:
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>
    /// See https://stackoverflow.com/a/68424710
    /// </summary>
    private async Task<LoginResponseDto?> GetToken(string username, string password)
    {
        var loginRequest = new LoginRequestDto(username, password);

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        if (!response.IsSuccessStatusCode) 
            return null;

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

        return loginResponse;
    }
}

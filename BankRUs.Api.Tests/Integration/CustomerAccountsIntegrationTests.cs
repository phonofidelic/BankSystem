using BankRUs.Api.Dtos.Accounts;
using BankRUs.Api.Dtos.Auth;
using BankRUs.Api.Tests.Infrastructure;
using BankRUs.Application.Configuration;
using BankRUs.Domain.ValueObjects;
using BankRUs.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BankRUs.Api.Tests.Integration;

public class CustomerAccountsIntegrationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    //private readonly int _seed = factory.Seed;
    private readonly HttpClient _client = factory.CreateClient();
    private readonly DefaultAdmin _defaultAdmin = factory.Services.GetRequiredService<IOptions<DefaultAdmin>>().Value;
    private readonly CustomerAccountDetails _testCustomerAccountDetails = new CustomerAccountDetails(
            firstName: "Test",
            lastName: "Testerson",
            email: "test.testerson@example.com",
            socialSecurityNumber: Seeder.GenerateSocialSecurityNumber(factory.Seed));

    [Fact]
    public async Task Get_WhenCustomerAccountsExist_ShouldReturn200AndCustomerAccounts()
    {
        // Arrange:
        // Log in as Admin
        await LoginClient(_defaultAdmin.Email, _defaultAdmin.Password);

        // Act:
        var response = await _client.GetAsync("/api/accounts/customers");

        // Assert:
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var getCustomerAccountsResponse = await response.Content.ReadFromJsonAsync<GetCustomerAccountsResponseDto>();
        
        Assert.NotNull(getCustomerAccountsResponse);
        Assert.NotEmpty(getCustomerAccountsResponse.Items);
        Assert.IsType<CustomerAccountsListItemDto>(getCustomerAccountsResponse.Items[0]);
        
        Assert.NotNull(getCustomerAccountsResponse.Paging);
        Assert.Equal(1, getCustomerAccountsResponse.Paging.Page);
        Assert.Equal(50, getCustomerAccountsResponse.Paging.PageSize);
        Assert.Equal("descending", getCustomerAccountsResponse.Paging.Sort);
    }

    [Fact]
    public async Task Get_WhenPagingIsSpecified_ShouldReflectPagingQuery()
    {
        // Arrange:
        // Log in as Admin
        await LoginClient(_defaultAdmin.Email, _defaultAdmin.Password);

        // Act:
        var response = await _client.GetAsync("/api/accounts/customers?size=5&page=2&sortOrder=ascending");

        // Assert:
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var getCustomerAccountsResponse = await response.Content.ReadFromJsonAsync<GetCustomerAccountsResponseDto>();

        Assert.NotNull(getCustomerAccountsResponse);
        Assert.NotNull(getCustomerAccountsResponse.Paging);
        Assert.Equal(2, getCustomerAccountsResponse.Paging.Page);
        Assert.Equal(5, getCustomerAccountsResponse.Paging.PageSize);
        Assert.Equal("ascending", getCustomerAccountsResponse.Paging.Sort);
    }

    [Fact]
    public async Task Post_WhenValidDataIsProvided_ShouldCreateNewCustomerAccount()
    {
        // Arrange:
        await LoginClient(_defaultAdmin.Email, _defaultAdmin.Password);

        var createCustomerRequest = new CreateCustomerAccountRequestDto(
            FirstName: _testCustomerAccountDetails.FirstName!,
            LastName: _testCustomerAccountDetails.LastName!,
            Email: _testCustomerAccountDetails.Email!,
            SocialSecurityNumber: _testCustomerAccountDetails.SocialSecurityNumber!,
            Password: "Test@123");

        // Act:
        var response = await _client.PostAsJsonAsync("/api/accounts/customers/create", createCustomerRequest);

        // Assert;
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_WhenExistingSsnIsProvided_ShouldRespondBadRequest()
    {
        // Arrange:
        await LoginClient(_defaultAdmin.Email, _defaultAdmin.Password);

        var secondCreateCustomerRequest = new CreateCustomerAccountRequestDto(
            FirstName: _testCustomerAccountDetails.FirstName!,
            LastName: _testCustomerAccountDetails.LastName!,
            Email: _testCustomerAccountDetails.Email!,
            SocialSecurityNumber: _testCustomerAccountDetails.SocialSecurityNumber!,
            Password: "Test@123");
        
        // Act:
        var secondResponse = await _client.PostAsJsonAsync("/api/accounts/customers/create", secondCreateCustomerRequest);

        // Assert:
        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);
    }

    /// <summary>
    /// See https://stackoverflow.com/a/68424710
    /// </summary>
    private async Task LoginClient(string username, string password)
    {
        var loginRequest = new LoginRequestDto(username, password);

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>() ?? throw new Exception("Login failed");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse?.Token);
    }
}

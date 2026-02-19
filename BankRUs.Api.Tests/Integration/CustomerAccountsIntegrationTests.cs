using BankRUs.Api.Dtos.Accounts;
using BankRUs.Api.Dtos.Auth;
using BankRUs.Api.Tests.Exceptions;
using BankRUs.Api.Tests.Infrastructure;
using BankRUs.Application.Configuration;
using BankRUs.Application.UseCases.GetCustomerAccountDetails;
using BankRUs.Domain.Entities;
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
            socialSecurityNumber: Seeder.GenerateSocialSecurityNumber(factory.NextSeed));

    [Fact]
    public async Task GetCustomerAccounts_WhenCustomerAccountsExist_ShouldReturn200AndCustomerAccounts()
    {
        // Arrange:
        // Log in as Admin
        await LoginClient(_defaultAdmin.Email, _defaultAdmin.Password);

        // Act:
        var response = await _client.GetAsync("/api/customer-accounts");

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
    public async Task GetCustomerAccounts_WhenPagingIsSpecified_ShouldReflectPagingQuery()
    {
        // Arrange:
        // Log in as Admin
        await LoginClient(_defaultAdmin.Email, _defaultAdmin.Password);

        // Act:
        var response = await _client.GetAsync("/api/customer-accounts?size=5&page=2&sortOrder=ascending");

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
    public async Task GetCustomerAccount_WhenCustomerAccountExists_ShouldReturnDetails()
    {
        // Arrange:
        await LoginClient(_defaultAdmin.Email, _defaultAdmin.Password);
        var ssn = Seeder.GenerateSocialSecurityNumber(factory.NextSeed);
        var createCustomerAccountRequest = new CreateCustomerAccountRequestDto(
            FirstName: "Test",
            LastName: "Details",
            Email: "test.details@example.com",
            SocialSecurityNumber: ssn,
            Password: "Test@123");

        var createCustomerAccountResponse = await _client.PostAsJsonAsync("/api/customer-accounts/create", createCustomerAccountRequest);
        var createCustomerAccountResponseContent = await createCustomerAccountResponse.Content.ReadFromJsonAsync<CreateCustomerAccountResponseDto>();
        var customerAccountId = createCustomerAccountResponseContent?.CustomerAccountId ?? throw new Exception("Customer account not found");

        // Act:
        var response = await _client.GetAsync($"/api/customer-accounts/{customerAccountId}");
        var content = await response.Content.ReadFromJsonAsync<GetCustomerAccountResponseDto>();

        // Assert:
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal("Test", content.FirstName);
        Assert.Equal("Details", content.LastName);
        Assert.Equal("test.details@example.com", content.Email);
        Assert.Equal(ssn, content.Ssn);
        Assert.Equal(CustomerAccountStatus.Opened.ToString(), content.AccountStatus);

        Assert.NotEmpty(content.BankAccounts);
        Assert.Single(content.BankAccounts);
        Assert.Equal("Default Checking Account", content.BankAccounts[0].Name);
    }

    [Fact]
    public async Task CreateCustomer_WhenValidDataIsProvided_ShouldCreateNewCustomerAccount()
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
        var response = await _client.PostAsJsonAsync("/api/customer-accounts/create", createCustomerRequest);

        // Assert;
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var responseContent = await response.Content.ReadFromJsonAsync<CreateCustomerAccountResponseDto>();

        Assert.NotNull(responseContent);
        Assert.IsType<Guid>(responseContent.CustomerAccountId);
    }

    [Fact]
    public async Task CreateCustomer_WhenExistingSsnIsProvided_ShouldRespondBadRequest()
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
        var secondResponse = await _client.PostAsJsonAsync("/api/customer-accounts/create", secondCreateCustomerRequest);

        // Assert:
        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }

    [Fact]
    public async Task PatchCustomer_WhenChangedDetailsProvided_UpdatesCustomerAccountDetails()
    {
        // Arrange:
        await LoginClient(_defaultAdmin.Email, _defaultAdmin.Password);

        var createCustomerRequest = new CreateCustomerAccountRequestDto(
            FirstName: "Test",
            LastName: "Patch",
            Email: "test.patch@example.com",
            SocialSecurityNumber: Seeder.GenerateSocialSecurityNumber(factory.NextSeed),
            Password: "TestP@ssw0rd!"
        );

        // Create new Customer account for test
        var createCustomerResponse = await _client.PostAsJsonAsync("/api/customer-accounts/create", createCustomerRequest);
        var createCustomerContent = await createCustomerResponse.Content.ReadFromJsonAsync<CreateCustomerAccountResponseDto>() ?? throw new CreateTestCustomerAccountException();
        var customerAccountId = createCustomerContent.CustomerAccountId;

        // Act:
        var patchCustomerAccountRequest = new PatchCustomerAccountRequestDto(
            FirstName: "Edited",
            null, null, null);

        var patchCustomerAccountResponse = await _client.PatchAsJsonAsync($"/api/customer-accounts/{customerAccountId}", patchCustomerAccountRequest);
        
        // Assert:
        Assert.Equal(HttpStatusCode.NoContent, patchCustomerAccountResponse.StatusCode);

        // Get updated Customer account details
        var getPatchedCustomerAccountResponse = await _client.GetAsync($"/api/customer-accounts/{customerAccountId}");
        Assert.Equal(HttpStatusCode.OK, getPatchedCustomerAccountResponse.StatusCode);
        
        var patchedCustomerAccountContent = await getPatchedCustomerAccountResponse.Content.ReadFromJsonAsync<GetCustomerAccountResponseDto>();
        Assert.NotNull(patchedCustomerAccountContent);
        Assert.Equal("Edited", patchedCustomerAccountContent.FirstName);
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

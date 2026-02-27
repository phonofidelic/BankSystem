using BankRUs.Api.Dtos.CustomerAccounts;
using BankRUs.Api.Tests.Exceptions;
using BankRUs.Api.Tests.Infrastructure;
using BankRUs.Application.Configuration;
using BankRUs.Domain.Entities;
using BankRUs.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;

namespace BankRUs.Api.Tests.Integration;

public class CustomerAccountsIntegrationTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    private readonly DefaultAdmin _defaultAdmin = factory.Services.GetRequiredService<IOptions<DefaultAdmin>>().Value;
    
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
        // Given
        await LoginClient(_defaultAdmin.Email, _defaultAdmin.Password);

        // When
        string paging = "?size=5&page=2&sortOrder=ascending";

        // Then
        await Paging_ShouldReflectPagingQuery<GetCustomerAccountsResponseDto>($"/api/customer-accounts{paging}");
    }

    [Fact]
    public async Task GetCustomerAccount_WhenCustomerAccountExists_ShouldReturnDetails()
    {
        // Arrange:
        await LoginClient(_defaultAdmin.Email, _defaultAdmin.Password);
        var ssn = Seeder.GenerateSocialSecurityNumber(NextSeed);
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
    public async Task CreateCustomerAccount_WhenValidDataIsProvided_ShouldCreateNewCustomerAccount()
    {
        // Arrange:
        await LoginClient(_defaultAdmin.Email, _defaultAdmin.Password);

        var createCustomerRequest = new CreateCustomerAccountRequestDto(
            FirstName: "Test",
            LastName: "Create",
            Email: "test.create@example.com",
            SocialSecurityNumber: Seeder.GenerateSocialSecurityNumber(NextSeed),
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
    public async Task CreateCustomerAccount_WhenExistingSsnIsProvided_ShouldRespondBadRequest()
    {
        // Arrange:
        await LoginClient(_defaultAdmin.Email, _defaultAdmin.Password);

        var ssn = Seeder.GenerateSocialSecurityNumber(NextSeed);
        var createFirstCustomerRequest = new CreateCustomerAccountRequestDto(
            FirstName: "Test",
            LastName: "First",
            Email: "test.first@example.com",
            SocialSecurityNumber: ssn,
            Password: "TestP@ssw0rd!"
        );
        await _client.PostAsJsonAsync("/api/customer-accounts/create", createFirstCustomerRequest);

        var secondCreateCustomerRequest = new CreateCustomerAccountRequestDto(
            FirstName: "Test",
            LastName: "Second",
            Email: "test.second@example.com",
            SocialSecurityNumber: ssn,
            Password: "TestP@ssw0rd!");
        
        // Act:
        var secondResponse = await _client.PostAsJsonAsync("/api/customer-accounts/create", secondCreateCustomerRequest);

        // Assert:
        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }

    [Fact]
    public async Task PatchCustomerAccount_WhenChangedDetailsProvided_UpdatesCustomerAccountDetails()
    {
        // Arrange:
        await LoginClient(_defaultAdmin.Email, _defaultAdmin.Password);

        var createCustomerRequest = new CreateCustomerAccountRequestDto(
            FirstName: "Test",
            LastName: "Patch",
            Email: "test.patch@example.com",
            SocialSecurityNumber: Seeder.GenerateSocialSecurityNumber(NextSeed),
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

    [Fact]
    public async Task CloseCustomerAccount_WhenCustomerAccountIsOpen_ShouldCloseCustomerAccount()
    {
        // Arrange:
        await LoginClient(_defaultAdmin.Email, _defaultAdmin.Password);

        // Create new Customer account for test
        var createCustomerAccountRequest = new CreateCustomerAccountRequestDto(
            FirstName: "Test",
            LastName: "Close",
            Email: "test.close@example.com",
            SocialSecurityNumber: Seeder.GenerateSocialSecurityNumber(NextSeed),
            Password: "TestP@ssw0rd!"
        );

        var createCustomerAccountResponse = await _client.PostAsJsonAsync("/api/customer-accounts/create", createCustomerAccountRequest);
        var createCustomerAccountContent = await createCustomerAccountResponse.Content.ReadFromJsonAsync<CreateCustomerAccountResponseDto>() ?? throw new CreateTestCustomerAccountException();
        var customerAccountId = createCustomerAccountContent.CustomerAccountId;
        
        // Act:
        var response = await _client.DeleteAsync($"/api/customer-accounts/{customerAccountId}");

        // Assert:
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getClosedCustomerAccountResponse = await _client.GetAsync($"/api/customer-accounts/{customerAccountId}");
        var getClosedCustomerAccountContent = await getClosedCustomerAccountResponse.Content.ReadFromJsonAsync<GetCustomerAccountResponseDto>();

        Assert.Equal(HttpStatusCode.OK, getClosedCustomerAccountResponse.StatusCode);
        Assert.NotNull(getClosedCustomerAccountContent);
        Assert.Equal("", getClosedCustomerAccountContent.FirstName);
        Assert.Equal("", getClosedCustomerAccountContent.LastName);
        Assert.Equal("", getClosedCustomerAccountContent.Email);
        Assert.Equal("", getClosedCustomerAccountContent.Ssn);
        Assert.Equal(CustomerAccountStatus.Closed.ToString(), getClosedCustomerAccountContent.AccountStatus);

        foreach (var bankAccount in getClosedCustomerAccountContent.BankAccounts)
        {
            Assert.Equal(BankAccountStatus.Closed.ToString(), bankAccount.AccountStatus);
            Assert.Equal(0, bankAccount.CurrentBalance);
            Assert.Equal("", bankAccount.Name);
        }
    }
}

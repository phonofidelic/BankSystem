using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BankRUs.Api.Dtos.Auth;
using BankRUs.Application;
using BankRUs.Application.Services.PaginationService;
using BankRUs.Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BankRUs.Api.Tests.Infrastructure;

public abstract class BaseIntegrationTest(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private static readonly int _seed = 184765;

    private readonly Random _random = new (_seed);

    protected UserCredentials _testCustomerCredentials = factory.TestCustomerCredentials;

    protected Guid _testCustomerBankAccountId { get; set; } = factory.TestCustomerBankAccountId;
    
    protected int NextSeed { get => _random.Next(); }
    
    protected readonly IUnitOfWork _unitOfWork = factory.Services.GetRequiredService<IUnitOfWork>();
    
    protected UserManager<ApplicationUser> _userManager = factory.Services.GetRequiredService<UserManager<ApplicationUser>>();

    protected readonly HttpClient _client = factory.CreateClient();

    /// <summary>
    /// See https://stackoverflow.com/a/68424710
    /// </summary>
    protected async Task LoginClient(string username, string password)
    {
        var loginRequest = new LoginRequestDto(username, password);

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>() ?? throw new Exception("Login failed");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse?.Token);
    }

    protected async Task Paging_ShouldReflectPagingQuery<T>(string url, UserCredentials credentials)
    {
        // Arrange:
        await LoginClient(credentials.Email, credentials.Password);
        var paging = BasePageQuery.Parse(url);

        // Act:
        var response = await _client.GetAsync(url);
        var content = await response.Content.ReadFromJsonAsync<BasePagedResult<T>>();
        
        // Assert:
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(paging.Page, content.Paging.Page);
        Assert.Equal(paging.Size, content.Paging.PageSize);
        Assert.Equal(paging.SortOrder.ToString(), content.Paging.Sort, ignoreCase: true);
    }
}

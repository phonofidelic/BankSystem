using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BankRUs.Api.Dtos.Auth;

namespace BankRUs.Api.Tests.Infrastructure;

public abstract class BaseIntegrationTest(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private static readonly int _seed = 184765;

    private readonly Random _random = new (_seed);

    protected readonly HttpClient _client = factory.CreateClient();
    
    protected int NextSeed { get => _random.Next(); }

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
}

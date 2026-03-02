using BankRUs.Application;
using BankRUs.Infrastructure.Persistence;
using BankRUs.Infrastructure.Repositories;
using BankRUs.Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using BankRUs.Application.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using BankRUs.Application.Services.EmailService;
using BankRUs.Application.Services.Identity;
using Microsoft.AspNetCore.Identity;

namespace BankRUs.Api.Tests.Infrastructure;

public class ApiFactory : WebApplicationFactory<Program>
{
    private static readonly int _seed = 184765;

    private Random _random = new(_seed);
    private SqliteConnection? _connection;

    public int NextSeed { get => _random.Next(); }

    public UserCredentials TestCustomerCredentials { get; } = new ("test.customer@example.com", "T3stP@ssw0rd");

    public Guid TestCustomerBankAccountId { get; } = Guid.Parse("414c4399-9a9e-48c9-b8bb-5f4a52d796dd");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtOptions:Issuer"] = "test.bank.se",
                ["JwtOptions:Audience"] = "testclient.se",
                ["JwtOptions:Secret"] = "this-is-a-test-secret-key-that-is-long-enough",
                ["JwtOptions:ExpiresMinutes"] = "15"
            });
        });

        builder.ConfigureServices(async services =>
        {
            // 1) Remove DbContext + options
            services.RemoveAll<ApplicationDbContext>();
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();

            // 2) Remove options set up by `AddDbContext` (i.e. `options.UseSqlServer(...)`)
            services.RemoveAll<IConfigureOptions<DbContextOptions<ApplicationDbContext>>>();
            services.RemoveAll<IOptionsChangeTokenSource<DbContextOptions<ApplicationDbContext>>>();

            var toRemove = services
                .Where(sd =>
                    sd.ServiceType.FullName != null &&
                    sd.ServiceType.FullName.Contains("IDbContextOptionsConfiguration") &&
                    sd.ServiceType.GenericTypeArguments.Contains(typeof(ApplicationDbContext)))
                .ToList();

            foreach (var sd in toRemove)
                services.Remove(sd);

            // 3) Configure test services
            services.Configure<DefaultAdmin>(opts =>
            {
                opts.Username = "testadmin";
                opts.Password = "Test@12345";
                opts.Email = "testadmin@test.com";
            });

            services.Configure<AppSettings>(opts =>
            {
                opts.SystemCulture = "sv-SE";
                opts.DefaultCurrency = new()
                {
                    EnglishName = "Swedish Krona",
                    NativeName = "Svensk krona",
                    Symbol = "kr",
                    ISOSymbol = "SEK"
                };
                opts.SupportedCurrencies =
                [
                    new() { EnglishName = "Swedish Krona", NativeName = "Svensk krona", Symbol = "kr", ISOSymbol = "SEK" },
                    new() { EnglishName = "Euro", NativeName = "Euro", Symbol = "€", ISOSymbol = "EUR" },
                    new() { EnglishName = "US Dollar", NativeName = "US Dollar", Symbol = "$", ISOSymbol = "USD" }
                ];
            });

            services.AddScoped<IEmailSender, TestEmailSender>();

            // 4) Add services for seeding mock data
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IIdentityService, IdentityService>();

            // 5) Add test database
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                // options.UseInMemoryDatabase("BankSystem.Api.Tests_IntegrationTesting");
                options.UseSqlite(_connection);
            });

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            db.Database.EnsureCreated();

            await IdentitySeeder.SeedAsync(scope.ServiceProvider);
            await CurrencySeeder.SeedAsync(scope.ServiceProvider);
            
            db.SetTimestamps(false);
            await SeedDatabase(scope);
            await SeedTestCustomerAccount(scope);
            await unitOfWork.SaveAsync();
            db.SetTimestamps(true);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection?.Dispose();
    }

    private static async Task SeedDatabase(IServiceScope scope)
    {
        await Seeder.GenerateSeededDataAsync(count: 10, seed: _seed, scope.ServiceProvider);
    }

    private async Task SeedTestCustomerAccount(IServiceScope scope)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var testCustomerApplicationUserId = Guid.NewGuid();
        var request = new CreateApplicationUserRequest
        (
            Email: TestCustomerCredentials.Email,
            FirstName: "Test",
            LastName: "Customer",
            Password: TestCustomerCredentials.Password
        );

        await IdentitySeeder.SeedTestUser(
            userManager,
            request,
            testCustomerApplicationUserId,
            Roles.Customer);

        await Seeder.CreateTestCustomerAccount(
            request, 
            testCustomerApplicationUserId,
            TestCustomerBankAccountId,
            NextSeed, 
            scope.ServiceProvider);
    }
}

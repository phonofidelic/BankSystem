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

namespace BankRUs.Api.Tests.Infrastructure;

public class ApiFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

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

            // 3) Configure test options
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

            // 4) Add services for seeding mock data
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 4) Add test database
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

            db.Database.EnsureCreated();
            // db.Database.Migrate();


            await IdentitySeeder.SeedAsync(scope.ServiceProvider);
            await CurrencySeeder.SeedAsync(scope.ServiceProvider);
            await SeedDatabase(scope);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection?.Dispose();
    }

    private static async Task SeedDatabase(IServiceScope scope)
    {
        // Generate/remove seeded data
        const int SEED = 184765;
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await Seeder.GenerateSeededDataAsync(count: 10, seed: SEED, scope.ServiceProvider);
        // await Seeder.RemoveSeededDataAsync(SEED, scope.ServiceProvider);

        dbContext.SetTimestamps(false);
        await unitOfWork.SaveAsync();
        dbContext.SetTimestamps(true);
    }
}

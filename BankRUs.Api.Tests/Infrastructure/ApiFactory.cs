using BankRUs.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace BankRUs.Api.Tests.Infrastructure;

internal class ApiFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
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
                    sd.ServiceType.GenericTypeArguments.Contains(typeof(ApplicationDbContext)));

            foreach (var sd in toRemove)
                services.Remove(sd);

            // 3) Add test database
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                //options.UseInMemoryDatabase("BankSystem.Api.Tests_IntegrationTesting");
                options.UseSqlite(_connection);
            });

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection?.Dispose();
    }
}

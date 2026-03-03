using BankSystem.Application;
using BankSystem.Application.BankAccounts;
using BankSystem.Application.Configuration;
using BankSystem.Application.Repositories;
using BankSystem.Application.Services.AuditLog;
using BankSystem.Application.Services.Authentication;
using BankSystem.Application.Services.Authentication.AuthenticateUser;
using BankSystem.Application.Services.CurrencyService;
using BankSystem.Application.Services.CustomerAccountService;
using BankSystem.Application.Services.EmailService;
using BankSystem.Application.Services.Identity;
using BankSystem.Application.Services.PaginationService;
using BankSystem.Application.Services.TransactionService;
using BankSystem.Application.UseCases.CloseCustomerAccount;
using BankSystem.Application.UseCases.GetCustomerAccountDetails;
using BankSystem.Application.UseCases.ListAllTransactions;
using BankSystem.Application.UseCases.ListCustomerAccounts;
using BankSystem.Application.UseCases.ListTransactionsForBankAccount;
using BankSystem.Application.UseCases.MakeDepositToBankAccount;
using BankSystem.Application.UseCases.MakeWithdrawalFromBankAccount;
using BankSystem.Application.UseCases.OpenCustomerAccount;
using BankSystem.Application.UseCases.UpdateCustomerAccount;
using BankSystem.Domain.Entities;
using BankSystem.Infrastructure.Persistence;
using BankSystem.Infrastructure.Repositories;
using BankSystem.Infrastructure.Services.AuditLogService;
using BankSystem.Infrastructure.Services.Authentication;
using BankSystem.Infrastructure.Services.Authenticationl;
using BankSystem.Infrastructure.Services.CurrencyService;
using BankSystem.Infrastructure.Services.CustomerAccountService;
using BankSystem.Infrastructure.Services.EmailService;
using BankSystem.Infrastructure.Services.Identity;
using BankSystem.Infrastructure.Services.IdentityService;
using BankSystem.Infrastructure.Services.PaginationService;
using BankSystem.Infrastructure.Services.TransactionService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<AppSettings>()
    .BindConfiguration(nameof(AppSettings))
    .ValidateDataAnnotations()
    .ValidateOnStart()
    .PostConfigure(options =>
    {
        // Set system culture from config
        CultureInfo systemCulture = new(options.SystemCulture);
        CultureInfo.DefaultThreadCurrentCulture = systemCulture;
        CultureInfo.DefaultThreadCurrentUICulture = systemCulture;
    });

builder.Services.AddOptions<DefaultAdmin>()
    .BindConfiguration(nameof(DefaultAdmin))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Registrera ApplicationDbContext i DI-containern
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services
  .AddIdentity<ApplicationUser, IdentityRole>()
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();

// Scoped services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPaginationService, PaginationService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IAuditLogger, AuditLogger>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ICustomerAccountService, CustomerAccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IEmailSender, FakeEmailSender>();
builder.Services.AddScoped<ICustomerAccountsRepository, CustomerAccountsRepository>();
builder.Services.AddScoped<IBankAccountsRepository, BankAccountsRepository>();

// Scoped Command/Query handlers
builder.Services.AddScoped<IHandler<GetCustomerAccountDetailsQuery, GetCustomerAccountDetailsResult>, GetCustomerAccountDetailsHandler>();
builder.Services.AddScoped<AuthenticateUserHandler>();
builder.Services.AddScoped<IHandler<OpenCustomerAccountCommand, OpenCustomerAccountResult>, OpenCustomerAccountHandler>();
builder.Services.AddScoped<IHandler<ListTransactionsForBankAccountQuery, ListTransactionsForBankAccountResult>, ListTransactionsForBankAccountHandler>();
builder.Services.AddScoped<IHandler<MakeDepositToBankAccountCommand, MakeDepositToBankAccountResult>, MakeDepositToBankAccountHandler>();
builder.Services.AddScoped<IHandler<MakeWithdrawalFromBankAccountCommand, MakeWithdrawalFromBankAccountResult>, MakeWithdrawalFromBankAccountHandler>();
builder.Services.AddScoped<IHandler<ListCustomerAccountsPageQuery, ListCustomerAccountsResult>, ListCustomerAccountsHandler>();
builder.Services.AddScoped<IHandler<UpdateCustomerAccountCommand, UpdateCustomerAccountResult>, UpdateCustomerAccountHandler>();
builder.Services.AddScoped<IHandler<CloseCustomerAccountCommand, CloseCustomerAccountResult>, CloseCustomerAccountHandler>();
builder.Services.AddScoped<IHandler<TransactionsPageQuery, BasePagedResult<Transaction>>, ListAllTransactionsHandler>();

// Jwt config
builder.Services.AddOptions<JwtOptions>()
    .BindConfiguration(nameof(JwtOptions))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwt = builder.Configuration
        .GetSection(nameof(JwtOptions))
        .Get<JwtOptions>()!;

    options.RequireHttpsMetadata = builder.Environment.IsProduction();
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwt.Issuer,
        ValidateAudience = true,
        ValidAudience = jwt.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt.Secret)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30),
        NameClaimType = JwtRegisteredClaimNames.Name,
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Policies.REQUIRE_ROLE_SYSTEM_ADMIN, policy => policy.RequireRole([
        Roles.SystemAdmin
    ]))
    .AddPolicy(Policies.REQUIRE_ROLE_CUSTOMER_SERVICE, policy =>
        policy.RequireRole([
            Roles.CustomerServiceRepresentative,
            Roles.SystemAdmin]))
    .AddDefaultPolicy(Policies.REQUIRE_AUTHENTICATION, policy =>
        policy.RequireAuthenticatedUser());

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Add documentation pages
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Apply any pending migrations
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await dbContext.Database.MigrateAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
    await CurrencySeeder.SeedAsync(scope.ServiceProvider);

    //// Generate/remove seeded data
    // const int SEED = 184765;
    // var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

    // await Seeder.GenerateSeededDataAsync(count: 10, seed: SEED, scope.ServiceProvider);
    // // await Seeder.RemoveSeededDataAsync(SEED, scope.ServiceProvider);

    // dbContext.SetTimestamps(false);
    // await unitOfWork.SaveAsync();
    // dbContext.SetTimestamps(true);
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

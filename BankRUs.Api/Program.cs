using BankRUs.Application;
using BankRUs.Application.BankAccounts;
using BankRUs.Application.Configuration;
using BankRUs.Application.Services.AuditLog;
using BankRUs.Application.Services.Authentication;
using BankRUs.Application.Services.Authentication.AuthenticateUser;
using BankRUs.Application.Services.CurrencyService;
using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.Services.Email;
using BankRUs.Application.Services.Identity;
using BankRUs.Application.Services.PaginationService;
using BankRUs.Application.Services.TransactionService;
using BankRUs.Application.UseCases.CustomerServiceRep.ListCustomerAccounts;
using BankRUs.Application.UseCases.GetBankAccountsForCustomer;
using BankRUs.Application.UseCases.GetCustomerAccountDetails;
using BankRUs.Application.UseCases.ListTransactionsForBankAccount;
using BankRUs.Application.UseCases.MakeDepositToBankAccount;
using BankRUs.Application.UseCases.MakeWithdrawalFromBankAccount;
using BankRUs.Application.UseCases.OpenAccount;
using BankRUs.Application.UseCases.UpdateCustomerAccount;
using BankRUs.Infrastructure.Persistence;
using BankRUs.Infrastructure.Repositories;
using BankRUs.Infrastructure.Services.AuditLogService;
using BankRUs.Infrastructure.Services.Authentication;
using BankRUs.Infrastructure.Services.Authenticationl;
using BankRUs.Infrastructure.Services.CurrencyService;
using BankRUs.Infrastructure.Services.CustomerService;
using BankRUs.Infrastructure.Services.Email;
using BankRUs.Infrastructure.Services.Identity;
using BankRUs.Infrastructure.Services.IdentityService;
using BankRUs.Infrastructure.Services.PaginationService;
using BankRUs.Infrastructure.Services.TransactionService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

//builder.Services.AddSingleton<CurrencyValueGenerator>();
//await CurrencySeeder


// Scoped services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPaginationService, PaginationService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IAuditLogger, AuditLogger>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IEmailSender, FakeEmailSender>();
builder.Services.AddScoped<IBankAccountsRepository, BankAccountsRepository>();

// Scoped Command/Query handlers
builder.Services.AddScoped<IHandler<GetCustomerAccountDetailsQuery, GetCustomerAccountDetailsResult>, GetCustomerAccountDetailsHandler>();
builder.Services.AddScoped<AuthenticateUserHandler>();
builder.Services.AddScoped<IHandler<OpenCustomerAccountCommand, OpenCustomerAccountResponseDto>, OpenCustomerAccountHandler>();
builder.Services.AddScoped<GetBankAccountsForCustomerHandler>();
builder.Services.AddScoped<IHandler<ListTransactionsForBankAccountQuery, ListTransactionsForBankAccountResult>, ListTransactionsForBankAccountHandler>();
builder.Services.AddScoped<IHandler<MakeDepositToBankAccountCommand, MakeDepositToBankAccountResult>, MakeDepositToBankAccountHandler>();
builder.Services.AddScoped<IHandler<MakeWithdrawalFromBankAccountCommand, MakeWithdrawalFromBankAccountResult>, MakeWithdrawalFromBankAccountHandler>();
builder.Services.AddScoped<IHandler<ListCustomerAccountsQuery, ListCustomerAccountsResult>, ListCustomerAccountsHandler>();
builder.Services.AddScoped<IHandler<UpdateCustomerAccountCommand, UpdateCustomerAccountResult>, UpdateCustomerAccountHandler>();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    dbContext.Database.Migrate();

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

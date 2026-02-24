using BankRUs.Application.BankAccounts;
using BankRUs.Application.GuardClause;
using BankRUs.Application.Repositories;
using BankRUs.Application.Services.CurrencyService;
using BankRUs.Application.Services.CustomerAccountService;
using BankRUs.Application.Services.EmailService;
using BankRUs.Application.Services.Identity;
using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.UseCases.OpenCustomerAccount;

public class OpenCustomerAccountHandler(
    IUnitOfWork unitOfWork,
    ICurrencyService currencyService,
    ICustomerAccountsRepository customerAccountRepository,
    IBankAccountsRepository bankAccountsRepository,
    IIdentityService identityService,
    ICustomerAccountService customerService,
    IEmailSender emailSender) : IHandler<OpenCustomerAccountCommand, OpenCustomerAccountResult>
{
    // ToDo: Add ILoggerService?
    //private readonly ILoggerService<OpenCustomerAccountHandler> _logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    private readonly ICurrencyService _currencyService = currencyService;
    
    private readonly ICustomerAccountsRepository _customerAccountRepository = customerAccountRepository;

    private readonly IBankAccountsRepository _bankAccountRepository = bankAccountsRepository;
    
    private readonly ICustomerAccountService _customerService = customerService;
    
    private readonly IIdentityService _identityService = identityService;
    
    private readonly IEmailSender _emailSender = emailSender;

    public async Task<OpenCustomerAccountResult> HandleAsync(OpenCustomerAccountCommand command)
    {
        // A Customer Account can be opened if...

        // 1) There is no Customer with the same Email
        var sanitizedEmail = Guard.Against.DuplicateCustomer(command.Email, _customerService.EmailExists);

        // 2) There is no Customer with the same SSN
        var sanitizedSocialSecurityNumber = Guard.Against.DuplicateCustomer(command.SocialSecurityNumber, _customerService.SsnExists);

        // Create the Customer details object
        var customerDetails = _customerService.ValidateCustomerAccountDetails(new CustomerAccountDetails(
            firstName: command.FirstName,
            lastName: command.LastName,
            email: command.Email,
            socialSecurityNumber: command.SocialSecurityNumber));

        // Create new ApplicationUser
        var createApplicationUserResult = await _identityService.CreateApplicationUserAsync(new CreateApplicationUserRequest(
            FirstName: command.FirstName,
            LastName: command.LastName,
            Email: command.Email,
            Password: command.Password
         )) ?? throw new Exception("Could not create application user"); ;

        // Create new Customer account
        var customerAccount = new CustomerAccount(createApplicationUserResult.UserId, command.SocialSecurityNumber);


        // Create default Bank account
        var defaultBankAccount = new BankAccount
        {
            Name = "Default Checking Account",
            CustomerId = customerAccount.Id,
            // ToDo: remove hard-coded ISO symbol
            Currency = _currencyService.ParseIsoSymbol("SEK")
        };

        await _bankAccountRepository.AddAsync(defaultBankAccount);
        
        await _customerService.OpenCustomerAccountAsync(new OpenCustomerAccountRequest(
            CustomerAccount: customerAccount,
            CustomerAccountDetails: customerDetails,
            DefaultBankAccount: defaultBankAccount,
            ApplicationUserId: createApplicationUserResult.UserId));

        await _customerAccountRepository.AddCustomerAccountAsync(customerAccount);
        
        // Send confirmation email to customer
        var sendEmailRequest = new OpenCustomerAccountConfirmationEmail(
            to: command.Email,
            from: "your.bank@example.com",
            body: "ToDo: add welcome message with confirmation link");

        await _emailSender.SendEmailAsync(sendEmailRequest);


        // Complete unit of work
        await _unitOfWork.SaveAsync();
        return new OpenCustomerAccountResult(CustomerAccountId: customerAccount.Id);
    }
}
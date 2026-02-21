using BankRUs.Application.GuardClause;
using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.Services.EmailService;
using BankRUs.Application.Services.Identity;
using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.UseCases.OpenCustomerAccount;

public class OpenCustomerAccountHandler(
    IUnitOfWork unitOfWork,
    IIdentityService identityService,
    ICustomerService customerService,
    IEmailSender emailSender) : IHandler<OpenCustomerAccountCommand, OpenCustomerAccountResult>
{
    // ToDo: Add ILoggerService?
    //private readonly ILoggerService<OpenCustomerAccountHandler> _logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IIdentityService _identityService = identityService;
    private readonly ICustomerService _customerService = customerService;
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

        // Create new Customer
        var createCustomerResult = await _customerService.CreateCustomerAsync(new CreateCustomerAccountRequest(
            ApplicationUserId: createApplicationUserResult.UserId, 
            SocialSecurityNumber: command.SocialSecurityNumber));

        var customerAccount = createCustomerResult.Customer;

        await _customerService.OpenCustomerAccountAsync(new OpenCustomerAccountRequest(
            CustomerAccount: createCustomerResult.Customer,
            CustomerAccountDetails: customerDetails,
            ApplicationUserId: createApplicationUserResult.UserId));

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
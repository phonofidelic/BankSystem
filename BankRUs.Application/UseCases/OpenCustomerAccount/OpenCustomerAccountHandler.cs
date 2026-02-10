using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.Services.Email;
using BankRUs.Application.Services.Identity;

namespace BankRUs.Application.UseCases.OpenAccount;

public class OpenCustomerAccountHandler(
    IUnitOfWork unitOfWork,
    IIdentityService identityService,
    ICustomerService customerService,
    IEmailSender emailSender) : IHandler<OpenCustomerAccountCommand, OpenCustomerAccountResponseDto>
{
    // ToDo: Add ILoggerService?
    //private readonly ILoggerService<OpenCustomerAccountHandler> _logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IIdentityService _identityService = identityService;
    private readonly ICustomerService _customerService = customerService;
    private readonly IEmailSender _emailSender = emailSender;

    public async Task<OpenCustomerAccountResponseDto> HandleAsync(OpenCustomerAccountCommand command)
    {
        // ToDo: Validate business rules for Customer Account Creation

        // Create new Customer
        var createCustomerResult = await _customerService.CreateCustomerAsync(new CreateCustomerRequest(
            SocialSecurityNumber: command.SocialSecurityNumber,
            Email: command.Email));


        // Create new ApplicationUser
        var createApplicationUserResult = await _identityService.CreateApplicationUserAsync(new CreateApplicationUserRequest(
            FirstName: command.FirstName,
            LastName: command.LastName,
            Email: command.Email,
            Password: command.Password
         ));

        var createdCustomer = createCustomerResult.Customer;

        // Add ApplicationUserId to the Customer
        createdCustomer.SetApplicationUserId(createApplicationUserResult.UserId);

        // Create default bank account for new Customer
        var createdDefaultBankAccountResult = await _customerService.CreateBankAccountAsync(new CreateBankAccountRequest(
            CustomerId: createCustomerResult.Customer.Id,
            BankAccountName: "Default Checking Account"));

        try
        {
            // Send confirmation email to customer
            var sendEmailRequest = new SendEmailRequest(
                To: command.Email,
                From: "your.bank@example.com",
                Subject: "Välkommen till BankAB!",
                Body: "Ditt bankkonto är nu redo!");

             await _emailSender.SendEmailAsync(sendEmailRequest);
        } catch
        {
            // ToDo: Log email sender error
        }

        // Complete unit of work
        await _unitOfWork.SaveAsync();
        return new OpenCustomerAccountResponseDto(UserId: createApplicationUserResult.UserId);
    }
}
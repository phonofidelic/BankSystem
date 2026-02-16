using BankRUs.Application.Exceptions;
using BankRUs.Application.GuardClause;
using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.UseCases.OpenCustomerAccount;

namespace BankRUs.Application.UseCases.UpdateCustomerAccount;

public class UpdateCustomerAccountHandler(
    IUnitOfWork unitOfWork,
    ICustomerService customerService): IHandler<UpdateCustomerAccountCommand, UpdateCustomerAccountResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICustomerService _customerService = customerService;

    public async Task<UpdateCustomerAccountResult> HandleAsync(UpdateCustomerAccountCommand command)
    {
        // A Customer account can be updated if...

        // 1) The Customer exists in the system
        var customerAccount = await _customerService.GetCustomerAsync(command.CustomerAccountId)
            ?? throw new CustomerNotFoundException();

        // 2) If the Email is new, it must be unique in the system
        if (command.Details.Email != null) 
            Guard.Against.DuplicateCustomer(command.Details.Email, _customerService.EmailExists);

        // 3) If the SSN is new, it must be unique in the system
        if (command.Details.SocialSecurityNumber != null)
            Guard.Against.DuplicateCustomer(command.Details.SocialSecurityNumber, _customerService.SsnExists);

        // 4) ToDo: The Customer has confirmed the change
        //    (implementation: by visiting a link sent in the confirmation email?)

        // If validation is successful
        customerAccount.UpdateAccountDetails(command.Details);

        await _unitOfWork.SaveAsync();

        return new UpdateCustomerAccountResult(command.Details.Fields);
    }
}

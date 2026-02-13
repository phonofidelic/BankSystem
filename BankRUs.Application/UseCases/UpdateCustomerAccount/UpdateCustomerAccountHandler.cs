using BankRUs.Application.Exceptions;
using BankRUs.Application.GuardClause;
using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.UseCases.OpenCustomerAccount;
using BankRUs.Domain.ValueObjects;

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
        var acceptedEmail = command.Email != null && command.Email != customerAccount.Email
            ? Guard.Against.DuplicateCustomer(command.Email, _customerService.EmailExists)
            : null;

        // 3) If the SSN is new, it must be unique in the system

        // 4) The Customer has confirmed the change
        //    (implementation: by visiting a link sent in the confirmation email?)

        var customerAccountDetails = new CustomerAccountDetails(
            firstName: command.FirstName,
            lastName: command.LastName,
            email: acceptedEmail,
            socialSecurityNumber: command.SocialSecurityNumber);

        // If validation is successful
        customerAccount.Update(customerAccountDetails);

        await _unitOfWork.SaveAsync();

        return new UpdateCustomerAccountResult(customerAccountDetails.Fields);
    }
}

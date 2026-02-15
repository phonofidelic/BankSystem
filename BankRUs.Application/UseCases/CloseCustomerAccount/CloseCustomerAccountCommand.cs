using System;

namespace BankRUs.Application.UseCases.CloseCustomerAccount;

public record CloseCustomerAccountCommand(Guid CustomerAccountId);

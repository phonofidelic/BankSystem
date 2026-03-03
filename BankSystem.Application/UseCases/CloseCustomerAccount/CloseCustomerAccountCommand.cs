using System;

namespace BankSystem.Application.UseCases.CloseCustomerAccount;

public record CloseCustomerAccountCommand(Guid CustomerAccountId);

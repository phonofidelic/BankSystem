using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.UseCases.MakeWithdrawalFromBankAccount;

public record MakeWithdrawalFromBankAccountCommand(Guid CustomerId,
        Guid BankAccountId,
        decimal Amount,
        string Currency,
        string? Reference);

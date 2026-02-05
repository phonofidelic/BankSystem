namespace BankRUs.Application.UseCases.MakeWithdrawalFromBankAccount;

public class MakeWithdrawalFromBankAccountHandler : IHandler<MakeWithdrawalFromBankAccountCommand, MakeWithdrawalFromBankAccountResult>
{
    public Task<MakeWithdrawalFromBankAccountResult> HandleAsync(MakeWithdrawalFromBankAccountCommand request)
    {
        // A withdrawal can be made from a Bank Account if...

        // 1) The Bank Account exists

        // 2) The Customer owns the Bank Account

        // 3) The Deposit Amount is a positive decimal

        // 4) The Deposit Reference message has no more than 140 characters
        
        // 5) The current balance covers the withdrawal amount

        throw new NotImplementedException();
    }
}

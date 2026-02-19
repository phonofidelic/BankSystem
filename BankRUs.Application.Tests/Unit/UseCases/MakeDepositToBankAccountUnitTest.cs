using BankRUs.Application.Tests.Infrastructure.Stubs;
using BankRUs.Application.UseCases.MakeDepositToBankAccount;

namespace BankRUs.Application.Tests.Unit.UseCases;

public class MakeDepositToBankAccountUnitTest
{
    [Fact]
    public async Task MakeDepositToBankAccountHandler_WhenBankAccountExists_ShouldReturnDepositResult()
    {
        var makeDepositHandler = new MakeDepositToBankAccountHandler(
            unitOfWork: new UnitOfWorkStub(),
            bankAccountsRepository: new BankAccountRepositoryStub(),
            currencyService: new CurrencyServiceStub(),
            transactionService: new TransactionServiceStub(),
            auditLogger: new AuditLoggerStub()
        );

        var makeDepositCommand = new MakeDepositToBankAccountCommand(
            CustomerId: Guid.NewGuid(),
            BankAccountId: Guid.NewGuid(),
            Amount: (decimal)100.00,
            Currency: "SEK",
            Reference: "Test make deposit transaction"
        );

        // Act:
        var makeDepositResult = await makeDepositHandler.HandleAsync(makeDepositCommand);

        // Assert:
        Assert.NotNull(makeDepositResult);
    }
}

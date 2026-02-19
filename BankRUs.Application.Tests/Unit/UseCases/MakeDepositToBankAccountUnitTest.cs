using BankRUs.Application.Tests.Infrastructure.Stubs;
using BankRUs.Application.UseCases.MakeDepositToBankAccount;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.Tests.Unit.UseCases;

public class MakeDepositToBankAccountUnitTest
{
    [Fact]
    public async Task MakeDepositToBankAccountHandler_WhenBankAccountExists_ShouldReturnResult()
    {
        var customerId = Guid.NewGuid();
        var bankAccountId = Guid.NewGuid();

        var makeDepositHandler = new MakeDepositToBankAccountHandler(
            unitOfWork: new UnitOfWorkStub(),
            bankAccountsRepository: new BankAccountRepositoryStub(customerId),
            currencyService: new CurrencyServiceStub(),
            transactionService: new TransactionServiceStub(),
            auditLogger: new AuditLoggerStub()
        );

        var makeDepositCommand = new MakeDepositToBankAccountCommand(
            CustomerId: customerId,
            BankAccountId: bankAccountId,
            Amount: (decimal)100.25111,
            Currency: "SEK",
            Reference: "Test make deposit transaction"
        );

        // Act:
        var makeDepositResult = await makeDepositHandler.HandleAsync(makeDepositCommand);

        // Assert:
        Assert.NotNull(makeDepositResult);
        Assert.Equal(customerId, makeDepositResult.CustomerId);
        Assert.Equal(TransactionType.Deposit, makeDepositResult.Type);
        Assert.Equal((decimal)100.25, makeDepositResult.Amount);
        Assert.Equal("SEK", makeDepositCommand.Currency);
        Assert.Equal("Test make deposit transaction", makeDepositResult.Reference);
    }
}

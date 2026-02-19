using BankRUs.Application.Exceptions;
using BankRUs.Application.Tests.Infrastructure.Stubs;
using BankRUs.Application.UseCases.MakeDepositToBankAccount;
using BankRUs.Domain.Entities;

namespace BankRUs.Application.Tests.Unit.UseCases;

public class MakeDepositToBankAccountUnitTest
{
    [Fact]
    public async Task MakeDepositToBankAccountHandler_WhenBankAccountExists_ShouldReturnResult()
    {
        // Arrange:
        var customerId = Guid.NewGuid();
        var bankAccountId = Guid.NewGuid();

        var makeDepositHandler = new MakeDepositToBankAccountHandler(
            unitOfWork: new UnitOfWorkStub(),
            bankAccountsRepository: new BankAccountRepositoryStub(customerId),
            currencyService: new CurrencyServiceStub(),
            transactionService: new TransactionServiceStub(),
            auditLogger: new AuditLoggerStub());

        var makeDepositCommand = new MakeDepositToBankAccountCommand(
            CustomerId: customerId,
            BankAccountId: bankAccountId,
            Amount: (decimal)100.25111,
            Currency: "SEK",
            Reference: "Test make deposit transaction");

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

    [Fact]
    public async Task MakeDepositToBankAccountHandler_WhenBankAccountNotFound_ShouldThrowNotFound()
    {
        // Arrange:
        var customerId = Guid.NewGuid();

        var makeDepositHandler = new MakeDepositToBankAccountHandler(
            unitOfWork: new UnitOfWorkStub(),
            bankAccountsRepository: new BankAccountRepositoryStub(customerId),
            currencyService: new CurrencyServiceStub(),
            transactionService: new TransactionServiceStub(),
            auditLogger: new AuditLoggerStub());

        var makeDepositCommand = new MakeDepositToBankAccountCommand(
            CustomerId: customerId,
            BankAccountId: Guid.NewGuid(),
            Amount: (decimal)100.25111,
            Currency: "SEK",
            Reference: "Test bank account not found deposit transaction");

        try
        {
            // Act:
            await makeDepositHandler.HandleAsync(makeDepositCommand);
        }
        catch (Exception ex) {
            // Assert:
            Assert.IsType<BankAccountNotFoundException>(ex);
        }
    }

    [Fact]
    public async Task MakeDepositToBankAccountHandler_WhenBankAccountNotOwned_ShouldThrowNotOwned()
    {
        // Arrange:
        var makeDepositHandler = new MakeDepositToBankAccountHandler(
            unitOfWork: new UnitOfWorkStub(),
            bankAccountsRepository: new BankAccountRepositoryStub(Guid.NewGuid()),
            currencyService: new CurrencyServiceStub(),
            transactionService: new TransactionServiceStub(),
            auditLogger: new AuditLoggerStub());

        var makeDepositCommand = new MakeDepositToBankAccountCommand(
            CustomerId: Guid.NewGuid(),
            BankAccountId: Guid.NewGuid(),
            Amount: (decimal)100.25111,
            Currency: "SEK",
            Reference: "Test bank account not found deposit transaction");

        try
        {
            // Act:
            await makeDepositHandler.HandleAsync(makeDepositCommand);
        }
        catch (Exception ex)
        {
            // Assert:
            Assert.IsType<BankAccountNotOwnedException>(ex);
        }
    }

    [Fact]
    public async Task MakeDepositToBankAccountHandler_WhenCurrencyDoesNotMatchBankAccount_ShouldThrowUnsupportedCurrency()
    {
        // Arrange:
        var customerId = Guid.NewGuid();

        var makeDepositHandler = new MakeDepositToBankAccountHandler(
            unitOfWork: new UnitOfWorkStub(),
            bankAccountsRepository: new BankAccountRepositoryStub(customerId),
            currencyService: new CurrencyServiceStub(),
            transactionService: new TransactionServiceStub(),
            auditLogger: new AuditLoggerStub());

        var makeDepositCommand = new MakeDepositToBankAccountCommand(
            CustomerId: customerId,
            BankAccountId: Guid.NewGuid(),
            Amount: (decimal)100.25111,
            Currency: "###",
            Reference: "Test bank account unsupported currency");

        try
        {
            // Act:
            await makeDepositHandler.HandleAsync(makeDepositCommand);
        } catch (Exception ex)
        {
            // Assert:
            Assert.IsType<BankAccountUnsupportedCurrencyException>(ex);
        }
    }
}
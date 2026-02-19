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
            Reference: "Test deposit transaction");

        // Act:
        var makeDepositResult = await makeDepositHandler.HandleAsync(makeDepositCommand);

        // Assert:
        Assert.NotNull(makeDepositResult);
        Assert.Equal(customerId, makeDepositResult.CustomerId);
        Assert.Equal(TransactionType.Deposit, makeDepositResult.Type);
        Assert.Equal((decimal)100.25, makeDepositResult.Amount);
        Assert.Equal("SEK", makeDepositCommand.Currency);
        Assert.Equal("Test deposit transaction", makeDepositResult.Reference);
    }

    [Fact]
    public async Task MakeDepositToBankAccountHandler_WhenBankAccountNotFound_ShouldThrowBankAccountNotFoundException()
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
            Reference: "Test deposit transaction, bank account not found");

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
    public async Task MakeDepositToBankAccountHandler_WhenBankAccountNotOwned_ShouldThrowBankAccountNotOwnedException()
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
            Reference: "Test deposit transaction, bank account not owned");

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
    public async Task MakeDepositToBankAccountHandler_WhenCurrencyDoesNotMatchBankAccount_ShouldThrowBankAccountUnsupportedCurrencyException()
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
            Reference: "Test deposit transaction, bank account unsupported currency");

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

    [Theory]
    [InlineData(-10)]
    [InlineData(0)]
    public async Task MakeDepositToBankAccountHandler_WhenTransactionAmountIsNotPositive_ShouldThrowBankAccountTransactionException(decimal amount)
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
            Amount: (decimal)amount,
            Currency: "SEK",
            Reference: "Test deposit transaction, negative amount");

        try
        {
            // Act:
            await makeDepositHandler.HandleAsync(makeDepositCommand);
        } catch (Exception ex)
        {
            // Assert:
            Assert.IsType<BankAccountTransactionException>(ex);
        }
    }
}
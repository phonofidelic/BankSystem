using BankRUs.Application.BankAccounts;
using BankRUs.Application.Exceptions;
using BankRUs.Application.GuardClause;
using BankRUs.Application.Guards;
using BankRUs.Application.Services.PaginationService;
using BankRUs.Application.Services.TransactionService;

namespace BankRUs.Application.UseCases.ListTransactionsForBankAccount;

public class ListTransactionsForBankAccountHandler(
    IBankAccountsRepository bankAccountsRepository,
    IPaginationService paginationService,
    ITransactionService transactionService) : IHandler<ListTransactionsForBankAccountQuery, ListTransactionsForBankAccountResult>
{
    private readonly IBankAccountsRepository _bankAccountRepository = bankAccountsRepository;
    private readonly IPaginationService _paginationService = paginationService;
    private readonly ITransactionService _transactionService = transactionService;
    public async Task<ListTransactionsForBankAccountResult> HandleAsync(ListTransactionsForBankAccountQuery query)
    {
        // Transactions for a Bank Account can be shown to a Customer if...

        // 1) The Bank Account exists
        bool bankAccountexists = _bankAccountRepository.BankAccountExists(query.BankAccountId);
        if (!bankAccountexists) throw new BankAccountNotFoundException();

        // 2) The Customer owns the Bank Account
        var bankAccountOwnerId = await _bankAccountRepository.GetCustomerAccountIdForBankAccountAsync(query.BankAccountId);
        Guard.Against.BankAccountNotOwned(bankAccountOwnerId, query.CustomerId);
        var bankAccount = await _bankAccountRepository.GetBankAccountAsync(query.BankAccountId);

        var transactionsQuery = new TransactionsPageQuery(
            Search: null,
            BankAccountId: query.BankAccountId,
            StartPeriodUtc: query.StartPeriodUtc,
            EndPeriodUtc: query.EndPeriodUdc,
            Type: query.Type,
            Page: query.Page,
            PageSize: query.PageSize,
            SortOrder: query.SortOrder ?? SortOrder.Descending);

        var queryResult = await _transactionService.GetTransactionsAsync(transactionsQuery);

        var paginationResult = await _paginationService.GetPagedResultAsync(transactionsQuery, queryResult);

        return new ListTransactionsForBankAccountResult(
            BankAccountId: bankAccount.Id,
            Currency: bankAccount.Currency,
            CurrentBalance: bankAccount.Balance,
            QueryResult: paginationResult);
    }
}

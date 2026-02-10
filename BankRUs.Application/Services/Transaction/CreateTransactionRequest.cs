using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;

namespace BankRUs.Application.Services.TransactionService;

public record CreateTransactionRequest(
    Guid CustomerId,
    Guid BankAccountId,
    decimal Amount,
    TransactionType Type,
    Currency Currency,
    string? Reference);

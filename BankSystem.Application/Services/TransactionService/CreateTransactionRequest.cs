using BankSystem.Domain.Entities;
using BankSystem.Domain.ValueObjects;

namespace BankSystem.Application.Services.TransactionService;

public record CreateTransactionRequest(
    Guid CustomerId,
    Guid BankAccountId,
    decimal Amount,
    TransactionType Type,
    Currency Currency,
    string? Reference);

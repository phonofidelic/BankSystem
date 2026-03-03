using System;
using BankSystem.Application.Services.PaginationService;
using BankSystem.Domain.Entities;

namespace BankSystem.Api.Dtos.Transactions;

public record GetTransactionsQueryDto(
    TransactionType? Type
) :BasePageQuery;

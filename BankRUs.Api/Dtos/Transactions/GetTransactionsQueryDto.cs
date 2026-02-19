using System;
using BankRUs.Application.Services.PaginationService;
using BankRUs.Domain.Entities;

namespace BankRUs.Api.Dtos.Transactions;

public record GetTransactionsQueryDto(
    TransactionType? Type
) :BasePageQuery;

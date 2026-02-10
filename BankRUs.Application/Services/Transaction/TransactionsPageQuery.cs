using BankRUs.Application.Paginatioin;
using BankRUs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Services.TransactionService;

public record TransactionsPageQuery(
    Guid? BankAccountId,
    DateTime? StartPeriodUtc,
    DateTime? EndPeriodUtc,
    TransactionType? Type,
    int Page,
    int PageSize,
    SortOrder SortOrder) : BasePageQuery(Page, PageSize, SortOrder);

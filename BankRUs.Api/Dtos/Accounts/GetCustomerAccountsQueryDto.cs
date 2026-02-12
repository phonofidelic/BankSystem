using BankRUs.Application.Services.PaginationService;

namespace BankRUs.Api.Dtos.Accounts;

public record GetCustomerAccountsQueryDto(string? Search) : BasePageQuery;

using BankRUs.Api.Dtos.Transactions;
using BankRUs.Application;
using BankRUs.Application.Services.PaginationService;
using BankRUs.Application.Services.TransactionService;
using BankRUs.Domain.Entities;
using BankRUs.Infrastructure.Services.IdentityService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankRUs.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = Policies.REQUIRE_ROLE_CUSTOMER_SERVICE)]

public class TransactionsController(
    IHandler<TransactionsPageQuery, BasePagedResult<Transaction>> listAllTransactionsHandler) : ControllerBase
{
    private readonly IHandler<TransactionsPageQuery, BasePagedResult<Transaction>> _listAllTransactionsHandler = listAllTransactionsHandler;

    [HttpGet()]
    [Produces("application/json")]
    [ProducesResponseType<GetTransactionsResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllTransactions([FromQuery] TransactionsPageQuery query)
    {
        var result = await _listAllTransactionsHandler.HandleAsync(query);
        var listItems = result.Items.Select(item => new TransactionsListItemDto(
            TransactionId: item.Id,
            Type: item.Type.ToString(),
            Amount: item.Amount,
            CreatedAt: item.CreatedAt,
            BalanceAfter: item.BalanceAfter,
            Currency: item.Currency.ToString(),
            Reference: item.Reference)).ToList();

        return Ok(new GetTransactionsResponseDto(
            Paging: result.Paging,
            Items: listItems
        ));
    }
}
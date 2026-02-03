using BankRUs.Api.Dtos.BankAccounts;
using BankRUs.Application;
using BankRUs.Application.UseCases.MakeDepositToBankAccount;
using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace BankRUs.Api.Controllers;

[Route("api/bank-accounts")]
[ApiController]
public class BankAccountsController(
    IHandler<MakeDepositToBankAccountCommand, MakeDepositToBankAccountResult> makeDepositToBankAccountHandler) : ControllerBase
{
    private readonly IHandler<MakeDepositToBankAccountCommand, MakeDepositToBankAccountResult> _makeDepositToBankAccountHandler = makeDepositToBankAccountHandler;

    // GET /api/bank-accounts

    // GET /api/bank-accounts/{bankAccountId}

    // POST /api/bank-accounts/{bankAccountId}/deposits
    [HttpPost("{id}/deposits")]
    public async Task<IActionResult> Get(
        [FromRoute] string id,
        [FromBody] PostDepositRequestDto request)
    {
        if (!Guid.TryParse(id, out Guid bankAccountId))
        {
            return NotFound();
        }

        try
        {
            MakeDepositToBankAccountResult result = await _makeDepositToBankAccountHandler.HandleAsync(new MakeDepositToBankAccountCommand(
                CustomerId: Guid.Parse("65840c74-674f-43e7-b3a8-8d0634cb6b4a"),
                BankAccountId: bankAccountId,
                Amount: request.Amount,
                Currency: request.ISO_Currency_Symbol,
                Reference: request.Reference));

            return Created(string.Empty, new PostDepositResponseDto(
                TransactionId: result.TransactionId,
                CustomerId: result.CustomerId,
                Type: result.Type,
                Amount: result.Amount,
                Currency: result.Currency,
                Reference: result.Reference ?? "",
                CreatedAt: result.CreatedAt,
                BalanceAfter: 100));
        }
        catch (Exception ex) { 
            return BadRequest(ex.Message);
        }
    }
}

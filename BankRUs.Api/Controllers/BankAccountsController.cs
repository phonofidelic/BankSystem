using BankRUs.Api.Dtos.BankAccounts;
using BankRUs.Application;
using BankRUs.Application.Services.AuditLog;
using BankRUs.Application.UseCases.MakeDepositToBankAccount;
using BankRUs.Application.UseCases.MakeDepositToBankAccount.Exceptions;
using BankRUs.Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankRUs.Api.Controllers;

[Route("api/bank-accounts")]
[Authorize(Roles = Roles.Customer)]
[ApiController]
public class BankAccountsController(
    IHandler<MakeDepositToBankAccountCommand, MakeDepositToBankAccountResult> makeDepositToBankAccountHandler,
    ILogger<BankAccountsController> logger,
    IAuditLogger auditLogger) : ControllerBase
{
    private readonly IHandler<MakeDepositToBankAccountCommand, MakeDepositToBankAccountResult> _makeDepositToBankAccountHandler = makeDepositToBankAccountHandler;
    private readonly ILogger<BankAccountsController> _logger = logger;
    private readonly IAuditLogger _auditLogger = auditLogger;

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
                Type: result.Type.ToString().ToLower(),
                Amount: result.Amount,
                Currency: result.Currency,
                Reference: result.Reference ?? "",
                CreatedAt: result.CreatedAt,
                BalanceAfter: 100,
                AuditLog: _auditLogger.GetAuditLogs()));
        }
        catch (Exception ex) {
            EventId eventId = new();
            _logger.LogError(eventId, ex, message: ex.Message);

            if (ex.GetType() == typeof(TransactionReferenceException))
            {
                ModelState.AddModelError("Reference", ex.Message);
                return BadRequest(ModelState);
            }

            return BadRequest();
        }
    }
}

using BankRUs.Api.Dtos.BankAccounts;
using BankRUs.Application;
using BankRUs.Application.Exceptions;
using BankRUs.Application.Services.AuditLog;
using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.Services.CustomerService.GetCustomer;
using BankRUs.Application.UseCases.MakeDepositToBankAccount;
using BankRUs.Application.UseCases.MakeWithdrawalFromBankAccount;
using BankRUs.Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankRUs.Api.Controllers;

[Route("api/bank-accounts")]
[Authorize(Roles = Roles.Customer)]
[ApiController]
public class BankAccountsController(
    ICustomerService customerService,
    IHandler<MakeDepositToBankAccountCommand, MakeDepositToBankAccountResult> makeDepositToBankAccountHandler,
    IHandler<MakeWithdrawalFromBankAccountCommand, MakeWithdrawalFromBankAccountResult> makeWithdrawalFromBankAccountHandler,
    ILogger<BankAccountsController> logger,
    IAuditLogger auditLogger) : ControllerBase
{
    private readonly ICustomerService _customerService = customerService;
    private readonly IHandler<MakeDepositToBankAccountCommand, MakeDepositToBankAccountResult> _makeDepositToBankAccountHandler = makeDepositToBankAccountHandler;
    private readonly IHandler<MakeWithdrawalFromBankAccountCommand, MakeWithdrawalFromBankAccountResult> _makeWithdrawalFromBankAccountHandler = makeWithdrawalFromBankAccountHandler;
    private readonly ILogger<BankAccountsController> _logger = logger;
    private readonly IAuditLogger _auditLogger = auditLogger;

    // GET /api/bank-accounts

    // GET /api/bank-accounts/{bankAccountId}

    // POST /api/bank-accounts/{bankAccountId}/deposits
    [HttpPost("{id}/deposits")]
    public async Task<IActionResult> PostDeposit(
        [FromRoute] string id,
        [FromBody] PostDepositRequestDto request)
    {
        if (!Guid.TryParse(id, out Guid bankAccountId))
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null) {
            return Unauthorized();
        }

        if (!Guid.TryParse(userId, out Guid userGuid))
        {
            return NotFound();
        }

        try
        {
            var getCustomerIdResult = await _customerService.GetCustomerIdAsync(new GetCustomerIdRequest(userGuid));
            
            var result = await _makeDepositToBankAccountHandler.HandleAsync(new MakeDepositToBankAccountCommand(
                CustomerId: getCustomerIdResult.CustomerId,
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
                Reference: result.Reference,
                CreatedAt: result.CreatedAt,
                BalanceAfter: result.BalanceAfter,
                AuditLog: _auditLogger.GetAuditLogs()));
        }
        catch (Exception ex) {
            EventId eventId = new();
            _logger.LogError(eventId, ex, message: ex.Message);

            if (ex is NotFoundException)
            {
                return NotFound(ex.Message);
            }

            if (ex is BadRequestException)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest();
        }
    }

    // POST /api/bank-accounts/{bankAccountId}/withdrawals
    [HttpPost("{id}/withdrawals")]
    public async Task<IActionResult> PostWithdrawal(
        [FromRoute] string id,
        [FromBody] PostWithdrawalRequestDto request)
    {
        if (!Guid.TryParse(id, out Guid bankAccountId))
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized();
        }

        if (!Guid.TryParse(userId, out Guid userGuid))
        {
            return NotFound();
        }

        try
        {
            var getCustomerIdResult = await _customerService.GetCustomerIdAsync(new GetCustomerIdRequest(userGuid));

            var result = await _makeWithdrawalFromBankAccountHandler.HandleAsync(new MakeWithdrawalFromBankAccountCommand(
                CustomerId: getCustomerIdResult.CustomerId,
                BankAccountId: bankAccountId,
                Amount: request.Amount,
                Currency: request.ISO_Currency_Symbol,
                Reference: request.Reference));

            return Created(string.Empty, new PostWithdrawalResponseDto(
                TransactionId: result.TransactionId,
                CustomerId: result.CustomerId,
                Type: result.Type.ToString().ToLower(),
                Amount: result.Amount,
                Currency: result.Currency,
                Reference: result.Reference,
                CreatedAt: result.CreatedAt,
                BalanceAfter: result.BalanceAfter,
                AuditLog: _auditLogger.GetAuditLogs()));
        }
        catch (Exception ex) {
            EventId eventId = new();
            _logger.LogError(eventId, ex, message: ex.Message);

            if (ex is NotFoundException)
            {
                return NotFound(ex.Message);
            }

            if (ex is BadRequestException)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest();
        }
    }
}
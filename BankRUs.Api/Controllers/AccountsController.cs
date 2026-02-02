using BankRUs.Api.Dtos.Accounts;
using BankRUs.Api.Dtos.BankAccounts;
using BankRUs.Application.UseCases.GetBankAccountsForCustomer;
using BankRUs.Application.UseCases.OpenAccount;
using BankRUs.Application.UseCases.OpenAccount.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankRUs.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly ILogger<AccountsController> _logger;
    private readonly OpenCustomerAccountHandler _openAccountHandler;
    private readonly GetBankAccountsForCustomerHandler _getBankAccountsForCustomerHandler;

    public AccountsController(
        ILogger<AccountsController> logger,
        OpenCustomerAccountHandler openAccountHandler,
        GetBankAccountsForCustomerHandler getBankAccountsForCustomerHandler)
    {
        _logger = logger;
        _openAccountHandler = openAccountHandler;
        _getBankAccountsForCustomerHandler = getBankAccountsForCustomerHandler;
    }

    // ToDo: Move to BankAccountsController
    // GET /api/accounts/{customerId}
    // ToDo: add guard
    [HttpGet("{CustomerId}")]
    public async Task<IActionResult> Get([FromRoute] GetBankAccountsRequestDto request)
    {

        if (!Guid.TryParse(request.CustomerId, out Guid customerId))
        {
            return NotFound();
        }
        var query = new GetBankAccountsForCustomerQuery(customerId);

        try
        {
            var result = await _getBankAccountsForCustomerHandler.HandleAsync(query);
        

            var response = new GetBankAccountsResponseDto(
                    result.bankAccounts.Select(ba =>
                        new CustomerBankAccountDto(
                            Id: ba.Id,
                            CustomerId: ba.CustomerId,
                            AccountName: ba.Name,
                            Balance: ba.Balance,
                            OpenedAt: ba.CreatedAt,
                            UpdatedAt: ba.UpdatedAt)));

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        } catch (Exception ex)
        {
            // Log error
            EventId eventId = new();
            _logger.LogError(eventId, ex, message: ex.Message);

            return NotFound();
        }
    }

    // POST /api/accounts (Endpoint /  API endpoint)
    [HttpPost]
    public async Task<IActionResult> Create(CreateAccountRequestDto request)
    {
        try
        {
            var openAccountResult = await _openAccountHandler.HandleAsync(
                new OpenCustomerAccountCommand(
                    FirstName: request.FirstName,
                    LastName: request.LastName,
                    SocialSecurityNumber: request.SocialSecurityNumber,
                    Email: request.Email));

            var response = new CreateAccountResponseDto(openAccountResult.UserId);

            // Return 201 Created
            return Created(string.Empty, response);
        } catch (Exception ex)
        {
            // Log error
            EventId eventId = new();
            _logger.LogError(eventId, ex, message: ex.Message);

            if (ex.GetType() == typeof(DuplicateCustomerException))
            {
                ModelState.AddModelError("Email", "Invalid Email");
                // Return 400 Bad Request
                
                return BadRequest();
            }

            return NotFound();
        }

    }
}

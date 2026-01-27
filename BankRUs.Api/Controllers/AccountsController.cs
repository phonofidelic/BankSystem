using BankRUs.Api.Dtos.Accounts;
using BankRUs.Application.UseCases.GetBankAccountsForCustomer;
using BankRUs.Application.UseCases.OpenAccount;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankRUs.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly OpenAccountHandler _openAccountHandler;
    private readonly GetBankAccountsForCustomerHandler _getBankAccountsForCustomerHandler;

    public AccountsController(
        OpenAccountHandler openAccountHandler,
        GetBankAccountsForCustomerHandler getBankAccountsForCustomerHandler)
    {
        _openAccountHandler = openAccountHandler;
        _getBankAccountsForCustomerHandler = getBankAccountsForCustomerHandler;
    }

    // GET /api/accounts/{customerId}
    // ToDo: add guard
    [HttpGet("{CustomerId}")]
    public async Task<IActionResult> Get([FromRoute] GetBankAccountsForCustomerRequestDto request)
    {

        if (!Guid.TryParse(request.CustomerId, out Guid customerId))
        {
            return NotFound();
        }
        var query = new GetBankAccountsForCustomerQuery(customerId);


        var result = await _getBankAccountsForCustomerHandler.HandleAsync(query);

        var response = new GetBankAccountsForCustomerResponseDto(
                result.bankAccounts.Select(ba =>
                    new CustomerBankAccountDto(
                        Id: ba.Id,
                        CustomerId: ba.CustomerId,
                        Balance: ba.Balance,
                        OpenedAt: ba.CreatedAt,
                        UpdatedAt: ba.UpdatedAt)));

        if (response == null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    // POST /api/accounts (Endpoint /  API endpoint)
    [HttpPost]
    public async Task<IActionResult> Create(CreateAccountRequestDto request)
    {
        // Tjocka vs Tunna controllers

        var openAccountResult = await _openAccountHandler.HandleAsync(
            new OpenAccountCommand(
                FirstName: request.FirstName,
                LastName: request.LastName,
                SocialSecurityNumber: request.SocialSecurityNumber,
                Email: request.Email));

        var response = new CreateAccountResponseDto(openAccountResult.UserId);

        // Returnera 201 Created
        
        return Created(string.Empty, response);
    }

    private static bool IsValidLuhn(string digits)
    {
        var sum = 0;

        for (int i = 0; i < 9; i++)
        {
            var num = digits[i] - '0';
            num *= (i % 2 == 0) ? 2 : 1;
            if (num > 9) num -= 9;
            sum += num;
        }

        var controlDigit = (10 - (sum % 10)) % 10;

        return controlDigit == digits[9] - '0';
    }

}

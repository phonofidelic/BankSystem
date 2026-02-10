using BankRUs.Api.Dtos.Accounts;
using BankRUs.Api.Dtos.BankAccounts;
using BankRUs.Application.Exceptions;
using BankRUs.Application.Services.Identity;
using BankRUs.Application.UseCases.GetBankAccountsForCustomer;
using BankRUs.Application.UseCases.OpenAccount;
using BankRUs.Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankRUs.Api.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = Roles.CustomerServiceRepresentative)]
[ApiController]
public class AccountsController(
    ILogger<AccountsController> logger,
    IIdentityService identityService,
    OpenCustomerAccountHandler openAccountHandler,
    GetBankAccountsForCustomerHandler getBankAccountsForCustomerHandler) : ControllerBase
{
    private readonly ILogger<AccountsController> _logger = logger;
    private readonly IIdentityService _identityService = identityService;
    private readonly OpenCustomerAccountHandler _openAccountHandler = openAccountHandler;
    private readonly GetBankAccountsForCustomerHandler _getBankAccountsForCustomerHandler = getBankAccountsForCustomerHandler;

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

    // POST /api/accounts/customers/create (Endpoint /  API endpoint)
    [HttpPost("customers/create")]
    public async Task<IActionResult> CreateCustomer(CreateAccountRequestDto request)
    {
        try
        {
            var openAccountResult = await _openAccountHandler.HandleAsync(
                new OpenCustomerAccountCommand(
                    FirstName: request.FirstName,
                    LastName: request.LastName,
                    SocialSecurityNumber: request.SocialSecurityNumber,
                    Email: request.Email,
                    Password: request.Password));

            // Return 201 Created
            return Created(string.Empty, new CreateAccountResponseDto(openAccountResult.UserId));
        } catch (Exception ex)
        {
            // Log error
            EventId eventId = new();
            _logger.LogError(eventId, ex, message: ex.Message);

            if (ex.GetType() == typeof(DuplicateCustomerException))
            {
                ModelState.AddModelError("Account", "Customer account already exists");
                // Return 400 Bad Request
                return BadRequest(ModelState);
            }

            return BadRequest();
        }
    }

    //POST /api/accounts/employees/create
    [HttpPost("employees/create")]
    public async Task<IActionResult> CreateEmployee(CreateEmployeeAccountRequestDto request)
    {
        var createApplicationUserResult = await _identityService.CreateApplicationUserAsync(new CreateApplicationUserRequest(
            FirstName: request.FirstName,
            LastName: request.LastName,
            Email: request.Email,
            Password: request.Password));

        if (createApplicationUserResult == null)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _identityService.AssignCustomerServiceRepresentativeRoleToUser(createApplicationUserResult.UserId);

            return Created(string.Empty, createApplicationUserResult.UserId);
        } catch (Exception ex)
        {
            // Log error
            EventId eventId = new();
            _logger.LogError(eventId, ex, message: ex.Message);

            return BadRequest();
        }
    }
}

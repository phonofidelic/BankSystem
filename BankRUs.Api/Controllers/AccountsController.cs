using BankRUs.Api.Dtos.Accounts;
using BankRUs.Api.Dtos.BankAccounts;
using BankRUs.Application;
using BankRUs.Application.Exceptions;
using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.Services.Identity;
using BankRUs.Application.Services.PaginationService;
using BankRUs.Application.UseCases.CloseCustomerAccount;
using BankRUs.Application.UseCases.GetBankAccountsForCustomer;
using BankRUs.Application.UseCases.ListCustomerAccounts;
using BankRUs.Application.UseCases.OpenAccount;
using BankRUs.Application.UseCases.UpdateCustomerAccount;
using BankRUs.Domain.ValueObjects;
using BankRUs.Infrastructure.Services.Identity;
using BankRUs.Infrastructure.Services.IdentityService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankRUs.Api.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = Policies.REQUIRE_ROLE_CUSTOMER_SERVICE)]
[ApiController]
public class AccountsController(
    ILogger<AccountsController> logger,
    ICustomerService customerService,
    IIdentityService identityService,
    IHandler<ListCustomerAccountsQuery, ListCustomerAccountsResult> listCustomerAccountsHandler,
    IHandler<OpenCustomerAccountCommand, OpenCustomerAccountResponseDto> openAccountHandler,
    IHandler<UpdateCustomerAccountCommand, UpdateCustomerAccountResult> updateCustomerAccountHandler,
    IHandler<CloseCustomerAccountCommand, CloseCustomerAccountResult> closeCustomerAccountHandler,
    GetBankAccountsForCustomerHandler getBankAccountsForCustomerHandler) : ControllerBase
{
    private readonly ILogger<AccountsController> _logger = logger;
    private readonly ICustomerService _customerService = customerService;
    private readonly IIdentityService _identityService = identityService;
    private readonly IHandler<ListCustomerAccountsQuery, ListCustomerAccountsResult> _listCustomerAccountsHandler = listCustomerAccountsHandler;
    private readonly IHandler<OpenCustomerAccountCommand, OpenCustomerAccountResponseDto> _openAccountHandler = openAccountHandler;
    private readonly IHandler<UpdateCustomerAccountCommand, UpdateCustomerAccountResult> _updateCustomerAccountHandler = updateCustomerAccountHandler;
    private readonly IHandler<CloseCustomerAccountCommand, CloseCustomerAccountResult> _closeCustomerAccountHandler = closeCustomerAccountHandler;
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

    // GET /api/accounts/customers
    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomerAccounts([FromQuery] ListCustomerAccountsQuery query)
    {
        var result = await _listCustomerAccountsHandler.HandleAsync(query);

        var customerItems = result.Items.Select(customer => new CustomerAccountsListItemDto(
            CustomerId: customer.Id,
            FirstName: customer.FirstName,
            LastName: customer.LastName,
            Email: customer.Email)).ToList();

        return Ok(new GetCustomerAccountsResponseDto(
            Paging: result.Meta,
            Items: customerItems));
    }

    // GET /api/accounts/customers/{customerId}
    [HttpGet("customers/{customerId}")]
    public async Task<IActionResult> GetCustomerAccount(Guid customerId)
    {
        try
        {
            var customer = await _customerService.GetCustomerAsync(customerId);
            var bankAccountListItems = customer.BankAccounts.Select(b => new CustomerBankAccountListItemDto(
                Id: b.Id,
                Name: b.Name,
                CurrentBalance: b.Balance,
                Currency: b.Currency.ToString(),
                OpenedAt: b.CreatedAt)).ToList();

            return Ok(new GetCustomerAccountResponseDto(
                Id: customer.Id,
                FirstName: customer.FirstName,
                LastName: customer.LastName,
                Ssn: customer.SocialSecurityNumber,
                Email: customer.Email,
                BankAccounts: bankAccountListItems));
        } 
        catch (Exception ex)
        {
            if (ex is NotFoundException)
            {
                return NotFound();
            }
            return BadRequest();
        }
    }

    // PATCH /api/accounts/customers/{customerAccountId}
    [HttpPatch("customers/{customerAccountId}")]
    public async Task<IActionResult> PatchCustomer(
        [FromRoute] Guid customerAccountId,
        [FromBody] PatchCustomerAccountRequestDto requestBody)
    {
        try
        {
            var updateCustomerAccountResult = await _updateCustomerAccountHandler.HandleAsync(new UpdateCustomerAccountCommand(
                CustomerAccountId: customerAccountId,
                Details: new CustomerAccountDetails(
                    firstName: requestBody.FirstName,
                    lastName: requestBody.LastName,
                    email: requestBody.Email,
                    socialSecurityNumber: requestBody.Ssn)));

            if (updateCustomerAccountResult.UpdatedFields.Count < 1)
            {
                _logger.LogInformation("No fields were updated");
                return Ok();
            }
            _logger.LogInformation("Updated customer fields: {0}", updateCustomerAccountResult.UpdatedFields);
            return NoContent();
        }
        catch (Exception ex) { 
            if (ex is NotFoundException)
            {
                return NotFound();
            }
            return BadRequest();
        }
    }

    // DELETE /api/accounts/customers/{customerAccountId}
    [HttpDelete("customers/{customerAccountId}")]
    public async Task<IActionResult> CloseCustomerAccount([FromRoute] Guid customerAccountId)
    {
        try
        {
            await _closeCustomerAccountHandler.HandleAsync(new CloseCustomerAccountCommand(customerAccountId));
            return NoContent();
        }
        catch (Exception ex)
        {
            if (ex is NotFoundException)
            {
                return NotFound();
            }

            EventId eventId = new();
            _logger.LogError(eventId, ex, message: ex.Message);

            return BadRequest();
        }
    }

    // POST /api/accounts/customers/create (Endpoint /  API endpoint)
    [HttpPost("customers/create")]
    public async Task<IActionResult> CreateCustomer(CreateAccountRequestDto requestBody)
    {
        try
        {
            var openAccountResult = await _openAccountHandler.HandleAsync(
                new OpenCustomerAccountCommand(
                    FirstName: requestBody.FirstName,
                    LastName: requestBody.LastName,
                    SocialSecurityNumber: requestBody.SocialSecurityNumber,
                    Email: requestBody.Email,
                    Password: requestBody.Password));

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
    [Authorize(Policy = Policies.REQUIRE_ROLE_SYSTEM_ADMIN)]
    public async Task<IActionResult> CreateEmployee(CreateEmployeeAccountRequestDto requestBody)
    {
        var createApplicationUserResult = await _identityService.CreateApplicationUserAsync(new CreateApplicationUserRequest(
            FirstName: requestBody.FirstName,
            LastName: requestBody.LastName,
            Email: requestBody.Email,
            Password: requestBody.Password));

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

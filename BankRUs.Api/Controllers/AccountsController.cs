using BankRUs.Api.Dtos.Accounts;
using BankRUs.Application;
using BankRUs.Application.Exceptions;
using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.UseCases.CloseCustomerAccount;
using BankRUs.Application.UseCases.ListCustomerAccounts;
using BankRUs.Application.UseCases.OpenCustomerAccount;
using BankRUs.Application.UseCases.UpdateCustomerAccount;
using BankRUs.Domain.ValueObjects;
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
    IHandler<ListCustomerAccountsQuery, ListCustomerAccountsResult> listCustomerAccountsHandler,
    IHandler<OpenCustomerAccountCommand, OpenCustomerAccountResult> openAccountHandler,
    IHandler<UpdateCustomerAccountCommand, UpdateCustomerAccountResult> updateCustomerAccountHandler,
    IHandler<CloseCustomerAccountCommand, CloseCustomerAccountResult> closeCustomerAccountHandler) : ControllerBase
{
    private readonly ILogger<AccountsController> _logger = logger;
    private readonly ICustomerService _customerService = customerService;
    private readonly IHandler<ListCustomerAccountsQuery, ListCustomerAccountsResult> _listCustomerAccountsHandler = listCustomerAccountsHandler;
    private readonly IHandler<OpenCustomerAccountCommand, OpenCustomerAccountResult> _openAccountHandler = openAccountHandler;
    private readonly IHandler<UpdateCustomerAccountCommand, UpdateCustomerAccountResult> _updateCustomerAccountHandler = updateCustomerAccountHandler;
    private readonly IHandler<CloseCustomerAccountCommand, CloseCustomerAccountResult> _closeCustomerAccountHandler = closeCustomerAccountHandler;

    // GET /api/accounts/customers
    [HttpGet("customers")]
    [Produces("application/json")]
    [ProducesResponseType<GetCustomerAccountsResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    [Produces("application/json")]
    [ProducesResponseType<GetCustomerAccountResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                OpenedAt: b.CreatedAt,
                AccountStatus: b.Status.ToString())).ToList();

            return Ok(new GetCustomerAccountResponseDto(
                Id: customer.Id,
                FirstName: customer.FirstName,
                LastName: customer.LastName,
                Ssn: customer.SocialSecurityNumber,
                Email: customer.Email,
                AccountStatus: customer.Status.ToString(),
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
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CloseCustomerAccount([FromRoute] Guid customerAccountId)
    {
        try
        {
            await _closeCustomerAccountHandler.HandleAsync(new CloseCustomerAccountCommand(customerAccountId));
            return NoContent();
        }
        catch (Exception ex)
        {
            EventId eventId = new();
            _logger.LogError(eventId, ex, message: ex.Message);

            if (ex is NotFoundException)
            {
                return NotFound();
            }

            return BadRequest();
        }
    }

    // POST /api/accounts/customers/create (Endpoint /  API endpoint)
    [HttpPost("customers/create")]
    [Produces("application/json")]
    [ProducesResponseType<CreateCustomerAccountResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCustomer(CreateCustomerAccountRequestDto requestBody)
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
            return Created(string.Empty, new CreateCustomerAccountResponseDto(openAccountResult.CustomerAccountId));
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

    
}

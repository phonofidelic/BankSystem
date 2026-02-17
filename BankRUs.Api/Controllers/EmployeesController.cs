using BankRUs.Api.Dtos.Employees;
using BankRUs.Application.Services.Identity;
using BankRUs.Infrastructure.Services.IdentityService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankRUs.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController(
    ILogger<AccountsController> logger,
    IIdentityService identityService) : ControllerBase
{
    private readonly ILogger<AccountsController> _logger = logger;
    private readonly IIdentityService _identityService = identityService;

    //POST /api/employees/create
    [HttpPost("create")]
    [Authorize(Policy = Policies.REQUIRE_ROLE_SYSTEM_ADMIN)]
    [HttpPost("customers/create")]
    [Produces("application/json")]
    [ProducesResponseType<CreateEmployeeAccountResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

            return Created(string.Empty, createApplicationUserResult);
        }
        catch (Exception ex)
        {
            // Log error
            EventId eventId = new();
            _logger.LogError(eventId, ex, message: ex.Message);

            return BadRequest();
        }
    }
}

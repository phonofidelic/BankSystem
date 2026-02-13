using BankRUs.Api.Dtos.Accounts;
using BankRUs.Api.Dtos.Me;
using BankRUs.Application;
using BankRUs.Application.Exceptions;
using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.UseCases.GetCustomerAccountDetails;
using BankRUs.Application.UseCases.UpdateCustomerAccount;
using BankRUs.Domain.ValueObjects;
using BankRUs.Infrastructure.Services.Identity;
using BankRUs.Infrastructure.Services.IdentityService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace BankRUs.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.REQUIRE_AUTHENTICATION)]
    public class MeController(
        ILogger<MeController> logger,
        ICustomerService customerService,
        IHandler<GetCustomerAccountDetailsQuery, GetCustomerAccountDetailsResult> getCustomerDetailsResponseHandler,
        IHandler<UpdateCustomerAccountCommand, UpdateCustomerAccountResult> updateCustomerAccountHandler) : ControllerBase
    {
        private readonly ILogger<MeController> _logger = logger;
        private readonly ICustomerService _customerService = customerService;
        private readonly IHandler<GetCustomerAccountDetailsQuery, GetCustomerAccountDetailsResult> _getBankAccountsForCustomerHandler = getCustomerDetailsResponseHandler;
        private readonly IHandler<UpdateCustomerAccountCommand, UpdateCustomerAccountResult> _updateCustomerAccountHandler = updateCustomerAccountHandler;

        // GET /api/me
        [HttpGet]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) {
                return Unauthorized();
            };

            var email = User.FindFirstValue(ClaimTypes.Email);


            if (!Guid.TryParse(userId, out Guid applicationUserId))
            {
                return Unauthorized();
            }

            if (!User.IsInRole(Roles.Customer))
            {
                // Return employee profile
                return Ok(new GetMeResponseDto(userId, email));
            }

            var query = new GetCustomerAccountDetailsQuery(applicationUserId);
            var result = await _getBankAccountsForCustomerHandler.HandleAsync(query);

            if (result == null)
            {
                return NotFound();
            }

            var bankAccountListItems = result.BankAccounts.Select(b => new MeCustomerBankAccountListItemDto(
                Id: b.Id,
                Name: b.Name,
                CurrentBalance: b.Balance,
                Currency: b.Currency.ToString(),
                OpenedAt: b.CreatedAt
            )).ToList();

            // Anonymize last four digits of social security number
            // ToDo: If anonymization is a business rule, move it to CustomerAccount entity or service
            var socialSecurityNumber = result.CustomerAccountDetails.SocialSecurityNumber ?? "";
            var lastFour = new Regex(@"\d{4}$");
            var anonomizedSocialSecurityNumber = lastFour.Replace(socialSecurityNumber, "####");

            var response = new GetMeCustomerAccountResponseDto(
                Id: result.CustomerAccountId,
                FirstName: result.CustomerAccountDetails.FirstName,
                LastName: result.CustomerAccountDetails.LastName,
                Ssn: anonomizedSocialSecurityNumber,
                Email: result.CustomerAccountDetails.Email,
                BankAccounts: bankAccountListItems
            );

            return Ok(response);
        }

        // PATCH /api/me
        [HttpPatch]
        public async Task<IActionResult> PatchMe(PatchCustomerAccountRequestDto requestBody)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var email = User.FindFirstValue(ClaimTypes.Email);

            if (!Guid.TryParse(userId, out Guid applicationUserId))
            {
                return Unauthorized();
            }

            if (!User.IsInRole(Roles.Customer))
            {
                return StatusCode(501);
            }

            try
            {
                var customerAccountId = await _customerService.GetCustomerIdAsync(applicationUserId);
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
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    return NotFound();
                }
                return BadRequest();
            }
        }
    }
}

using BankRUs.Api.Dtos.CustomerAccounts;
using BankRUs.Api.Dtos.Me;
using BankRUs.Application;
using BankRUs.Application.Exceptions;
using BankRUs.Application.Services.CustomerAccountService;
using BankRUs.Application.UseCases.CloseCustomerAccount;
using BankRUs.Application.UseCases.GetCustomerAccountDetails;
using BankRUs.Application.UseCases.UpdateCustomerAccount;
using BankRUs.Domain.ValueObjects;
using BankRUs.Infrastructure.Services.Identity;
using BankRUs.Infrastructure.Services.IdentityService;
using Microsoft.AspNetCore.Authorization;
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
        ICustomerAccountService customerService,
        IHandler<GetCustomerAccountDetailsQuery, GetCustomerAccountDetailsResult> getCustomerDetailsResponseHandler,
        IHandler<UpdateCustomerAccountCommand, UpdateCustomerAccountResult> updateCustomerAccountHandler,
        IHandler<CloseCustomerAccountCommand, CloseCustomerAccountResult> closeCustomerAccountHandler) : ControllerBase
    {
        private readonly ILogger<MeController> _logger = logger;
        private readonly ICustomerAccountService _customerService = customerService;
        private readonly IHandler<GetCustomerAccountDetailsQuery, GetCustomerAccountDetailsResult> _getBankAccountsForCustomerHandler = getCustomerDetailsResponseHandler;
        private readonly IHandler<UpdateCustomerAccountCommand, UpdateCustomerAccountResult> _updateCustomerAccountHandler = updateCustomerAccountHandler;
        private readonly IHandler<CloseCustomerAccountCommand, CloseCustomerAccountResult> _closeCustomerAccountHandler = closeCustomerAccountHandler;

        // GET /api/me
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType<GetMeResponseDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<GetMeCustomerAccountResponseDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status501NotImplemented)]
        public async Task<IActionResult> PatchMe(PatchCustomerAccountRequestDto requestBody)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            if (!Guid.TryParse(userId, out Guid applicationUserId))
            {
                return Unauthorized();
            }

            if (!User.IsInRole(Roles.Customer))
            {
                return StatusCode(StatusCodes.Status501NotImplemented);
            }

            try
            {
                var customerAccountId = await _customerService.GetCustomerAccountIdAsync(applicationUserId);
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

        // DELETE /api/me
        [HttpDelete]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status501NotImplemented)]
        public async Task<IActionResult> DeleteMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            if (!Guid.TryParse(userId, out Guid applicationUserId))
            {
                return Unauthorized();
            }

            if (!User.IsInRole(Roles.Customer))
            {
                return StatusCode(StatusCodes.Status501NotImplemented);
            }

            try
            {
                var customerAccountId = await _customerService.GetCustomerAccountIdAsync(applicationUserId);
                await _closeCustomerAccountHandler.HandleAsync(new CloseCustomerAccountCommand(customerAccountId));

                return NoContent();
            }
            catch (Exception ex) {
                EventId eventId = new();
                _logger.LogError(eventId, ex, message: ex.Message);

                if (ex is NotFoundException)
                {
                    return NotFound();
                }

                return BadRequest();
            }
        }
    }
}

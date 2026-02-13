using System.Security.Claims;
using BankRUs.Api.Dtos.Me;
using BankRUs.Application;
using BankRUs.Application.UseCases.GetCustomerAccountDetails;
using BankRUs.Domain.ValueObjects;
using BankRUs.Infrastructure.Services.Identity;
using BankRUs.Infrastructure.Services.IdentityService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankRUs.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.REQUIRE_AUTHENTICATION)]
    public class MeController(
        ILogger<MeController> logger,
        IHandler<GetCustomerAccountDetailsQuery, GetCustomerAccountDetailsResult> getCustomerDetailsResponseHandler
    ) : ControllerBase
    {
        private readonly ILogger<MeController> _logger = logger;
        private readonly IHandler<GetCustomerAccountDetailsQuery, GetCustomerAccountDetailsResult> _getBankAccountsForCustomerHandler = getCustomerDetailsResponseHandler;

        // GET /api/me
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) {
                return Unauthorized();
            };

            var email = User.FindFirstValue(ClaimTypes.Email);

            if (!User.IsInRole(Roles.Customer))
            {
                // Return employee profile
                return Ok(new GetMeResponseDto(userId, email));
            }

            if (!Guid.TryParse(userId, out Guid applicationUserId))
            {
                return NotFound();
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

            var response = new GetMeCustomerAccountResponseDto(
                Id: result.CustomerAccountId,
                FirstName: result.CustomerAccountDetails.FirstName,
                LastName: result.CustomerAccountDetails.LastName,
                Ssn: result.CustomerAccountDetails.SocialSecurityNumber,
                Email: result.CustomerAccountDetails.Email,
                BankAccounts: bankAccountListItems
            );

            return Ok(response);
        }
    }
}

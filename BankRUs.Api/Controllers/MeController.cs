using System.Security.Claims;
using BankRUs.Api.Dtos.Me;
using BankRUs.Infrastructure.Services.IdentityService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankRUs.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.REQUIRE_AUTHENTICATION)]
    public class MeController : ControllerBase
    {
        // GET /api/me
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) {
                return Unauthorized();
            };

            var email = User.FindFirstValue(ClaimTypes.Email);
            
            return Ok(new GetMeResponseDto(userId, email));
        }
    }
}

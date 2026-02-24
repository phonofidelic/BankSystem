using BankRUs.Application.Services.Authentication;
using BankRUs.Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Identity;

namespace BankRUs.Infrastructure.Services.Authentication
{
    public class AuthenticationService(UserManager<ApplicationUser> userManager) : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        public async Task<AuthenticatedUser?> AuthenticateUserAsync(string username, string password)
        {
            var user = await _userManager.FindByEmailAsync(username);

            if (user == null || user.UserName == null || user.Email == null) {
                return null;
            }

            var validPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!validPassword)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new AuthenticatedUser(
                UserId: user.Id,
                UserName: user.UserName,
                Email: user.Email,
                Roles: roles);
        }
    }
}

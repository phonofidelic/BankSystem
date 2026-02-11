using BankRUs.Application.Services.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankRUs.Infrastructure.Services.Authenticationl;

public class TokenService(IOptions<JwtOptions> options) : ITokenService
{
    private readonly JwtOptions _jwt = options.Value;
    public Token CreateToken(string userId, string email, IEnumerable<string>? roles = null)
    {
        // 1. Add claims
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.PreferredUsername, email)
        };

        // 2. Add roles
        foreach (var role in roles ?? Enumerable.Empty<string>())
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // From appSettings
        var issuer = _jwt.Issuer;
        var audience = _jwt.Audience;
        var expiresMinutes = _jwt.ExpiresMinutes;
        // From secrets
        var secret = _jwt.Secret;

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var signingCreds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: signingCreds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new Token(
            AccessToken: tokenString,
            ExpiresAtUtc: DateTime.UtcNow.AddHours(1));
    }
}
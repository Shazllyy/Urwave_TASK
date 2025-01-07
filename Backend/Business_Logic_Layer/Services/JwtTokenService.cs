using Business_Logic_Layer.ServicesInterfaces;
using Data_Access_Layer.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business_Logic_Layer.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        // Hardcoded JWT configuration values
        private const string SecretKey = "IuSt4&8k00SsQwelOiuE334GhstSAdSllpOiuSdxHtgggfSiuiNnCCYyTe";  // Secret key for signing JWT
        private const string Issuer = "http://localhost:5000";  // Issuer of the JWT
        private const string Audience = "your-api-audience";  // Audience for the JWT

        public string GenerateJwtToken(User user)
        {
            // Set token expiration
            var expiration = DateTime.UtcNow.AddHours(1);

            // Create claims
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.Name) // Add role claim
            };

            // Create signing key from hardcoded secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create JWT token
            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            // Return the generated token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

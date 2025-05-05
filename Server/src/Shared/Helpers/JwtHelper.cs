using System.ComponentModel.Design;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Shared.Settings;

namespace Shared.Helpers 
{
    public class JwtHelper
    {
        public static string GenerateAccessToken(int employeeId, int role, int companyId,string accessToken, JwtSettings settings)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("EmployeeId", employeeId.ToString()),
                new Claim(ClaimTypes.Role, role.ToString()),
                new Claim("CompanyId", companyId.ToString()),
                new Claim("AccessToken", accessToken),                
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID cho token
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(settings.ExpiryInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}

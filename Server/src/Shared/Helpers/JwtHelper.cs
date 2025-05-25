using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Helpers 
{
    public class JwtHelper
    {
        public static string  GenerateAccessToken(int accountId, int employeeId, int companyId, int role , IConfiguration configuration,out string jwtID)
        {
            jwtID = GenerateRefreshToken();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("AccountId", accountId.ToString()),
                new Claim("EmployeeId", employeeId.ToString()),
                new Claim("CompanyId", companyId.ToString()),
                new Claim(ClaimTypes.Role, role.ToString()),               
                new Claim(JwtRegisteredClaimNames.Jti, jwtID),                
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID cho token
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(configuration["Jwt:ExpiryInMinutes"])),
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

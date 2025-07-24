using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Entities.Auth;
using WorkflowMgmt.Domain.Interface.JwtToken;

namespace WorkflowMgmt.Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user, Role role, DepartmentDTO dept)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = BuildClaims(user, role, dept);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiresInMinutes"]!)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public AuthTokenResponse GenerateTokens(User user, Role role, DepartmentDTO? dept)
        {
            var accessToken = GenerateToken(user, role, dept);
            var refreshToken = GenerateRefreshToken();
            var expiresAt = DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiresInMinutes"]!));

            return new AuthTokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt
            };
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = false, // We'll check expiry separately
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public bool IsTokenExpired(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);
                return jsonToken.ValidTo < DateTime.Now;
            }
            catch
            {
                return true;
            }
        }

        private List<Claim> BuildClaims(User user, Role role, DepartmentDTO? dept)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Name, user.username),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Role, role.code),
                new Claim("firstName", user.first_name),
                new Claim("lastName", user.last_name),
                new Claim("fullName", $"{user.first_name} {user.last_name}"),
                new Claim("roleId", user.role_id.ToString()),
                new Claim("roleName", role.name),
                new Claim("roleCode", role.code),
                new Claim("userId", user.id.ToString())
            };

            // Add department claims if user has a department
            if (dept != null)
            {
                claims.Add(new Claim("departmentId", dept.Id.ToString() ?? ""));
                claims.Add(new Claim("departmentName", dept.name));
                claims.Add(new Claim("departmentCode", dept.code));
            }

            if (role.permissions != null)
            {
                foreach (var permission in role.permissions)
                {
                    claims.Add(new Claim("permission", permission));
                }
            }

            return claims;
        }
    }
}

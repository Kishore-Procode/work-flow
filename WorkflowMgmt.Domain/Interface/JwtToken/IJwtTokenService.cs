using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Entities.Auth;

namespace WorkflowMgmt.Domain.Interface.JwtToken
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user, Role role, DepartmentDTO dept);
        AuthTokenResponse GenerateTokens(User user, Role role, DepartmentDTO dept);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
        bool IsTokenExpired(string token);
    }
}

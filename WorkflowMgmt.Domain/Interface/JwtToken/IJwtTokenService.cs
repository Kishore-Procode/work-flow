using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;

namespace WorkflowMgmt.Domain.Interface.JwtToken
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user, Role role, DepartmentDTO dept);
    }
}

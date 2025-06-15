using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Interface.Common
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? Username { get; }
        string? Email { get; }
        string? Role { get; }
        string? RoleCode { get; }
        string? DepartmentId { get; }
        string? DepartmentName { get; }
        bool IsAuthenticated { get; }
        bool IsAdmin { get; }
        bool IsLeadership { get; }
        bool HasPermission(string permission);
        bool CanAccessDocument(string documentId, string documentType);
    }
}

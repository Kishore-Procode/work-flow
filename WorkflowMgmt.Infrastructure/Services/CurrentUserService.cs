using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.Common;

namespace WorkflowMgmt.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        public string? Username => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
        public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
        public string? Role => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
        public string? RoleCode => _httpContextAccessor.HttpContext?.User?.FindFirst("roleCode")?.Value;
        public string? DepartmentId => _httpContextAccessor.HttpContext?.User?.FindFirst("departmentId")?.Value;
        public string? DepartmentName => _httpContextAccessor.HttpContext?.User?.FindFirst("departmentName")?.Value;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
        public bool IsAdmin => RoleCode == "ADMIN";
        public bool IsLeadership => RoleCode == "LEADERSHIP" || IsAdmin;

        public bool HasPermission(string permission)
        {
            if (IsAdmin) return true;

            var permissions = _httpContextAccessor.HttpContext?.User?.FindAll("permission")?.Select(c => c.Value);
            return permissions?.Contains(permission) == true;
        }

        public bool CanAccessDocument(string documentId, string documentType)
        {
            if (IsAdmin || IsLeadership) return true;

            // TODO: Implement document-specific access logic
            // This would typically check if the user is the document owner
            // or has workflow permissions for the document
            return true;
        }
    }
}

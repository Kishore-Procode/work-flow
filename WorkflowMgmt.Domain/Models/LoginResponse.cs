using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Application.Features.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? DepartmentCode { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public DateTime? LastLogin { get; set; }
        public string[] Permissions { get; set; } = Array.Empty<string>();

        // Helper properties for UI
        public string DisplayName => !string.IsNullOrEmpty(FullName) ? FullName : Username;
        public string RoleDisplay => Role switch
        {
            "FACULTY" => "Faculty",
            "PROGRAM_CONVENER" => "Program Convener",
            "LEADERSHIP" => "Leadership Team",
            "BOARD_EXAMINATIONS" => "Board of Examinations",
            "SHAREHOLDER" => "Shareholder",
            "INDUSTRY_EXPERT" => "Industry Expert",
            "ADMIN" => "Administrator",
            _ => Role
        };
        public bool IsAdmin => RoleCode == "ADMIN";
        public bool IsLeadership => RoleCode == "LEADERSHIP" || RoleCode == "ADMIN";
        public bool CanApprove => Permissions.Contains("approve_documents") || IsAdmin;
        public bool CanReject => Permissions.Contains("reject_documents") || IsAdmin;
        public bool CanProvideFeedback => Permissions.Contains("provide_feedback") || RoleCode == "SHAREHOLDER" || RoleCode == "INDUSTRY_EXPERT";
        public bool IsFeedbackProvider => RoleCode == "SHAREHOLDER" || RoleCode == "INDUSTRY_EXPERT";
    }
}

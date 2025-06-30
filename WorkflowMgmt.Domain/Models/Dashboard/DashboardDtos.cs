using System;
using System.Collections.Generic;

namespace WorkflowMgmt.Domain.Models.Dashboard
{
    public class DashboardStatsDto
    {
        public string Title { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Change { get; set; } = string.Empty;
        public string ChangeType { get; set; } = string.Empty; // positive, negative, neutral
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public class DashboardActivityDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // approval, submission, revision, completion
        public string Message { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string DocumentId { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class DashboardDeadlineDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Deadline { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty; // high, medium, low
        public string Assignee { get; set; } = string.Empty;
        public string DocumentId { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string CurrentStage { get; set; } = string.Empty;
    }

    public class DashboardWorkflowStatusDto
    {
        public string DocumentId { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CurrentStage { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public int Progress { get; set; }
        public int DaysLeft { get; set; }
        public string Priority { get; set; } = string.Empty;
    }

    public class RoleDashboardDto
    {
        public string UserRole { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public List<DashboardStatsDto> Stats { get; set; } = new();
        public List<DashboardActivityDto> RecentActivities { get; set; } = new();
        public List<DashboardDeadlineDto> UpcomingDeadlines { get; set; } = new();
        public List<DashboardWorkflowStatusDto> WorkflowStatus { get; set; } = new();
        public Dictionary<string, object> RoleSpecificData { get; set; } = new();
    }

    // Role-specific data models
    public class AdminDashboardData
    {
        public int TotalUsers { get; set; }
        public int ActiveWorkflows { get; set; }
        public int SystemAlerts { get; set; }
        public List<DepartmentStatsDto> DepartmentStats { get; set; } = new();
        public List<UserActivityDto> UserActivities { get; set; } = new();
    }

    public class FacultyDashboardData
    {
        public int MySyllabi { get; set; }
        public int MyLessons { get; set; }
        public int MySessions { get; set; }
        public int PendingReviews { get; set; }
        public List<MyDocumentDto> MyDocuments { get; set; } = new();
    }

    public class ApproverDashboardData
    {
        public int PendingApprovals { get; set; }
        public int ApprovedThisMonth { get; set; }
        public int RejectedThisMonth { get; set; }
        public List<PendingApprovalDto> PendingDocuments { get; set; } = new();
    }

    public class DepartmentStatsDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalDocuments { get; set; }
        public int PendingDocuments { get; set; }
        public int ApprovedDocuments { get; set; }
        public int ActiveFaculty { get; set; }
    }

    public class UserActivityDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Activity { get; set; } = string.Empty;
        public DateTime ActivityDate { get; set; }
        public string DocumentType { get; set; } = string.Empty;
    }

    public class MyDocumentDto
    {
        public string DocumentId { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CurrentStage { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
        public int Progress { get; set; }
    }

    public class PendingApprovalDto
    {
        public string DocumentId { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string SubmittedBy { get; set; } = string.Empty;
        public DateTime SubmittedDate { get; set; }
        public string CurrentStage { get; set; } = string.Empty;
        public int DaysWaiting { get; set; }
        public string Priority { get; set; } = string.Empty;
    }
}

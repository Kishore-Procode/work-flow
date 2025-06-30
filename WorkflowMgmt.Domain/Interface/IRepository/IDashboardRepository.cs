using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models.Dashboard;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IDashboardRepository
    {
        Task<RoleDashboardDto> GetDashboardDataByRole(Guid userId, string roleCode, int? departmentId);
        Task<List<DashboardStatsDto>> GetStatsForRole(Guid userId, string roleCode, int? departmentId);
        Task<List<DashboardActivityDto>> GetRecentActivities(Guid userId, string roleCode, int? departmentId, int limit = 10);
        Task<List<DashboardDeadlineDto>> GetUpcomingDeadlines(Guid userId, string roleCode, int? departmentId, int limit = 5);
        Task<List<DashboardWorkflowStatusDto>> GetWorkflowStatus(Guid userId, string roleCode, int? departmentId, int limit = 10);
        
        // Role-specific data methods
        Task<AdminDashboardData> GetAdminDashboardData(Guid userId);
        Task<FacultyDashboardData> GetFacultyDashboardData(Guid userId, int? departmentId);
        Task<ApproverDashboardData> GetApproverDashboardData(Guid userId, string roleCode);
        
        // Common stats methods
        Task<int> GetTotalDocumentsByType(string documentType, int? departmentId = null);
        Task<int> GetPendingDocumentsByType(string documentType, int? departmentId = null);
        Task<int> GetApprovedDocumentsThisMonth(int? departmentId = null);
        Task<int> GetUserDocumentCount(Guid userId, string documentType);
        Task<int> GetPendingApprovalsForUser(Guid userId, string roleCode);
    }
}

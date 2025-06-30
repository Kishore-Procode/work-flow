using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Domain.Models.Dashboard;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class DashboardRepository : RepositoryTranBase, IDashboardRepository
    {
        public DashboardRepository(IDbTransaction transaction) : base(transaction) { }

        public async Task<RoleDashboardDto> GetDashboardDataByRole(Guid userId, string roleCode, int? departmentId)
        {
            var dashboard = new RoleDashboardDto
            {
                UserRole = roleCode,
                UserName = "User", // This should be populated from user data
                Department = "Department", // This should be populated from department data
                RoleSpecificData = new Dictionary<string, object>()
            };

            // Execute queries sequentially to avoid connection conflicts
            dashboard.Stats = await GetStatsForRole(userId, roleCode, departmentId);
            dashboard.RecentActivities = await GetRecentActivities(userId, roleCode, departmentId);
            dashboard.UpcomingDeadlines = await GetUpcomingDeadlines(userId, roleCode, departmentId);
            dashboard.WorkflowStatus = await GetWorkflowStatus(userId, roleCode, departmentId);

            // Get role-specific data
            switch (roleCode.ToUpper())
            {
                case "ADMIN":
                    var adminData = await GetAdminDashboardData(userId);
                    dashboard.RoleSpecificData["adminData"] = adminData;
                    break;
                case "FACULTY":
                    var facultyData = await GetFacultyDashboardData(userId, departmentId);
                    dashboard.RoleSpecificData["facultyData"] = facultyData;
                    break;
                case "LEADERSHIP":
                case "PROGRAM_CONVENER":
                case "BOARD_EXAMINATIONS":
                    var approverData = await GetApproverDashboardData(userId, roleCode);
                    dashboard.RoleSpecificData["approverData"] = approverData;
                    break;
            }

            return dashboard;
        }

        public async Task<List<DashboardStatsDto>> GetStatsForRole(Guid userId, string roleCode, int? departmentId)
        {
            var stats = new List<DashboardStatsDto>();

            switch (roleCode.ToUpper())
            {
                case "ADMIN":
                    stats = await GetAdminStats(departmentId);
                    break;
                case "FACULTY":
                    stats = await GetFacultyStats(userId, departmentId);
                    break;
                case "LEADERSHIP":
                case "PROGRAM_CONVENER":
                case "BOARD_EXAMINATIONS":
                    stats = await GetApproverStats(userId, roleCode, departmentId);
                    break;
            }

            return stats;
        }

        private async Task<List<DashboardStatsDto>> GetAdminStats(int? departmentId)
        {
            var stats = new List<DashboardStatsDto>();

            // Get Total Syllabi
            var syllabusQuery = @"
                SELECT COUNT(*)
                FROM workflowmgmt.syllabi
                WHERE (@DepartmentId IS NULL OR department_id = @DepartmentId)";

            var syllabusCount = await Connection.QuerySingleAsync<int>(syllabusQuery, new { DepartmentId = departmentId }, transaction: Transaction);
            stats.Add(new DashboardStatsDto
            {
                Title = "Total Syllabi",
                Value = syllabusCount.ToString(),
                Change = "Active syllabi",
                ChangeType = "positive",
                Icon = "BookOpen",
                Color = "bg-blue-600"
            });

            // Get Pending Approvals
            var pendingQuery = @"
                SELECT COUNT(*)
                FROM workflowmgmt.document_workflows dw
                WHERE dw.status = 'active'";

            var pendingCount = await Connection.QuerySingleAsync<int>(pendingQuery, transaction: Transaction);
            stats.Add(new DashboardStatsDto
            {
                Title = "Pending Approvals",
                Value = pendingCount.ToString(),
                Change = "Awaiting review",
                ChangeType = "neutral",
                Icon = "Clock",
                Color = "bg-yellow-600"
            });

            // Get Active Faculty
            var facultyQuery = @"
                SELECT COUNT(*)
                FROM workflowmgmt.users u
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                WHERE r.code = 'FACULTY'
                AND u.is_active = true
                AND (@DepartmentId IS NULL OR u.department_id = @DepartmentId)";

            var facultyCount = await Connection.QuerySingleAsync<int>(facultyQuery, new { DepartmentId = departmentId }, transaction: Transaction);
            stats.Add(new DashboardStatsDto
            {
                Title = "Active Faculty",
                Value = facultyCount.ToString(),
                Change = "Faculty members",
                ChangeType = "positive",
                Icon = "Users",
                Color = "bg-purple-600"
            });

            return stats;
        }

        private async Task<List<DashboardStatsDto>> GetFacultyStats(Guid userId, int? departmentId)
        {
            var stats = new List<DashboardStatsDto>();

            // Get My Syllabi
            var syllabusQuery = "SELECT COUNT(*) FROM workflowmgmt.syllabi WHERE faculty_id = @UserId";
            var syllabusCount = await Connection.QuerySingleAsync<int>(syllabusQuery, new { UserId = userId }, transaction: Transaction);
            stats.Add(new DashboardStatsDto
            {
                Title = "My Syllabi",
                Value = syllabusCount.ToString(),
                Change = "Total syllabi",
                ChangeType = "positive",
                Icon = "BookOpen",
                Color = "bg-blue-600"
            });

            // Get My Lessons
            var lessonQuery = @"
                SELECT COUNT(*)
                FROM workflowmgmt.lesson_plans lp
                INNER JOIN workflowmgmt.syllabi s ON lp.syllabus_id = s.id
                WHERE s.faculty_id = @UserId";
            var lessonCount = await Connection.QuerySingleAsync<int>(lessonQuery, new { UserId = userId }, transaction: Transaction);
            stats.Add(new DashboardStatsDto
            {
                Title = "My Lessons",
                Value = lessonCount.ToString(),
                Change = "Total lessons",
                ChangeType = "positive",
                Icon = "Target",
                Color = "bg-green-600"
            });

            // Get My Sessions
            var sessionQuery = @"
                SELECT COUNT(*)
                FROM workflowmgmt.sessions sess
                INNER JOIN workflowmgmt.lesson_plans lp ON sess.lesson_plan_id = lp.id
                INNER JOIN workflowmgmt.syllabi s ON lp.syllabus_id = s.id
                WHERE s.faculty_id = @UserId";
            var sessionCount = await Connection.QuerySingleAsync<int>(sessionQuery, new { UserId = userId }, transaction: Transaction);
            stats.Add(new DashboardStatsDto
            {
                Title = "My Sessions",
                Value = sessionCount.ToString(),
                Change = "Total sessions",
                ChangeType = "positive",
                Icon = "Presentation",
                Color = "bg-purple-600"
            });

            // Get Pending Reviews
            var pendingQuery = @"
                SELECT COUNT(*)
                FROM workflowmgmt.workflow_stage_history wsh
                WHERE wsh.assigned_to = @UserId
                AND wsh.processed_date IS NULL";
            var pendingCount = await Connection.QuerySingleAsync<int>(pendingQuery, new { UserId = userId }, transaction: Transaction);
            stats.Add(new DashboardStatsDto
            {
                Title = "Pending Reviews",
                Value = pendingCount.ToString(),
                Change = "Awaiting action",
                ChangeType = "neutral",
                Icon = "Clock",
                Color = "bg-yellow-600"
            });

            return stats;
        }

        private async Task<List<DashboardStatsDto>> GetApproverStats(Guid userId, string roleCode, int? departmentId)
        {
            var stats = new List<DashboardStatsDto>();

            // Get Pending Approvals
            var pendingQuery = @"
                SELECT COUNT(*)
                FROM workflowmgmt.workflow_stage_history wsh
                WHERE wsh.assigned_to = @UserId
                AND wsh.processed_date IS NULL";
            var pendingCount = await Connection.QuerySingleAsync<int>(pendingQuery, new { UserId = userId }, transaction: Transaction);
            stats.Add(new DashboardStatsDto
            {
                Title = "Pending Approvals",
                Value = pendingCount.ToString(),
                Change = "Awaiting your review",
                ChangeType = "neutral",
                Icon = "Clock",
                Color = "bg-yellow-600"
            });

            // Get Approved This Month
            var approvedQuery = @"
                SELECT COUNT(*)
                FROM workflowmgmt.workflow_stage_history wsh
                WHERE wsh.processed_by = @UserId
                AND wsh.action_taken = 'approved'
                AND wsh.processed_date >= DATE_TRUNC('month', CURRENT_DATE)";
            var approvedCount = await Connection.QuerySingleAsync<int>(approvedQuery, new { UserId = userId }, transaction: Transaction);
            stats.Add(new DashboardStatsDto
            {
                Title = "Approved This Month",
                Value = approvedCount.ToString(),
                Change = "Documents approved",
                ChangeType = "positive",
                Icon = "CheckCircle",
                Color = "bg-green-600"
            });

            // Get Total Documents
            var totalQuery = "SELECT COUNT(*) FROM workflowmgmt.document_workflows";
            var totalCount = await Connection.QuerySingleAsync<int>(totalQuery, transaction: Transaction);
            stats.Add(new DashboardStatsDto
            {
                Title = "Total Documents",
                Value = totalCount.ToString(),
                Change = "All documents",
                ChangeType = "neutral",
                Icon = "FileText",
                Color = "bg-blue-600"
            });

            return stats;
        }

        public async Task<List<DashboardActivityDto>> GetRecentActivities(Guid userId, string roleCode, int? departmentId, int limit = 10)
        {
            // Return empty list for now to avoid complex query issues
            // This can be implemented later with simpler queries
            return new List<DashboardActivityDto>();
        }

        public async Task<List<DashboardDeadlineDto>> GetUpcomingDeadlines(Guid userId, string roleCode, int? departmentId, int limit = 5)
        {
            // Return empty list for now to avoid complex query issues
            return new List<DashboardDeadlineDto>();
        }

        public async Task<List<DashboardWorkflowStatusDto>> GetWorkflowStatus(Guid userId, string roleCode, int? departmentId, int limit = 10)
        {
            // Return empty list for now to avoid complex query issues
            return new List<DashboardWorkflowStatusDto>();
        }

        public async Task<AdminDashboardData> GetAdminDashboardData(Guid userId)
        {
            var result = new AdminDashboardData
            {
                TotalUsers = await Connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM workflowmgmt.users WHERE is_active = true", transaction: Transaction),
                ActiveWorkflows = await Connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM workflowmgmt.document_workflows WHERE status = 'active'", transaction: Transaction),
                SystemAlerts = 0,
                DepartmentStats = new List<DepartmentStatsDto>()
            };

            return result;
        }

        public async Task<FacultyDashboardData> GetFacultyDashboardData(Guid userId, int? departmentId)
        {
            var result = new FacultyDashboardData
            {
                MySyllabi = await Connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM workflowmgmt.syllabi WHERE faculty_id = @UserId", new { UserId = userId }, transaction: Transaction),
                MyLessons = await Connection.QuerySingleAsync<int>(@"
                    SELECT COUNT(*)
                    FROM workflowmgmt.lesson_plans lp
                    INNER JOIN workflowmgmt.syllabi s ON lp.syllabus_id = s.id
                    WHERE s.faculty_id = @UserId", new { UserId = userId }, transaction: Transaction),
                MySessions = await Connection.QuerySingleAsync<int>(@"
                    SELECT COUNT(*)
                    FROM workflowmgmt.sessions sess
                    INNER JOIN workflowmgmt.lesson_plans lp ON sess.lesson_plan_id = lp.id
                    INNER JOIN workflowmgmt.syllabi s ON lp.syllabus_id = s.id
                    WHERE s.faculty_id = @UserId", new { UserId = userId }, transaction: Transaction),
                PendingReviews = await Connection.QuerySingleAsync<int>(@"
                    SELECT COUNT(*)
                    FROM workflowmgmt.workflow_stage_history wsh
                    WHERE wsh.assigned_to = @UserId
                    AND wsh.processed_date IS NULL", new { UserId = userId }, transaction: Transaction),
                MyDocuments = new List<MyDocumentDto>()
            };

            return result;
        }

        public async Task<ApproverDashboardData> GetApproverDashboardData(Guid userId, string roleCode)
        {
            var result = new ApproverDashboardData
            {
                PendingApprovals = await Connection.QuerySingleAsync<int>(@"
                    SELECT COUNT(*)
                    FROM workflowmgmt.workflow_stage_history wsh
                    WHERE wsh.assigned_to = @UserId
                    AND wsh.processed_date IS NULL", new { UserId = userId }, transaction: Transaction),
                ApprovedThisMonth = await Connection.QuerySingleAsync<int>(@"
                    SELECT COUNT(*)
                    FROM workflowmgmt.workflow_stage_history wsh
                    WHERE wsh.processed_by = @UserId
                    AND wsh.action_taken = 'approved'
                    AND wsh.processed_date >= DATE_TRUNC('month', CURRENT_DATE)", new { UserId = userId }, transaction: Transaction),
                RejectedThisMonth = await Connection.QuerySingleAsync<int>(@"
                    SELECT COUNT(*)
                    FROM workflowmgmt.workflow_stage_history wsh
                    WHERE wsh.processed_by = @UserId
                    AND wsh.action_taken = 'rejected'
                    AND wsh.processed_date >= DATE_TRUNC('month', CURRENT_DATE)", new { UserId = userId }, transaction: Transaction),
                PendingDocuments = new List<PendingApprovalDto>()
            };

            return result;
        }

        // Utility methods
        public async Task<int> GetTotalDocumentsByType(string documentType, int? departmentId = null)
        {
            var query = documentType.ToLower() switch
            {
                "syllabus" => "SELECT COUNT(*) FROM workflowmgmt.syllabi WHERE (@DepartmentId IS NULL OR department_id = @DepartmentId)",
                "lesson" => @"SELECT COUNT(*) FROM workflowmgmt.lesson_plans lp
                             INNER JOIN workflowmgmt.syllabi s ON lp.syllabus_id = s.id
                             WHERE (@DepartmentId IS NULL OR s.department_id = @DepartmentId)",
                "session" => @"SELECT COUNT(*) FROM workflowmgmt.sessions sess
                              INNER JOIN workflowmgmt.lesson_plans lp ON sess.lesson_plan_id = lp.id
                              INNER JOIN workflowmgmt.syllabi s ON lp.syllabus_id = s.id
                              WHERE (@DepartmentId IS NULL OR s.department_id = @DepartmentId)",
                _ => "SELECT 0"
            };

            return await Connection.QuerySingleAsync<int>(query, new { DepartmentId = departmentId }, transaction: Transaction);
        }

        public async Task<int> GetPendingDocumentsByType(string documentType, int? departmentId = null)
        {
            var query = @"
                SELECT COUNT(*)
                FROM workflowmgmt.document_workflows dw
                WHERE dw.document_type = @DocumentType
                AND dw.status = 'active'
                AND (@DepartmentId IS NULL OR EXISTS (
                    SELECT 1 FROM workflowmgmt.syllabi s WHERE s.id::text = dw.document_id AND s.department_id = @DepartmentId AND dw.document_type = 'syllabus'
                    UNION
                    SELECT 1 FROM workflowmgmt.lesson_plans lp
                    INNER JOIN workflowmgmt.syllabi s ON lp.syllabus_id = s.id
                    WHERE lp.id::text = dw.document_id AND s.department_id = @DepartmentId AND dw.document_type = 'lesson'
                    UNION
                    SELECT 1 FROM workflowmgmt.sessions sess
                    INNER JOIN workflowmgmt.lesson_plans lp ON sess.lesson_plan_id = lp.id
                    INNER JOIN workflowmgmt.syllabi s ON lp.syllabus_id = s.id
                    WHERE sess.id::text = dw.document_id AND s.department_id = @DepartmentId AND dw.document_type = 'session'
                ))";

            return await Connection.QuerySingleAsync<int>(query, new { DocumentType = documentType, DepartmentId = departmentId }, transaction: Transaction);
        }

        public async Task<int> GetApprovedDocumentsThisMonth(int? departmentId = null)
        {
            var query = @"
                SELECT COUNT(DISTINCT dw.id)
                FROM workflowmgmt.document_workflows dw
                INNER JOIN workflowmgmt.workflow_stage_history wsh ON dw.id = wsh.document_workflow_id
                WHERE wsh.action_taken = 'approved'
                AND wsh.processed_date >= DATE_TRUNC('month', CURRENT_DATE)
                AND (@DepartmentId IS NULL OR EXISTS (
                    SELECT 1 FROM workflowmgmt.syllabi s WHERE s.id::text = dw.document_id AND s.department_id = @DepartmentId AND dw.document_type = 'syllabus'
                    UNION
                    SELECT 1 FROM workflowmgmt.lesson_plans lp
                    INNER JOIN workflowmgmt.syllabi s ON lp.syllabus_id = s.id
                    WHERE lp.id::text = dw.document_id AND s.department_id = @DepartmentId AND dw.document_type = 'lesson'
                    UNION
                    SELECT 1 FROM workflowmgmt.sessions sess
                    INNER JOIN workflowmgmt.lesson_plans lp ON sess.lesson_plan_id = lp.id
                    INNER JOIN workflowmgmt.syllabi s ON lp.syllabus_id = s.id
                    WHERE sess.id::text = dw.document_id AND s.department_id = @DepartmentId AND dw.document_type = 'session'
                ))";

            return await Connection.QuerySingleAsync<int>(query, new { DepartmentId = departmentId }, transaction: Transaction);
        }

        public async Task<int> GetUserDocumentCount(Guid userId, string documentType)
        {
            var query = documentType.ToLower() switch
            {
                "syllabus" => "SELECT COUNT(*) FROM workflowmgmt.syllabi WHERE faculty_id = @UserId",
                "lesson" => @"SELECT COUNT(*) FROM workflowmgmt.lesson_plans lp
                             INNER JOIN workflowmgmt.syllabi s ON lp.syllabus_id = s.id
                             WHERE s.faculty_id = @UserId",
                "session" => @"SELECT COUNT(*) FROM workflowmgmt.sessions sess
                              INNER JOIN workflowmgmt.lesson_plans lp ON sess.lesson_plan_id = lp.id
                              INNER JOIN workflowmgmt.syllabi s ON lp.syllabus_id = s.id
                              WHERE s.faculty_id = @UserId",
                _ => "SELECT 0"
            };

            return await Connection.QuerySingleAsync<int>(query, new { UserId = userId }, transaction: Transaction);
        }

        public async Task<int> GetPendingApprovalsForUser(Guid userId, string roleCode)
        {
            var query = @"
                SELECT COUNT(*)
                FROM workflowmgmt.workflow_stage_history wsh
                WHERE wsh.assigned_to = @UserId
                AND wsh.processed_date IS NULL";

            return await Connection.QuerySingleAsync<int>(query, new { UserId = userId }, transaction: Transaction);
        }
    }
}

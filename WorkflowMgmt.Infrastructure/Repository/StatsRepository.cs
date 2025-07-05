using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Domain.Models.Stats;
using WorkflowMgmt.Domain.Models.User;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class StatsRepository : RepositoryTranBase, IStatsRepository
    {
        public StatsRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<DepartmentStatsDto> GetDepartmentStatsAsync()
        {
            var sql = @"
                SELECT 
                    COUNT(*) as TotalDepartments,
                    COUNT(CASE WHEN status = 'Active' THEN 1 END) as ActiveDepartments,
                    COALESCE(SUM(
                        CASE 
                            WHEN programs_offered IS NOT NULL AND programs_offered != '' 
                            THEN array_length(string_to_array(programs_offered, ','), 1)
                            ELSE 0 
                        END
                    ), 0) as TotalPrograms,
                    COUNT(CASE 
                        WHEN accreditation IS NOT NULL 
                        AND UPPER(accreditation) LIKE '%NBA%' 
                        THEN 1 
                    END) as NbaAccredited
                FROM workflowmgmt.departments";

            return await Connection.QuerySingleAsync<DepartmentStatsDto>(sql, transaction: Transaction);
        }

        public async Task<CourseStatsDto> GetCourseStatsAsync()
        {
            var sql = @"
                WITH course_stats AS (
                  SELECT 
                    -- Basic counts
                    COUNT(*) as total_courses,
                    COUNT(CASE WHEN c.status = 'Active' THEN 1 END) as active_courses,
                    COUNT(CASE WHEN c.status = 'Inactive' THEN 1 END) as inactive_courses,
                    COUNT(CASE WHEN c.status = 'Draft' THEN 1 END) as draft_courses,
                    
                    -- Credit statistics
                    COALESCE(SUM(c.credits), 0) as total_credits,
                    COALESCE(ROUND(AVG(c.credits::numeric), 2), 0) as average_credits
                  FROM workflowmgmt.courses c
                  WHERE c.is_active = true
                ),
                courses_by_type AS (
                  SELECT 
                    c.course_type,
                    COUNT(*) as count
                  FROM workflowmgmt.courses c
                  WHERE c.is_active = true
                  GROUP BY c.course_type
                ),
                courses_by_level AS (
                  SELECT 
                    c.level,
                    COUNT(*) as count
                  FROM workflowmgmt.courses c
                  WHERE c.is_active = true
                  GROUP BY c.level
                ),
                courses_by_department AS (
                  SELECT 
                    d.name as department_name,
                    COUNT(c.id) as count
                  FROM workflowmgmt.departments d
                  LEFT JOIN workflowmgmt.courses c ON d.id = c.department_id AND c.is_active = true
                  WHERE d.is_active = true
                  GROUP BY d.id, d.name
                  HAVING COUNT(c.id) > 0  -- Only include departments with courses
                )
                SELECT 
                  -- Basic statistics
                  cs.total_courses as TotalCourses,
                  cs.active_courses as ActiveCourses,
                  cs.inactive_courses as InactiveCourses,
                  cs.draft_courses as DraftCourses,
                  cs.total_credits as TotalCredits,
                  cs.average_credits as AverageCredits,
                  
                  -- Aggregated data as JSON objects
                  COALESCE(
                    (SELECT json_object_agg(course_type, count) FROM courses_by_type),
                    '{}'::json
                  ) as courses_by_type_json,
                  
                  COALESCE(
                    (SELECT json_object_agg(level, count) FROM courses_by_level),
                    '{}'::json
                  ) as courses_by_level_json,
                  
                  COALESCE(
                    (SELECT json_object_agg(department_name, count) FROM courses_by_department),
                    '{}'::json
                  ) as courses_by_department_json

                FROM course_stats cs";

            var result = await Connection.QuerySingleAsync<CourseStatsQueryResultDto>(sql, transaction: Transaction);
            var courseStats = new CourseStatsDto
            {
                TotalCourses = result.TotalCourses,
                ActiveCourses = result.ActiveCourses,
                InactiveCourses = result.InactiveCourses,
                DraftCourses = result.DraftCourses,
                TotalCredits = result.TotalCredits,
                AverageCredits = result.AverageCredits,
                CoursesByType = result.courses_by_type_json != null
                ? JsonSerializer.Deserialize<Dictionary<string, int>>(result.courses_by_type_json) ?? new()
                : new(),
                        CoursesByLevel = result.courses_by_level_json != null
                ? JsonSerializer.Deserialize<Dictionary<string, int>>(result.courses_by_level_json) ?? new()
                : new(),
                        CoursesByDepartment = result.courses_by_department_json != null
                ? JsonSerializer.Deserialize<Dictionary<string, int>>(result.courses_by_department_json) ?? new()
                : new()
            };

            return courseStats;
        }

        public async Task<SemesterStatsDto> GetSemesterStatsAsync()
        {
            var sql = @"
                WITH semester_stats AS (
                  SELECT
                    -- Basic counts
                    COUNT(*) as total_semesters,
                    COUNT(CASE WHEN s.is_active = true THEN 1 END) as active_semesters,
                    COUNT(CASE WHEN s.status = 'Upcoming' THEN 1 END) as upcoming_semesters,
                    COUNT(CASE WHEN s.status = 'Ongoing' THEN 1 END) as ongoing_semesters,
                    COUNT(CASE WHEN s.status = 'Completed' THEN 1 END) as completed_semesters,

                    -- Student and duration statistics
                    COALESCE(SUM(s.total_students), 0) as total_students,
                    COALESCE(ROUND(AVG(s.duration_weeks::numeric), 2), 0) as average_duration_weeks,
                    COUNT(CASE WHEN s.exam_scheduled = true THEN 1 END) as semesters_with_exams
                  FROM workflowmgmt.semesters s
                  WHERE s.is_active = true
                ),
                semesters_by_status AS (
                  SELECT
                    s.status,
                    COUNT(*) as count
                  FROM workflowmgmt.semesters s
                  WHERE s.is_active = true
                  GROUP BY s.status
                ),
                semesters_by_level AS (
                  SELECT
                    s.level,
                    COUNT(*) as count
                  FROM workflowmgmt.semesters s
                  WHERE s.is_active = true
                  GROUP BY s.level
                ),
                semesters_by_department AS (
                  SELECT
                    d.name as department_name,
                    COUNT(*) as count
                  FROM workflowmgmt.semesters s
                  JOIN workflowmgmt.departments d ON s.department_id = d.id
                  WHERE s.is_active = true
                  GROUP BY d.id, d.name
                ),
                semesters_by_academic_year AS (
                  SELECT
                    s.academic_year,
                    COUNT(*) as count
                  FROM workflowmgmt.semesters s
                  WHERE s.is_active = true
                  GROUP BY s.academic_year
                )

                SELECT
                  -- Basic statistics
                  ss.total_semesters as TotalSemesters,
                  ss.active_semesters as ActiveSemesters,
                  ss.upcoming_semesters as UpcomingSemesters,
                  ss.ongoing_semesters as OngoingSemesters,
                  ss.completed_semesters as CompletedSemesters,
                  ss.total_students as TotalStudents,
                  ss.average_duration_weeks as AverageDurationWeeks,
                  ss.semesters_with_exams as SemestersWithExams,

                  -- Aggregated data as JSON objects
                  COALESCE(
                    (SELECT json_object_agg(status, count) FROM semesters_by_status),
                    '{}'::json
                  ) as semesters_by_status_json,

                  COALESCE(
                    (SELECT json_object_agg(level, count) FROM semesters_by_level),
                    '{}'::json
                  ) as semesters_by_level_json,

                  COALESCE(
                    (SELECT json_object_agg(department_name, count) FROM semesters_by_department),
                    '{}'::json
                  ) as semesters_by_department_json,

                  COALESCE(
                    (SELECT json_object_agg(academic_year, count) FROM semesters_by_academic_year),
                    '{}'::json
                  ) as semesters_by_academic_year_json

                FROM semester_stats ss";

            var result = await Connection.QuerySingleAsync<SemesterStatsQueryResultDto>(sql, transaction: Transaction);
            var semesterStats = new SemesterStatsDto
            {
                TotalSemesters = result.TotalSemesters,
                ActiveSemesters = result.ActiveSemesters,
                UpcomingSemesters = result.UpcomingSemesters,
                OngoingSemesters = result.OngoingSemesters,
                CompletedSemesters = result.CompletedSemesters,
                TotalStudents = result.TotalStudents,
                AverageDurationWeeks = result.AverageDurationWeeks,
                SemestersWithExams = result.SemestersWithExams,
                SemestersByStatus = result.semesters_by_status_json != null
                    ? JsonSerializer.Deserialize<Dictionary<string, int>>(result.semesters_by_status_json) ?? new()
                    : new(),
                SemestersByLevel = result.semesters_by_level_json != null
                    ? JsonSerializer.Deserialize<Dictionary<string, int>>(result.semesters_by_level_json) ?? new()
                    : new(),
                SemestersByDepartment = result.semesters_by_department_json != null
                    ? JsonSerializer.Deserialize<Dictionary<string, int>>(result.semesters_by_department_json) ?? new()
                    : new(),
                SemestersByAcademicYear = result.semesters_by_academic_year_json != null
                    ? JsonSerializer.Deserialize<Dictionary<string, int>>(result.semesters_by_academic_year_json) ?? new()
                    : new()
            };

            return semesterStats;
        }

        // Keep individual methods for backward compatibility if needed
        public async Task<int> GetTotalDepartmentsAsync()
        {
            var sql = "SELECT COUNT(*) FROM workflowmgmt.departments";
            return await Connection.QuerySingleAsync<int>(sql, transaction: Transaction);
        }

        public async Task<int> GetActiveDepartmentsAsync()
        {
            var sql = "SELECT COUNT(*) FROM workflowmgmt.departments WHERE status = 'Active'";
            return await Connection.QuerySingleAsync<int>(sql, transaction: Transaction);
        }

        public async Task<int> GetTotalProgramsAsync()
        {
            var sql = @"
                SELECT COALESCE(SUM(
                    CASE
                        WHEN programs_offered IS NOT NULL AND programs_offered != ''
                        THEN array_length(string_to_array(programs_offered, ','), 1)
                        ELSE 0
                    END
                ), 0) as total_programs
                FROM workflowmgmt.departments";

            return await Connection.QuerySingleAsync<int>(sql, transaction: Transaction);
        }

        public async Task<int> GetNbaAccreditedDepartmentsAsync()
        {
            var sql = @"
                SELECT COUNT(*)
                FROM workflowmgmt.departments
                WHERE accreditation IS NOT NULL
                AND UPPER(accreditation) LIKE '%NBA%'";

            return await Connection.QuerySingleAsync<int>(sql, transaction: Transaction);
        }

        public async Task<UserStatsDto> GetUserStatsAsync()
        {
            // Get basic user statistics
            var basicStatsSql = @"
                SELECT
                    COUNT(*) as TotalUsers,
                    COUNT(CASE WHEN is_active = true THEN 1 END) as ActiveUsers,
                    COUNT(CASE WHEN last_login >= @RecentDate THEN 1 END) as RecentLogins
                FROM workflowmgmt.users";

            var recentDate = DateTime.UtcNow.AddDays(-30); // Last 30 days
            var basicStats = await Connection.QuerySingleAsync<UserStatsDto>(basicStatsSql, new { RecentDate = recentDate }, transaction: Transaction);

            // Get users by role with proper JOIN
            var rolesSql = @"
                SELECT
                    COALESCE(r.name, 'Unassigned') as RoleName,
                    COUNT(u.id) as Count
                FROM workflowmgmt.roles r
                LEFT JOIN workflowmgmt.users u ON r.id = u.role_id
                WHERE r.is_active = true
                GROUP BY r.id, r.name
                HAVING COUNT(u.id) > 0
                UNION ALL
                SELECT
                    'Unassigned' as RoleName,
                    COUNT(u.id) as Count
                FROM workflowmgmt.users u
                WHERE u.role_id IS NULL OR u.role_id NOT IN (SELECT id FROM workflowmgmt.roles WHERE is_active = true)
                HAVING COUNT(u.id) > 0
                ORDER BY RoleName";

            var usersByRole = await Connection.QueryAsync<UsersByRoleDto>(rolesSql, transaction: Transaction);
            basicStats.UsersByRole = usersByRole.ToArray();

            // Get users by department with proper JOIN
            var departmentsSql = @"
                SELECT
                    COALESCE(d.name, 'Unassigned') as DepartmentName,
                    COUNT(u.id) as Count
                FROM workflowmgmt.departments d
                LEFT JOIN workflowmgmt.users u ON d.id = u.department_id
                WHERE d.is_active = true
                GROUP BY d.id, d.name
                HAVING COUNT(u.id) > 0
                UNION ALL
                SELECT
                    'Unassigned' as DepartmentName,
                    COUNT(u.id) as Count
                FROM workflowmgmt.users u
                WHERE u.department_id IS NULL OR u.department_id NOT IN (SELECT id FROM workflowmgmt.departments WHERE is_active = true)
                HAVING COUNT(u.id) > 0
                ORDER BY DepartmentName";

            var usersByDepartment = await Connection.QueryAsync<UsersByDepartmentDto>(departmentsSql, transaction: Transaction);
            basicStats.UsersByDepartment = usersByDepartment.ToArray();

            return basicStats;
        }
    }
}

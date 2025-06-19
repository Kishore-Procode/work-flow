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
    }
}

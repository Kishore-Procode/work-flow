using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities.Courses;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class CourseRepository : RepositoryTranBase, ICourseRepository
    {
        public CourseRepository(IDbTransaction transaction) : base(transaction) { }

        public async Task<List<CourseDTO>> GetAllCourses()
        {
            var query = @"
                SELECT
                    id,
                    name as CourseName,
                    code as CourseCode,
                    description,
                    credits,
                    department_id as DepartmentId,
                    course_type as CourseType,
                    level,
                    duration_weeks as DurationWeeks,
                    max_capacity as MaxCapacity,
                    status,
                    prerequisites,
                    learning_objectives as LearningObjectives,
                    learning_outcomes as LearningOutcomes,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy,
                    is_active as IsActive
                FROM workflowmgmt.courses
                ORDER BY code";

            var courses = await Connection.QueryAsync<CourseDTO>(query, transaction: Transaction);
            return courses.ToList();
        }

        public async Task<CourseDTO?> GetCourseById(int id)
        {
            var query = @"
                SELECT
                    id,
                    name as CourseName,
                    code as CourseCode,
                    description,
                    credits,
                    department_id as DepartmentId,
                    course_type as CourseType,
                    level,
                    duration_weeks as DurationWeeks,
                    max_capacity as MaxCapacity,
                    status,
                    prerequisites,
                    learning_objectives as LearningObjectives,
                    learning_outcomes as LearningOutcomes,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy,
                    is_active as IsActive
                FROM workflowmgmt.courses
                WHERE id = @Id AND is_active = true";

            return await Connection.QuerySingleOrDefaultAsync<CourseDTO>(query, new { Id = id }, Transaction);
        }

        public async Task<List<CourseDTO>> GetCoursesByDepartmentAsync(int departmentId)
        {
            var query = @"
                SELECT
                    id,
                    name as CourseName,
                    code as CourseCode,
                    description,
                    credits,
                    department_id as DepartmentId,
                    course_type as CourseType,
                    level,
                    duration_weeks as DurationWeeks,
                    max_capacity as MaxCapacity,
                    status,
                    prerequisites,
                    learning_objectives as LearningObjectives,
                    learning_outcomes as LearningOutcomes,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy,
                    is_active as IsActive
                FROM workflowmgmt.courses
                WHERE department_id = @DepartmentId AND is_active = true
                ORDER BY code";

            var courses = await Connection.QueryAsync<CourseDTO>(query, new { DepartmentId = departmentId }, transaction: Transaction);
            return courses.ToList();
        }

        public async Task<bool> IsCourseCodeExists(string code, int? excludeId = null)
        {
            var query = "SELECT 1 FROM workflowmgmt.courses WHERE code = @Code AND is_active = true" +
                        (excludeId.HasValue ? " AND id != @Id" : "");
            var result = await Connection.ExecuteScalarAsync<int?>(query, new { Code = code, Id = excludeId }, Transaction);
            return result.HasValue;
        }

        public async Task<int> InsertCourse(CourseDTO course)
        {
            if (await IsCourseCodeExists(course.CourseCode))
                throw new Exception("Course code already exists");

            var query = @"
                INSERT INTO workflowmgmt.courses (
                    name,
                    code,
                    description,
                    credits,
                    department_id,
                    course_type,
                    level,
                    duration_weeks,
                    max_capacity,
                    status,
                    prerequisites,
                    learning_objectives,
                    learning_outcomes,
                    created_date,
                    created_by,
                    is_active
                ) VALUES (
                    @CourseName,
                    @CourseCode,
                    @Description,
                    @Credits,
                    @DepartmentId,
                    @CourseType,
                    @Level,
                    @DurationWeeks,
                    @MaxCapacity,
                    @Status,
                    @Prerequisites,
                    @LearningObjectives,
                    @LearningOutcomes,
                    @CreatedDate,
                    @CreatedBy,
                    @IsActive
                )
                RETURNING id;
            ";

            try
            {
                return await Connection.ExecuteScalarAsync<int>(query, course, Transaction);
            }
            catch (PostgresException ex) when (ex.SqlState == "23505")
            {
                throw new Exception("Duplicate course code detected.");
            }
        }

        public async Task<bool> UpdateCourse(CourseDTO course)
        {
            if (await IsCourseCodeExists(course.CourseCode, course.Id))
                throw new Exception("Course code already exists");

            var query = @"
                UPDATE workflowmgmt.courses SET
                    name = @CourseName,
                    code = @CourseCode,
                    description = @Description,
                    credits = @Credits,
                    department_id = @DepartmentId,
                    course_type = @CourseType,
                    level = @Level,
                    duration_weeks = @DurationWeeks,
                    max_capacity = @MaxCapacity,
                    status = @Status,
                    prerequisites = @Prerequisites,
                    learning_objectives = @LearningObjectives,
                    learning_outcomes = @LearningOutcomes,
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy,
                    is_active = @IsActive
                WHERE id = @Id;
            ";

            var rowsAffected = await Connection.ExecuteAsync(query, course, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteOrRestoreCourse(int id, string modifiedBy, bool isRestore)
        {
            var query = @"
                UPDATE workflowmgmt.courses SET 
                    is_active = @IsActive,
                    status = @Status,
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE id = @Id;
            ";

            var result = await Connection.ExecuteAsync(query, new
            {
                Id = id,
                IsActive = isRestore,
                Status = isRestore ? "Active" : "Inactive",
                ModifiedDate = DateTime.Now,
                ModifiedBy = modifiedBy
            }, Transaction);

            return result > 0;
        }
    }
}

using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using WorkflowMgmt.Domain.Entities.Semesters;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class SemesterRepository : RepositoryTranBase, ISemesterRepository
    {
        public SemesterRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<SemesterDTO>> GetAllSemesters()
        {
            var sql = @"
                SELECT
                    s.id as Id,
                    s.name as Name,
                    s.code as Code,
                    academic_year as AcademicYear,
                    s.department_id as DepartmentId,
                    s.start_date as StartDate,
                    s.end_date as EndDate,
                    s.duration_weeks as DurationWeeks,
                    s.level as Level,
                    s.total_students as TotalStudents,
                    s.status as Status,
                    s.description as Description,
                    s.exam_scheduled as ExamScheduled,
                    s.created_date as CreatedDate,
                    s.modified_date as ModifiedDate,
                    s.created_by as CreatedBy,
                    s.modified_by as ModifiedBy,
                    s.is_active as IsActive,
                    d.name as departmentName,
                    d.code as departmentCode,
                    c.name as courseName,
                    c.code as courseCode
                FROM workflowmgmt.semesters s
                JOIN workflowmgmt.departments d on d.id = s.department_id
                LEFT JOIN workflowmgmt.courses c on c.id = s.course_id
                ORDER BY s.academic_year DESC, s.name";
            var semesters = await Connection.QueryAsync<SemesterDTO>(sql, transaction: Transaction);
            return semesters.ToList();
        }

        public async Task<SemesterDTO?> GetSemesterById(int id)
        {
            var sql = @"
                SELECT
                    s.id as Id,
                    s.name as Name,
                    s.code as Code,
                    academic_year as AcademicYear,
                    s.department_id as DepartmentId,
                    s.start_date as StartDate,
                    s.end_date as EndDate,
                    s.duration_weeks as DurationWeeks,
                    s.level as Level,
                    s.total_students as TotalStudents,
                    s.status as Status,
                    s.description as Description,
                    s.exam_scheduled as ExamScheduled,
                    s.created_date as CreatedDate,
                    s.modified_date as ModifiedDate,
                    s.created_by as CreatedBy,
                    s.modified_by as ModifiedBy,
                    s.is_active as IsActive,
                    d.name as departmentName,
                    d.code as departmentCode,
                    c.name as courseName,
                    c.code as courseCode
                FROM workflowmgmt.semesters s
                JOIN workflowmgmt.departments d on d.id = s.department_id
                LEFT JOIN workflowmgmt.courses c on c.semester_id = s.id
                WHERE s.id = @Id ";
            return await Connection.QueryFirstOrDefaultAsync<SemesterDTO>(sql, new { Id = id }, Transaction);
        }

        public async Task<List<SemesterDTO>> GetSemestersByDepartmentAsync(int departmentId)
        {
            var sql = @"
                SELECT
                    s.id as Id,
                    s.name as Name,
                    s.code as Code,
                    academic_year as AcademicYear,
                    s.department_id as DepartmentId,
                    s.start_date as StartDate,
                    s.end_date as EndDate,
                    s.duration_weeks as DurationWeeks,
                    s.level as Level,
                    s.total_students as TotalStudents,
                    s.status as Status,
                    s.description as Description,
                    s.exam_scheduled as ExamScheduled,
                    s.created_date as CreatedDate,
                    s.modified_date as ModifiedDate,
                    s.created_by as CreatedBy,
                    s.modified_by as ModifiedBy,
                    s.is_active as IsActive,
                    d.name as departmentName,
                    d.code as departmentCode,
                    c.name as courseName,
                    c.code as courseCode
                FROM workflowmgmt.semesters s
                JOIN workflowmgmt.departments d on d.id = s.department_id
                LEFT JOIN workflowmgmt.courses c on c.semester_id = s.id
                WHERE s.department_id = @DepartmentId AND s.is_active = true
                ORDER BY s.academic_year DESC, s.name";

            var semesters = await Connection.QueryAsync<SemesterDTO>(sql, new { DepartmentId = departmentId }, Transaction);
            return semesters.ToList();
        }

        public async Task<List<SemesterDTO>> GetSemestersByDepartmentAndCourseAsync(int departmentId, int courseId)
        {
            // Find the semester that contains the selected course
            // Since courses have semester_id, we get the semester from the course
            var sql = @"
                SELECT DISTINCT
                    s.id as Id,
                    s.name as Name,
                    s.code as Code,
                    s.academic_year as AcademicYear,
                    s.department_id as DepartmentId,
                    s.start_date as StartDate,
                    s.end_date as EndDate,
                    s.duration_weeks as DurationWeeks,
                    s.level as Level,
                    s.total_students as TotalStudents,
                    s.status as Status,
                    s.description as Description,
                    s.exam_scheduled as ExamScheduled,
                    s.created_date as CreatedDate,
                    s.modified_date as ModifiedDate,
                    s.created_by as CreatedBy,
                    s.modified_by as ModifiedBy,
                    s.is_active as IsActive,
                    d.name as departmentName,
                    d.code as departmentCode,
                    c.name as courseName,
                    c.code as courseCode
                FROM workflowmgmt.courses c
                JOIN workflowmgmt.semesters s on s.id = c.semester_id
                JOIN workflowmgmt.departments d on d.id = s.department_id
                WHERE c.id = @CourseId
                AND c.department_id = @DepartmentId
                AND s.is_active = true
                AND c.is_active = true
                ORDER BY s.academic_year DESC, s.name";

            var semesters = await Connection.QueryAsync<SemesterDTO>(sql, new { DepartmentId = departmentId, CourseId = courseId }, Transaction);
            return semesters.ToList();
        }

        public async Task<int> InsertSemester(SemesterDTO semester)
        {
            var sql = @"
                INSERT INTO workflowmgmt.semesters (
                    name,
                    code,
                    academic_year,
                    department_id,
                    course_id,
                    start_date,
                    end_date,
                    duration_weeks,
                    level,
                    total_students,
                    status,
                    description,
                    exam_scheduled,
                    created_date,
                    created_by
                ) VALUES (
                    @Name,
                    @Code,
                    @AcademicYear,
                    @DepartmentId,
                    @CourseId,
                    @StartDate,
                    @EndDate,
                    @DurationWeeks,
                    @Level,
                    @TotalStudents,
                    @Status,
                    @Description,
                    @ExamScheduled,
                    NOW(),
                    @CreatedBy
                ) RETURNING id;
            ";

            return await Connection.ExecuteScalarAsync<int>(sql, semester, Transaction);
        }

        public async Task<bool> UpdateSemester(SemesterDTO semester)
        {
            var sql = @"
                UPDATE workflowmgmt.semesters SET
                    name = @Name,
                    code = @Code,
                    academic_year = @AcademicYear,
                    department_id = @DepartmentId,
                    start_date = @StartDate,
                    end_date = @EndDate,
                    duration_weeks = @DurationWeeks,
                    level = @Level,
                    total_students = @TotalStudents,
                    status = @Status,
                    description = @Description,
                    exam_scheduled = @ExamScheduled,
                    modified_date = NOW(),
                    modified_by = @ModifiedBy,
                    is_active = @IsActive
                WHERE id = @Id;
            ";

            var affectedRows = await Connection.ExecuteAsync(sql, semester, Transaction);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteOrRestoreSemester(int id, string modifiedBy, bool isRestore)
        {
            var sql = @"
                UPDATE workflowmgmt.semesters
                SET is_active = @IsActive,
                    status = @Status,
                    modified_by = @ModifiedBy,
                    modified_date = NOW()
                WHERE id = @Id AND is_active != @IsActive;
            ";

            var result = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                ModifiedBy = modifiedBy,
                IsActive = isRestore,
                Status = isRestore ? "Upcoming" : "Inactive"
            }, Transaction);

            return result > 0;
        }
    }
}

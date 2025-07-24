using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    s.id,
                    s.created_date as createdDate,
                    s.modified_date as modifiedDate,
                    s.created_by as createdBy,
                    s.modified_by as modifiedBy,
                    s.is_active as isActive,
                    s.name,
                    s.code,
                    s.academic_year_id,
                    s.department_id,
                    COALESCE(array_to_string(s.course_id, ','), '') as course_id_string,
                    s.start_date,
                    s.end_date,
                    s.duration_weeks,
                    s.level_id,
                    s.total_students,
                    s.status,
                    s.description,
                    s.exam_scheduled,
                    d.name as department_name,
                    d.code as department_code,
                    l.name as level_name,
                    l.code as level_code,
                    ay.name as academic_year_name,
                    ay.code as academic_year_code
                FROM workflowmgmt.semesters s
                LEFT JOIN workflowmgmt.departments d ON s.department_id = d.id
                LEFT JOIN workflowmgmt.levels l ON s.level_id = l.id
                LEFT JOIN workflowmgmt.academic_years ay ON s.academic_year_id = ay.id
                WHERE s.is_active = true
                ORDER BY ay.start_year DESC, s.name";

            var semesters = await Connection.QueryAsync<SemesterDTO>(sql, transaction: Transaction);
            var semesterList = semesters.ToList();

            // Get courses for each semester
            foreach (var semester in semesterList)
            {
                try
                {
                    // Initialize courses list
                    semester.courses = new List<CourseInfo>();

                    if (semester.course_id != null && semester.course_id.Length > 0)
                    {
                        var coursesSql = @"
                            SELECT id, name, code
                            FROM workflowmgmt.courses
                            WHERE id = ANY(@CourseIds) AND is_active = true";

                        var courses = await Connection.QueryAsync<CourseInfo>(
                            coursesSql,
                            new { CourseIds = semester.course_id },
                            Transaction);

                        semester.courses = courses.ToList();
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but don't fail the entire operation
                    Console.WriteLine($"Error loading courses for semester {semester.Id}: {ex.Message}");
                    semester.courses = new List<CourseInfo>();
                }
            }

            return semesterList;
        }

        public async Task<SemesterDTO?> GetSemesterById(int id)
        {
            var sql = @"
                SELECT
                    s.id,
                    s.created_date as createdDate,
                    s.modified_date as modifiedDate,
                    s.created_by as createdBy,
                    s.modified_by as modifiedBy,
                    s.is_active as isActive,
                    s.name,
                    s.code,
                    s.academic_year_id,
                    s.department_id,
                    COALESCE(array_to_string(s.course_id, ','), '') as course_id_string,
                    s.start_date,
                    s.end_date,
                    s.duration_weeks,
                    s.level_id,
                    s.total_students,
                    s.status,
                    s.description,
                    s.exam_scheduled,
                    d.name as department_name,
                    d.code as department_code,
                    l.name as level_name,
                    l.code as level_code,
                    ay.name as academic_year_name,
                    ay.code as academic_year_code
                FROM workflowmgmt.semesters s
                LEFT JOIN workflowmgmt.departments d ON s.department_id = d.id
                LEFT JOIN workflowmgmt.levels l ON s.level_id = l.id
                LEFT JOIN workflowmgmt.academic_years ay ON s.academic_year_id = ay.id
                WHERE s.id = @Id";

            var semester = await Connection.QueryFirstOrDefaultAsync<SemesterDTO>(sql, new { Id = id }, Transaction);
            
            if (semester != null && semester.course_id != null && semester.course_id.Length > 0)
            {
                var coursesSql = @"
                    SELECT id, name, code 
                    FROM workflowmgmt.courses 
                    WHERE id = ANY(@CourseIds)";
                
                var courses = await Connection.QueryAsync<CourseInfo>(
                    coursesSql, 
                    new { CourseIds = semester.course_id }, 
                    Transaction);
                
                semester.courses = courses.ToList();
            }
            else if (semester != null)
            {
                semester.courses = new List<CourseInfo>();
            }

            return semester;
        }

        public async Task<List<SemesterDTO>> GetSemestersByDepartmentAsync(int departmentId)
        {
            var sql = @"
                SELECT
                    s.id,
                    s.created_date as createdDate,
                    s.modified_date as modifiedDate,
                    s.created_by as createdBy,
                    s.modified_by as modifiedBy,
                    s.is_active as isActive,
                    s.name,
                    s.code,
                    s.academic_year_id,
                    s.department_id,
                    COALESCE(array_to_string(s.course_id, ','), '') as course_id_string,
                    s.start_date,
                    s.end_date,
                    s.duration_weeks,
                    s.level_id,
                    s.total_students,
                    s.status,
                    s.description,
                    s.exam_scheduled,
                    d.name as department_name,
                    d.code as department_code,
                    l.name as level_name,
                    l.code as level_code,
                    ay.name as academic_year_name,
                    ay.code as academic_year_code
                FROM workflowmgmt.semesters s
                LEFT JOIN workflowmgmt.departments d ON s.department_id = d.id
                LEFT JOIN workflowmgmt.levels l ON s.level_id = l.id
                LEFT JOIN workflowmgmt.academic_years ay ON s.academic_year_id = ay.id
                WHERE s.department_id = @DepartmentId AND s.is_active = true
                ORDER BY ay.start_year DESC, s.name";

            var semesters = await Connection.QueryAsync<SemesterDTO>(sql, new { DepartmentId = departmentId }, Transaction);
            var semesterList = semesters.ToList();

            // Get courses for each semester
            foreach (var semester in semesterList)
            {
                if (semester.course_id != null && semester.course_id.Length > 0)
                {
                    var coursesSql = @"
                        SELECT id, name, code 
                        FROM workflowmgmt.courses 
                        WHERE id = ANY(@CourseIds)";
                    
                    var courses = await Connection.QueryAsync<CourseInfo>(
                        coursesSql, 
                        new { CourseIds = semester.course_id }, 
                        Transaction);
                    
                    semester.courses = courses.ToList();
                }
                else
                {
                    semester.courses = new List<CourseInfo>();
                }
            }

            return semesterList;
        }

        public async Task<int> InsertSemester(SemesterDTO semester)
        {
            var sql = @"
                INSERT INTO workflowmgmt.semesters (
                    name,
                    code,
                    academic_year_id,
                    department_id,
                    course_id,
                    start_date,
                    end_date,
                    duration_weeks,
                    level_id,
                    total_students,
                    status,
                    description,
                    exam_scheduled,
                    is_active,
                    created_date,
                    created_by
                ) VALUES (
                    @name,
                    @code,
                    @academic_year_id,
                    @department_id,
                    @course_id,
                    @start_date,
                    @end_date,
                    @duration_weeks,
                    @level_id,
                    @total_students,
                    @status,
                    @description,
                    @exam_scheduled,
                    @is_active,
                    NOW(),
                    @CreatedBy
                )
                RETURNING id;
            ";

            var semesterId = await Connection.ExecuteScalarAsync<int>(sql, semester, Transaction);
            return semesterId;
        }

        public async Task<bool> UpdateSemester(SemesterDTO semester)
        {
            var sql = @"
                UPDATE workflowmgmt.semesters SET
                name = @name,
                code = @code,
                academic_year_id = @academic_year_id,
                department_id = @department_id,
                course_id = @course_id,
                start_date = @start_date,
                end_date = @end_date,
                duration_weeks = @duration_weeks,
                level_id = @level_id,
                total_students = @total_students,
                status = @status,
                description = @description,
                exam_scheduled = @exam_scheduled,
                is_active = @is_active,
                modified_date = NOW(),
                modified_by = @ModifiedBy
                WHERE id = @id;
           ";

            var rowsAffected = await Connection.ExecuteAsync(sql, semester, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteOrRestoreSemester(int id, string modifiedBy, bool isRestore)
        {
            var sql = @"
        UPDATE workflowmgmt.semesters
        SET is_active = @IsActive,
            status = @Status,
            modified_by = @ModifiedBy,
            modified_date = NOW()
        WHERE id = @Id AND is_active != @IsActive";

            var result = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                ModifiedBy = modifiedBy,
                IsActive = isRestore,
                Status = isRestore ? "Active" : "Inactive"
            }, Transaction);

            return result > 0;
        }
    }
}

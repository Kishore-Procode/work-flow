

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class DepartmentRepository : RepositoryTranBase, IDepartmentRepository
    {
        public DepartmentRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<DepartmentDTO>> GetAllDepartments()
        {
            var sql = @"
                SELECT d.*, l.name as level_name, l.code as level_code
                FROM workflowmgmt.departments d
                LEFT JOIN workflowmgmt.levels l ON d.level_id = l.id
                ORDER BY d.name";

            var departments = await Connection.QueryAsync<DepartmentDTO>(sql, Transaction);
            return departments.ToList();
        }

        public async Task<List<DepartmentDTO>> GetDepartmentsByLevelId(int levelId)
        {
            var sql = @"
                SELECT d.*, l.name as level_name, l.code as level_code
                FROM workflowmgmt.departments d
                LEFT JOIN workflowmgmt.levels l ON d.level_id = l.id
                WHERE d.level_id = @LevelId AND d.is_active = true
                ORDER BY d.name";

            var departments = await Connection.QueryAsync<DepartmentDTO>(sql, new { LevelId = levelId }, Transaction);
            return departments.ToList();
        }
        public async Task<DepartmentDTO?> GetDepartmentById(int id)
        {
            var sql = @"
                SELECT d.*, l.name as level_name, l.code as level_code
                FROM workflowmgmt.departments d
                LEFT JOIN workflowmgmt.levels l ON d.level_id = l.id
                WHERE d.id = @Id";

            var department = await Connection.QueryFirstOrDefaultAsync<DepartmentDTO>(sql, new { Id = id }, Transaction);
            return department;
        }
        public async Task<int> InsertDepartment(DepartmentDTO department)
        {
            var sql = @"
                INSERT INTO workflowmgmt.departments (
                    name,
                    code,
                    description,
                    head_of_department,
                    email,
                    phone,
                    established_year,
                    programs_offered,
                    accreditation,
                    level_id,
                    status,
                    created_date,
                    created_by
                ) VALUES (
                    @name,
                    @code,
                    @description,
                    @head_of_department,
                    @email,
                    @phone,
                    @established_year,
                    @programs_offered,
                    @accreditation,
                    @level_id,
                    @status,
                    NOW(),
                    @CreatedBy
                )
                RETURNING id;
            ";

            var departmentId = await Connection.ExecuteScalarAsync<int>(sql, department, Transaction);
            return departmentId;
        }

        public async Task<bool> UpdateDepartment(DepartmentDTO department)
        {
            var sql = @"
                UPDATE workflowmgmt.departments SET
                name = @name,
                code = @code,
                description = @description,
                head_of_department = @head_of_department,
                email = @email,
                phone = @phone,
                established_year = @established_year,
                programs_offered = @programs_offered,
                accreditation = @accreditation,
                level_id = @level_id,
                status = @status,
                modified_date = NOW(),
                modified_by = @ModifiedBy,
                is_active = @IsActive
                WHERE id = @id;
           ";

            var rowsAffected = await Connection.ExecuteAsync(sql, department, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteOrRestoreDepartment(int id, string modifiedBy, bool isRestore)
        {
            var sql = @"
        UPDATE workflowmgmt.departments
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

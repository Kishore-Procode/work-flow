using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Domain.IRepository;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class DepartmentRepository : RepositoryTranBase, IDepartmentRepository
    {
        public DepartmentRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<DepartmentDTO>> GetAllDepartments()
        {
            var departments = await Connection.QueryAsync<DepartmentDTO>(
                "SELECT * FROM workflowmgmt.departments order by code", Transaction);

            return departments.ToList();
        }
        public async Task<DepartmentDTO?> GetDepartmentById(int id)
        {
            var sql = "SELECT * FROM workflowmgmt.departments WHERE id = @Id";
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

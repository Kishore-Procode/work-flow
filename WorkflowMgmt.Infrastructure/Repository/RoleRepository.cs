using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Domain.Models.WorkflowManagement;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class WorkflowRoleRepository : RepositoryTranBase, IWorkflowRoleRepository
    {
        public WorkflowRoleRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<RoleDto>> GetAllAsync()
        {
            var sql = @"
                SELECT
                    id as Id,
                    name as Name,
                    code as Code,
                    description as Description,
                    permissions as Permissions,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy
                FROM workflowmgmt.roles
                ORDER BY name";

            var result = await Connection.QueryAsync<RoleDto>(sql, transaction: Transaction);
            return result.ToList();
        }

        public async Task<List<RoleDto>> GetActiveAsync()
        {
            var sql = @"
                SELECT
                    id as Id,
                    name as Name,
                    code as Code,
                    description as Description,
                    permissions as Permissions,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy
                FROM workflowmgmt.roles
                WHERE is_active = true
                ORDER BY name";

            var result = await Connection.QueryAsync<RoleDto>(sql, transaction: Transaction);
            return result.ToList();
        }

        public async Task<RoleDto?> GetByIdAsync(int id)
        {
            var sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    code as Code,
                    description as Description,
                    hierarchy_level as HierarchyLevel,
                    permissions as Permissions,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy
                FROM workflowmgmt.roles
                WHERE id = @Id";

            return await Connection.QuerySingleOrDefaultAsync<RoleDto>(sql, new { Id = id }, transaction: Transaction);
        }

        public async Task<RoleDto?> GetByCodeAsync(string code)
        {
            var sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    code as Code,
                    description as Description,
                    hierarchy_level as HierarchyLevel,
                    permissions as Permissions,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy
                FROM workflowmgmt.roles
                WHERE code = @Code";

            return await Connection.QuerySingleOrDefaultAsync<RoleDto>(sql, new { Code = code }, transaction: Transaction);
        }

        public async Task<int> CreateAsync(CreateRoleDto role)
        {
            var sql = @"
                INSERT INTO workflowmgmt.roles 
                (name, code, description, hierarchy_level, permissions, is_active, created_date, created_by)
                VALUES (@Name, @Code, @Description, @HierarchyLevel, @Permissions, true, @CreatedDate, @CreatedBy)
                RETURNING id";

            var id = await Connection.QuerySingleAsync<int>(sql, new
            {
                role.Name,
                role.Code,
                role.Description,
                role.HierarchyLevel,
                role.Permissions,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return id;
        }

        public async Task<bool> UpdateAsync(int id, UpdateRoleDto role)
        {
            var sql = @"
                UPDATE workflowmgmt.roles 
                SET name = @Name,
                    code = @Code,
                    description = @Description,
                    hierarchy_level = @HierarchyLevel,
                    permissions = @Permissions,
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                role.Name,
                role.Code,
                role.Description,
                role.HierarchyLevel,
                role.Permissions,
                ModifiedDate = DateTime.UtcNow,
                ModifiedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM workflowmgmt.roles WHERE id = @Id";
            var rowsAffected = await Connection.ExecuteAsync(sql, new { Id = id }, transaction: Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> ToggleActiveAsync(int id)
        {
            var sql = @"
                UPDATE workflowmgmt.roles 
                SET is_active = NOT is_active,
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                ModifiedDate = DateTime.UtcNow,
                ModifiedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var sql = "SELECT COUNT(1) FROM workflowmgmt.roles WHERE id = @Id";
            var count = await Connection.QuerySingleAsync<int>(sql, new { Id = id }, transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> ExistsByCodeAsync(string code, int? excludeId = null)
        {
            var sql = @"
                SELECT COUNT(1) 
                FROM workflowmgmt.roles 
                WHERE code = @Code";

            object parameters = new { Code = code };

            if (excludeId.HasValue)
            {
                sql += " AND id != @ExcludeId";
                parameters = new { Code = code, ExcludeId = excludeId.Value };
            }

            var count = await Connection.QuerySingleAsync<int>(sql, parameters, transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var sql = @"
                SELECT COUNT(1) 
                FROM workflowmgmt.roles 
                WHERE name = @Name";

            object parameters = new { Name = name };

            if (excludeId.HasValue)
            {
                sql += " AND id != @ExcludeId";
                parameters = new { Name = name, ExcludeId = excludeId.Value };
            }

            var count = await Connection.QuerySingleAsync<int>(sql, parameters, transaction: Transaction);
            return count > 0;
        }
    }
}

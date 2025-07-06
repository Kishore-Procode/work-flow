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
    public class StageAssignmentRepository : RepositoryTranBase, IStageAssignmentRepository
    {
        public StageAssignmentRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<IEnumerable<StageAssigneeDto>> GetByStageIdAsync(Guid stageId)
        {
            var sql = @"
                SELECT 
                    sa.id as Id,
                    sa.workflow_stage_id as WorkflowStageId,
                    sa.user_id as UserId,
                    sa.role_id as RoleId,
                    sa.department_id as DepartmentId,
                    sa.is_default as IsDefault,
                    CONCAT(u.first_name, ' ', u.last_name) as UserFullName,
                    u.email as UserEmail,
                    r.name as RoleName,
                    d.name as DepartmentName
                FROM workflowmgmt.stage_assignments sa
                INNER JOIN workflowmgmt.users u ON sa.user_id = u.id
                INNER JOIN workflowmgmt.roles r ON sa.role_id = r.id
                LEFT JOIN workflowmgmt.departments d ON sa.department_id = d.id
                WHERE sa.workflow_stage_id = @StageId
                ORDER BY sa.is_default DESC, u.last_name, u.first_name";

            return await Connection.QueryAsync<StageAssigneeDto>(sql, new { StageId = stageId }, transaction: Transaction);
        }

        public async Task<IEnumerable<StageAssigneeDto>> GetByUserIdAsync(Guid userId)
        {
            var sql = @"
                SELECT 
                    sa.id as Id,
                    sa.workflow_stage_id as WorkflowStageId,
                    sa.user_id as UserId,
                    sa.role_id as RoleId,
                    sa.department_id as DepartmentId,
                    sa.is_default as IsDefault,
                    CONCAT(u.first_name, ' ', u.last_name) as UserFullName,
                    u.email as UserEmail,
                    r.name as RoleName,
                    d.name as DepartmentName
                FROM workflowmgmt.stage_assignments sa
                INNER JOIN workflowmgmt.users u ON sa.user_id = u.id
                INNER JOIN workflowmgmt.roles r ON sa.role_id = r.id
                LEFT JOIN workflowmgmt.departments d ON sa.department_id = d.id
                WHERE sa.user_id = @UserId
                ORDER BY sa.is_default DESC";

            return await Connection.QueryAsync<StageAssigneeDto>(sql, new { UserId = userId }, transaction: Transaction);
        }

        public async Task<StageAssigneeDto?> GetByIdAsync(Guid id)
        {
            var sql = @"
                SELECT 
                    sa.id as Id,
                    sa.workflow_stage_id as WorkflowStageId,
                    sa.user_id as UserId,
                    sa.role_id as RoleId,
                    sa.department_id as DepartmentId,
                    sa.is_default as IsDefault,
                    CONCAT(u.first_name, ' ', u.last_name) as UserFullName,
                    u.email as UserEmail,
                    r.name as RoleName,
                    d.name as DepartmentName
                FROM workflowmgmt.stage_assignments sa
                INNER JOIN workflowmgmt.users u ON sa.user_id = u.id
                INNER JOIN workflowmgmt.roles r ON sa.role_id = r.id
                LEFT JOIN workflowmgmt.departments d ON sa.department_id = d.id
                WHERE sa.id = @Id";

            return await Connection.QuerySingleOrDefaultAsync<StageAssigneeDto>(sql, new { Id = id }, transaction: Transaction);
        }

        public async Task<Guid> CreateAsync(Guid stageId, AssignUserToStageDto assignment)
        {
            var id = Guid.NewGuid();
            var sql = @"
                INSERT INTO workflowmgmt.stage_assignments 
                (id, workflow_stage_id, user_id, role_id, department_id, is_default, is_active, created_date, created_by)
                VALUES (@Id, @StageId, @UserId, @RoleId, @DepartmentId, @IsDefault, true, @CreatedDate, @CreatedBy)";

            await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                StageId = stageId,
                assignment.UserId,
                assignment.RoleId,
                assignment.DepartmentId,
                assignment.IsDefault,
                CreatedDate = DateTime.Now,
                CreatedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return id;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var sql = "DELETE FROM workflowmgmt.stage_assignments WHERE id = @Id";
            var rowsAffected = await Connection.ExecuteAsync(sql, new { Id = id }, transaction: Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteByStageAndUserAsync(Guid stageId, Guid userId)
        {
            var sql = "DELETE FROM workflowmgmt.stage_assignments WHERE workflow_stage_id = @StageId AND user_id = @UserId";
            var rowsAffected = await Connection.ExecuteAsync(sql, new { StageId = stageId, UserId = userId }, transaction: Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(Guid stageId, Guid userId)
        {
            var sql = "SELECT COUNT(1) FROM workflowmgmt.stage_assignments WHERE workflow_stage_id = @StageId AND user_id = @UserId";
            var count = await Connection.QuerySingleAsync<int>(sql, new { StageId = stageId, UserId = userId }, transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> SetDefaultAssigneeAsync(Guid stageId, Guid userId, int? departmentId = null)
        {
            // First, remove default flag from all other assignments for this stage/department
            var clearDefaultSql = @"
                UPDATE workflowmgmt.stage_assignments 
                SET is_default = false 
                WHERE workflow_stage_id = @StageId";

            object clearParams = new { StageId = stageId };

            if (departmentId.HasValue)
            {
                clearDefaultSql += " AND department_id = @DepartmentId";
                clearParams = new { StageId = stageId, DepartmentId = departmentId.Value };
            }

            await Connection.ExecuteAsync(clearDefaultSql, clearParams, transaction: Transaction);

            // Set the new default
            var setDefaultSql = @"
                UPDATE workflowmgmt.stage_assignments 
                SET is_default = true 
                WHERE workflow_stage_id = @StageId AND user_id = @UserId";

            object setParams = new { StageId = stageId, UserId = userId };

            if (departmentId.HasValue)
            {
                setDefaultSql += " AND department_id = @DepartmentId";
                setParams = new { StageId = stageId, UserId = userId, DepartmentId = departmentId.Value };
            }

            var rowsAffected = await Connection.ExecuteAsync(setDefaultSql, setParams, transaction: Transaction);
            return rowsAffected > 0;
        }

        public async Task<StageAssigneeDto?> GetDefaultAssigneeAsync(Guid stageId, int? departmentId = null)
        {
            var sql = @"
                SELECT 
                    sa.id as Id,
                    sa.workflow_stage_id as WorkflowStageId,
                    sa.user_id as UserId,
                    sa.role_id as RoleId,
                    sa.department_id as DepartmentId,
                    sa.is_default as IsDefault,
                    CONCAT(u.first_name, ' ', u.last_name) as UserFullName,
                    u.email as UserEmail,
                    r.name as RoleName,
                    d.name as DepartmentName
                FROM workflowmgmt.stage_assignments sa
                INNER JOIN workflowmgmt.users u ON sa.user_id = u.id
                INNER JOIN workflowmgmt.roles r ON sa.role_id = r.id
                LEFT JOIN workflowmgmt.departments d ON sa.department_id = d.id
                WHERE sa.workflow_stage_id = @StageId 
                AND sa.is_default = true";

            object parameters = new { StageId = stageId };

            if (departmentId.HasValue)
            {
                sql += " AND sa.department_id = @DepartmentId";
                parameters = new { StageId = stageId, DepartmentId = departmentId.Value };
            }

            sql += " LIMIT 1";

            return await Connection.QuerySingleOrDefaultAsync<StageAssigneeDto>(sql, parameters, transaction: Transaction);
        }
    }
}

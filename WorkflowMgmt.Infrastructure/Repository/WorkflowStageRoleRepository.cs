using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Domain.Models.Workflow;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class WorkflowStageRoleRepository : RepositoryTranBase, IWorkflowStageRoleRepository
    {
        public WorkflowStageRoleRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<WorkflowStageRoleDto>> GetByStageIdAsync(Guid stageId)
        {
            var sql = @"
                SELECT
                    wsr.role_code as RoleCode,
                    r.name as RoleName,
                    wsr.is_required as IsRequired
                FROM workflowmgmt.workflow_stage_roles wsr
                INNER JOIN workflowmgmt.roles r ON wsr.role_code = r.code
                WHERE wsr.workflow_stage_id = @StageId
                ORDER BY r.name";

            var result = await Connection.QueryAsync<WorkflowStageRoleDto>(sql, new { StageId = stageId }, transaction: Transaction);
            return result.ToList();
        }

        public async Task<bool> CreateAsync(Guid stageId, CreateWorkflowStageRoleDto role)
        {
            var sql = @"
                INSERT INTO workflowmgmt.workflow_stage_roles 
                (workflow_stage_id, role_code, is_required, created_date, created_by)
                VALUES (@StageId, @RoleCode, @IsRequired, @CreatedDate, @CreatedBy)";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                StageId = stageId,
                role.RoleCode,
                role.IsRequired,
                CreatedDate = DateTime.Now,
                CreatedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteByStageIdAsync(Guid stageId)
        {
            var sql = "DELETE FROM workflowmgmt.workflow_stage_roles WHERE workflow_stage_id = @StageId";
            var rowsAffected = await Connection.ExecuteAsync(sql, new { StageId = stageId }, transaction: Transaction);
            return rowsAffected >= 0; // Return true even if no rows deleted (stage might not have roles)
        }

        public async Task<bool> DeleteByTemplateIdAsync(Guid templateId)
        {
            var sql = @"
                DELETE FROM workflowmgmt.workflow_stage_roles
                WHERE workflow_stage_id IN (
                    SELECT id FROM workflowmgmt.workflow_stages
                    WHERE workflow_template_id = @TemplateId
                )";
            var rowsAffected = await Connection.ExecuteAsync(sql, new { TemplateId = templateId }, transaction: Transaction);
            return rowsAffected >= 0; // Return true even if no rows deleted
        }

        public async Task<bool> UpdateStageRolesAsync(Guid stageId, List<UpdateRoleDto> roles)
        {
            // Delete existing roles
            await DeleteByStageIdAsync(stageId);

            // Insert new roles
            var createRoles = roles.Select(r => new CreateWorkflowStageRoleDto
            {
                RoleCode = r.RoleCode,
                IsRequired = r.IsRequired
            }).ToList();

            return await CreateMultipleAsync(stageId, createRoles);
        }

        public async Task<List<WorkflowStageRoleDto>> GetByRoleCodeAsync(string roleCode)
        {
            var sql = @"
                SELECT
                    wsr.role_code as RoleCode,
                    r.name as RoleName,
                    wsr.is_required as IsRequired
                FROM workflowmgmt.workflow_stage_roles wsr
                INNER JOIN workflowmgmt.roles r ON wsr.role_code = r.code
                WHERE wsr.role_code = @RoleCode
                ORDER BY r.name";

            var result = await Connection.QueryAsync<WorkflowStageRoleDto>(sql, new { RoleCode = roleCode }, transaction: Transaction);
            return result.ToList();
        }

        public async Task<bool> ExistsAsync(Guid stageId, string roleCode)
        {
            var sql = @"
                SELECT COUNT(1) 
                FROM workflowmgmt.workflow_stage_roles 
                WHERE workflow_stage_id = @StageId AND role_code = @RoleCode";

            var count = await Connection.QuerySingleAsync<int>(sql, new { StageId = stageId, RoleCode = roleCode }, transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> CreateMultipleAsync(Guid stageId, List<CreateWorkflowStageRoleDto> roles)
        {
            if (!roles.Any())
                return true;

            var sql = @"
                INSERT INTO workflowmgmt.workflow_stage_roles
                (workflow_stage_id, role_code, is_required, created_date, created_by)
                VALUES (@StageId, @RoleCode, @IsRequired, @CreatedDate, @CreatedBy)";

            var parameters = roles.Select(role => new
            {
                StageId = stageId,
                role.RoleCode,
                role.IsRequired,
                CreatedDate = DateTime.Now,
                CreatedBy = "system" // TODO: Get from current user context
            });

            var rowsAffected = await Connection.ExecuteAsync(sql, parameters, transaction: Transaction);
            return rowsAffected > 0;
        }
    }
}

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
    public class WorkflowStagePermissionRepository : RepositoryTranBase, IWorkflowStagePermissionRepository
    {
        public WorkflowStagePermissionRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<IEnumerable<WorkflowStagePermissionDto>> GetByStageIdAsync(Guid stageId)
        {
            var sql = @"
                SELECT 
                    permission_name as PermissionName,
                    is_required as IsRequired
                FROM workflowmgmt.workflow_stage_permissions
                WHERE workflow_stage_id = @StageId
                ORDER BY permission_name";

            return await Connection.QueryAsync<WorkflowStagePermissionDto>(sql, new { StageId = stageId }, transaction: Transaction);
        }

        public async Task<bool> CreateAsync(Guid stageId, CreateWorkflowStagePermissionDto permission)
        {
            var sql = @"
                INSERT INTO workflowmgmt.workflow_stage_permissions 
                (workflow_stage_id, permission_name, is_required, created_date, created_by)
                VALUES (@StageId, @PermissionName, @IsRequired, @CreatedDate, @CreatedBy)";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                StageId = stageId,
                permission.PermissionName,
                permission.IsRequired,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteByStageIdAsync(Guid stageId)
        {
            var sql = "DELETE FROM workflowmgmt.workflow_stage_permissions WHERE workflow_stage_id = @StageId";
            var rowsAffected = await Connection.ExecuteAsync(sql, new { StageId = stageId }, transaction: Transaction);
            return rowsAffected >= 0; // Return true even if no rows deleted
        }

        public async Task<bool> UpdateStagePermissionsAsync(Guid stageId, IEnumerable<UpdatePermissionDto> permissions)
        {
            // Delete existing permissions
            await DeleteByStageIdAsync(stageId);

            // Insert new permissions
            var createPermissions = permissions.Select(p => new CreateWorkflowStagePermissionDto
            {
                PermissionName = p.PermissionName,
                IsRequired = p.IsRequired
            });

            return await CreateMultipleAsync(stageId, createPermissions);
        }

        public async Task<IEnumerable<WorkflowStagePermissionDto>> GetByPermissionNameAsync(string permissionName)
        {
            var sql = @"
                SELECT 
                    permission_name as PermissionName,
                    is_required as IsRequired
                FROM workflowmgmt.workflow_stage_permissions
                WHERE permission_name = @PermissionName
                ORDER BY permission_name";

            return await Connection.QueryAsync<WorkflowStagePermissionDto>(sql, new { PermissionName = permissionName }, transaction: Transaction);
        }

        public async Task<bool> ExistsAsync(Guid stageId, string permissionName)
        {
            var sql = @"
                SELECT COUNT(1) 
                FROM workflowmgmt.workflow_stage_permissions 
                WHERE workflow_stage_id = @StageId AND permission_name = @PermissionName";

            var count = await Connection.QuerySingleAsync<int>(sql, new { StageId = stageId, PermissionName = permissionName }, transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> CreateMultipleAsync(Guid stageId, IEnumerable<CreateWorkflowStagePermissionDto> permissions)
        {
            if (!permissions.Any())
                return true;

            var sql = @"
                INSERT INTO workflowmgmt.workflow_stage_permissions 
                (workflow_stage_id, permission_name, is_required, created_date, created_by)
                VALUES (@StageId, @PermissionName, @IsRequired, @CreatedDate, @CreatedBy)";

            var parameters = permissions.Select(permission => new
            {
                StageId = stageId,
                permission.PermissionName,
                permission.IsRequired,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "system" // TODO: Get from current user context
            });

            var rowsAffected = await Connection.ExecuteAsync(sql, parameters, transaction: Transaction);
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<AvailablePermissionDto>> GetAvailablePermissionsAsync()
        {
            var sql = @"
                SELECT 
                    permission_name as PermissionName,
                    permission_name as DisplayName,
                    CASE 
                        WHEN permission_name LIKE '%syllabus%' THEN 'syllabus'
                        WHEN permission_name LIKE '%lesson%' THEN 'lesson'
                        WHEN permission_name LIKE '%session%' THEN 'session'
                        WHEN permission_name LIKE '%workflow%' THEN 'workflow'
                        ELSE 'general'
                    END as Category
                FROM workflowmgmt.get_available_permissions()
                ORDER BY Category, PermissionName";

            return await Connection.QueryAsync<AvailablePermissionDto>(sql, transaction: Transaction);
        }
    }
}

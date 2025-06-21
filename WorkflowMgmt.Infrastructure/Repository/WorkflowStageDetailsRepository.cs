using Dapper;
using Newtonsoft.Json;
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
    public class WorkflowStageDetailsRepository : RepositoryTranBase, IWorkflowStageDetailsRepository
    {
        public WorkflowStageDetailsRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<WorkflowStageDetailsDto>> GetByWorkflowTemplateIdAsync(Guid workflowTemplateId)
        {
            var sql = @"
                SELECT
                    ws.id as StageId,
                    ws.workflow_template_id as WorkflowTemplateId,
                    ws.stage_name as StageName,
                    ws.stage_order as StageOrder,
                    ws.assigned_role as AssignedRole,
                    ws.description as Description,
                    ws.is_required as IsRequired,
                    ws.auto_approve as AutoApprove,
                    ws.timeout_days as TimeoutDays,
                    ws.is_active as IsActive,
                    ws.created_date as CreatedDate,
                    ws.modified_date as ModifiedDate
                FROM workflowmgmt.workflow_stages ws
                WHERE ws.workflow_template_id = @WorkflowTemplateId
                ORDER BY ws.stage_order";

            var stages = await Connection.QueryAsync<WorkflowStageDetailsDto>(sql, new { WorkflowTemplateId = workflowTemplateId }, transaction: Transaction);
            var stageList = stages.ToList();

            // Get roles for each stage
            foreach (var stage in stageList)
            {
                stage.RequiredRoles = await GetStageRolesAsync(stage.StageId);
                stage.Actions = await GetStageActionsAsync(stage.StageId);
            }

            return stageList;
        }

        public async Task<WorkflowStageDetailsDto?> GetByStageIdAsync(Guid stageId)
        {
            var sql = @"
                SELECT
                    ws.id as StageId,
                    ws.workflow_template_id as WorkflowTemplateId,
                    ws.stage_name as StageName,
                    ws.stage_order as StageOrder,
                    ws.assigned_role as AssignedRole,
                    ws.description as Description,
                    ws.is_required as IsRequired,
                    ws.auto_approve as AutoApprove,
                    ws.timeout_days as TimeoutDays,
                    ws.is_active as IsActive,
                    ws.created_date as CreatedDate,
                    ws.modified_date as ModifiedDate
                FROM workflowmgmt.workflow_stages ws
                WHERE ws.id = @StageId";

            var stage = await Connection.QuerySingleOrDefaultAsync<WorkflowStageDetailsDto>(sql, new { StageId = stageId }, transaction: Transaction);

            if (stage == null)
                return null;

            // Get roles and actions for the stage
            stage.RequiredRoles = await GetStageRolesAsync(stage.StageId);
            stage.Actions = await GetStageActionsAsync(stage.StageId);

            return stage;
        }

        public async Task<List<RoleOptionDto>> GetActiveRolesAsync()
        {
            var sql = @"
                SELECT
                    code as Code,
                    name as Name,
                    description as Description
                FROM workflowmgmt.roles
                WHERE is_active = true
                ORDER BY name";

            var result = await Connection.QueryAsync<RoleOptionDto>(sql, transaction: Transaction);
            return result.ToList();
        }

        private async Task<List<WorkflowStageRoleDto>> GetStageRolesAsync(Guid stageId)
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

        private async Task<List<WorkflowStageActionDto>> GetStageActionsAsync(Guid stageId)
        {
            var sql = @"
                SELECT
                    id as Id,
                    workflow_stage_id as WorkflowStageId,
                    action_name as ActionName,
                    action_type as ActionType,
                    next_stage_id as NextStageId,
                    is_active as IsActive,
                    created_date as CreatedDate
                FROM workflowmgmt.workflow_stage_actions
                WHERE workflow_stage_id = @StageId AND is_active = true
                ORDER BY action_name";

            var result = await Connection.QueryAsync<WorkflowStageActionDto>(sql, new { StageId = stageId }, transaction: Transaction);
            return result.ToList();
        }
    }
}

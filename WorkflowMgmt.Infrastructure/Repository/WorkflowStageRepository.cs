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
    public class WorkflowStageRepository : RepositoryTranBase, IWorkflowStageRepository
    {
        public WorkflowStageRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<IEnumerable<WorkflowStageDto>> GetByTemplateIdAsync(Guid templateId)
        {
            var sql = @"
                SELECT 
                    ws.id as Id,
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
                    ws.modified_date as ModifiedDate,
                    ws.created_by as CreatedBy,
                    ws.modified_by as ModifiedBy,
                    
                    wsa.id as Id,
                    wsa.workflow_stage_id as WorkflowStageId,
                    wsa.action_name as ActionName,
                    wsa.action_type as ActionType,
                    wsa.next_stage_id as NextStageId,
                    wsa.is_active as IsActive,
                    wsa.created_date as CreatedDate
                FROM workflowmgmt.workflow_stages ws
                LEFT JOIN workflowmgmt.workflow_stage_actions wsa ON ws.id = wsa.workflow_stage_id AND wsa.is_active = true
                WHERE ws.workflow_template_id = @TemplateId AND ws.is_active = true
                ORDER BY ws.stage_order, wsa.action_name";

            var stageDict = new Dictionary<Guid, WorkflowStageDto>();

            await Connection.QueryAsync<WorkflowStageDto, WorkflowStageActionDto, WorkflowStageDto>(
                sql,
                (stage, action) =>
                {
                    if (!stageDict.TryGetValue(stage.Id, out var stageEntry))
                    {
                        stageEntry = stage;
                        stageEntry.Actions = new List<WorkflowStageActionDto>();
                        stageDict.Add(stage.Id, stageEntry);
                    }

                    if (action != null)
                    {
                        stageEntry.Actions.Add(action);
                    }

                    return stageEntry;
                },
                new { TemplateId = templateId },
                transaction: Transaction,
                splitOn: "Id");

            return stageDict.Values.OrderBy(s => s.StageOrder);
        }

        public async Task<WorkflowStageDto?> GetByIdAsync(Guid id)
        {
            var sql = @"
                SELECT 
                    ws.id as Id,
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
                    ws.modified_date as ModifiedDate,
                    ws.created_by as CreatedBy,
                    ws.modified_by as ModifiedBy,
                    
                    wsa.id as Id,
                    wsa.workflow_stage_id as WorkflowStageId,
                    wsa.action_name as ActionName,
                    wsa.action_type as ActionType,
                    wsa.next_stage_id as NextStageId,
                    wsa.is_active as IsActive,
                    wsa.created_date as CreatedDate
                FROM workflowmgmt.workflow_stages ws
                LEFT JOIN workflowmgmt.workflow_stage_actions wsa ON ws.id = wsa.workflow_stage_id AND wsa.is_active = true
                WHERE ws.id = @Id
                ORDER BY wsa.action_name";

            WorkflowStageDto? stage = null;

            await Connection.QueryAsync<WorkflowStageDto, WorkflowStageActionDto, WorkflowStageDto>(
                sql,
                (stageEntry, action) =>
                {
                    if (stage == null)
                    {
                        stage = stageEntry;
                        stage.Actions = new List<WorkflowStageActionDto>();
                    }

                    if (action != null)
                    {
                        stage.Actions.Add(action);
                    }

                    return stage;
                },
                new { Id = id },
                transaction: Transaction,
                splitOn: "Id");

            return stage;
        }

        public async Task<IEnumerable<WorkflowStageDto>> GetByRoleAsync(string role)
        {
            var sql = @"
                SELECT 
                    id as Id,
                    workflow_template_id as WorkflowTemplateId,
                    stage_name as StageName,
                    stage_order as StageOrder,
                    assigned_role as AssignedRole,
                    description as Description,
                    is_required as IsRequired,
                    auto_approve as AutoApprove,
                    timeout_days as TimeoutDays,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy
                FROM workflowmgmt.workflow_stages
                WHERE assigned_role = @Role AND is_active = true
                ORDER BY workflow_template_id, stage_order";

            var stages = await Connection.QueryAsync<WorkflowStageDto>(sql, new { Role = role }, transaction: Transaction);
            
            // Initialize empty actions list for each stage
            foreach (var stage in stages)
            {
                stage.Actions = new List<WorkflowStageActionDto>();
            }

            return stages;
        }

        public async Task<WorkflowStageDto?> GetFirstStageByTemplateIdAsync(Guid templateId)
        {
            var sql = @"
                SELECT 
                    id as Id,
                    workflow_template_id as WorkflowTemplateId,
                    stage_name as StageName,
                    stage_order as StageOrder,
                    assigned_role as AssignedRole,
                    description as Description,
                    is_required as IsRequired,
                    auto_approve as AutoApprove,
                    timeout_days as TimeoutDays,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy
                FROM workflowmgmt.workflow_stages
                WHERE workflow_template_id = @TemplateId AND is_active = true
                ORDER BY stage_order
                LIMIT 1";

            var stage = await Connection.QuerySingleOrDefaultAsync<WorkflowStageDto>(sql, new { TemplateId = templateId }, transaction: Transaction);
            
            if (stage != null)
            {
                stage.Actions = new List<WorkflowStageActionDto>();
            }

            return stage;
        }

        public async Task<WorkflowStageDto?> GetNextStageAsync(Guid currentStageId, string actionType)
        {
            var sql = @"
                SELECT 
                    ws.id as Id,
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
                    ws.modified_date as ModifiedDate,
                    ws.created_by as CreatedBy,
                    ws.modified_by as ModifiedBy
                FROM workflowmgmt.workflow_stages ws
                INNER JOIN workflowmgmt.workflow_stage_actions wsa ON ws.id = wsa.next_stage_id
                WHERE wsa.workflow_stage_id = @CurrentStageId 
                AND wsa.action_type = @ActionType 
                AND wsa.is_active = true 
                AND ws.is_active = true";

            var stage = await Connection.QuerySingleOrDefaultAsync<WorkflowStageDto>(sql, 
                new { CurrentStageId = currentStageId, ActionType = actionType }, 
                transaction: Transaction);
            
            if (stage != null)
            {
                stage.Actions = new List<WorkflowStageActionDto>();
            }

            return stage;
        }

        public async Task<WorkflowStageDto?> GetNextStageAsync(Guid templateId, int currentStageOrder)
        {
            var sql = @"
                SELECT
                    ws.id as Id,
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
                    ws.modified_date as ModifiedDate,
                    ws.created_by as CreatedBy,
                    ws.modified_by as ModifiedBy
                FROM workflowmgmt.workflow_stages ws
                WHERE ws.workflow_template_id = @TemplateId
                AND ws.stage_order > @CurrentStageOrder
                AND ws.is_active = true
                ORDER BY ws.stage_order
                LIMIT 1";

            var stage = await Connection.QuerySingleOrDefaultAsync<WorkflowStageDto>(sql,
                new { TemplateId = templateId, CurrentStageOrder = currentStageOrder },
                transaction: Transaction);

            if (stage != null)
            {
                stage.Actions = new List<WorkflowStageActionDto>();
            }

            return stage;
        }

        public async Task<Guid> CreateAsync(Guid templateId, CreateWorkflowStageDto stage)
        {
            var id = Guid.NewGuid();
            var sql = @"
                INSERT INTO workflowmgmt.workflow_stages 
                (id, workflow_template_id, stage_name, stage_order, assigned_role, description, 
                 is_required, auto_approve, timeout_days, is_active, created_date, created_by)
                VALUES (@Id, @TemplateId, @StageName, @StageOrder, @AssignedRole, @Description, 
                        @IsRequired, @AutoApprove, @TimeoutDays, true, @CreatedDate, @CreatedBy)";

            await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                TemplateId = templateId,
                stage.StageName,
                stage.StageOrder,
                stage.AssignedRole,
                stage.Description,
                stage.IsRequired,
                stage.AutoApprove,
                stage.TimeoutDays,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return id;
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateWorkflowStageDto stage)
        {
            var sql = @"
                UPDATE workflowmgmt.workflow_stages 
                SET stage_name = @StageName,
                    stage_order = @StageOrder,
                    assigned_role = @AssignedRole,
                    description = @Description,
                    is_required = @IsRequired,
                    auto_approve = @AutoApprove,
                    timeout_days = @TimeoutDays,
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                stage.StageName,
                stage.StageOrder,
                stage.AssignedRole,
                stage.Description,
                stage.IsRequired,
                stage.AutoApprove,
                stage.TimeoutDays,
                ModifiedDate = DateTime.UtcNow,
                ModifiedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var sql = "DELETE FROM workflowmgmt.workflow_stages WHERE id = @Id";
            var rowsAffected = await Connection.ExecuteAsync(sql, new { Id = id }, transaction: Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteByTemplateIdAsync(Guid templateId)
        {
            var sql = "DELETE FROM workflowmgmt.workflow_stages WHERE workflow_template_id = @TemplateId";
            var rowsAffected = await Connection.ExecuteAsync(sql, new { TemplateId = templateId }, transaction: Transaction);
            return rowsAffected >= 0; // Return true even if no rows deleted (template had no stages)
        }

        public async Task<bool> DeactivateStagesByTemplateIdAsync(Guid templateId)
        {
            var sql = @"
                UPDATE workflowmgmt.workflow_stages
                SET is_active = false,
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE workflow_template_id = @TemplateId";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                TemplateId = templateId,
                ModifiedDate = DateTime.UtcNow,
                ModifiedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return rowsAffected >= 0; // Return true even if no rows affected (no stages to deactivate)
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var sql = "SELECT COUNT(1) FROM workflowmgmt.workflow_stages WHERE id = @Id";
            var count = await Connection.QuerySingleAsync<int>(sql, new { Id = id }, transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> ExistsInTemplateAsync(Guid templateId, int stageOrder, Guid? excludeId = null)
        {
            var sql = @"
                SELECT COUNT(1) 
                FROM workflowmgmt.workflow_stages 
                WHERE workflow_template_id = @TemplateId 
                AND stage_order = @StageOrder";

            object parameters = new { TemplateId = templateId, StageOrder = stageOrder };

            if (excludeId.HasValue)
            {
                sql += " AND id != @ExcludeId";
                parameters = new { TemplateId = templateId, StageOrder = stageOrder, ExcludeId = excludeId.Value };
            }

            var count = await Connection.QuerySingleAsync<int>(sql, parameters, transaction: Transaction);
            return count > 0;
        }

        public async Task<int> GetMaxStageOrderAsync(Guid templateId)
        {
            var sql = @"
                SELECT COALESCE(MAX(stage_order), 0) 
                FROM workflowmgmt.workflow_stages 
                WHERE workflow_template_id = @TemplateId";

            return await Connection.QuerySingleAsync<int>(sql, new { TemplateId = templateId }, transaction: Transaction);
        }

        public async Task<bool> ReorderStagesAsync(Guid templateId, List<(Guid stageId, int newOrder)> stageOrders)
        {
            var sql = @"
                UPDATE workflowmgmt.workflow_stages 
                SET stage_order = @NewOrder,
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE id = @StageId AND workflow_template_id = @TemplateId";

            var parameters = stageOrders.Select(so => new
            {
                StageId = so.stageId,
                NewOrder = so.newOrder,
                TemplateId = templateId,
                ModifiedDate = DateTime.UtcNow,
                ModifiedBy = "system" // TODO: Get from current user context
            });

            var rowsAffected = await Connection.ExecuteAsync(sql, parameters, transaction: Transaction);
            return rowsAffected == stageOrders.Count;
        }
    }
}

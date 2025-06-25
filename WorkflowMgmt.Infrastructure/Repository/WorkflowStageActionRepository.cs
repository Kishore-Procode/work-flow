using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Domain.Models.Workflow;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class WorkflowStageActionRepository : RepositoryTranBase, IWorkflowStageActionRepository
    {
        public WorkflowStageActionRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<IEnumerable<WorkflowStageActionDto>> GetByStageIdAsync(Guid stageId)
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

            return await Connection.QueryAsync<WorkflowStageActionDto>(sql, new { StageId = stageId }, transaction: Transaction);
        }

        public async Task<IEnumerable<WorkflowStageActionDto>> GetActiveByStageIdAsync(Guid stageId)
        {
            // This is the same as GetByStageIdAsync since it already filters by is_active = true
            return await GetByStageIdAsync(stageId);
        }

        public async Task<WorkflowStageActionDto?> GetByIdAsync(Guid id)
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
                WHERE id = @Id";

            return await Connection.QuerySingleOrDefaultAsync<WorkflowStageActionDto>(sql, new { Id = id }, transaction: Transaction);
        }

        public async Task<WorkflowStageActionDto?> GetByStageAndActionTypeAsync(Guid stageId, string actionType)
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
                WHERE workflow_stage_id = @StageId 
                AND action_type = @ActionType 
                AND is_active = true";

            return await Connection.QuerySingleOrDefaultAsync<WorkflowStageActionDto>(sql, 
                new { StageId = stageId, ActionType = actionType }, 
                transaction: Transaction);
        }

        public async Task<Guid> CreateAsync(Guid stageId, CreateWorkflowStageActionDto action)
        {
            var id = Guid.NewGuid();
            var sql = @"
                INSERT INTO workflowmgmt.workflow_stage_actions 
                (id, workflow_stage_id, action_name, action_type, next_stage_id, is_active, created_date)
                VALUES (@Id, @StageId, @ActionName, @ActionType, @NextStageId, true, @CreatedDate)";

            await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                StageId = stageId,
                action.ActionName,
                action.ActionType,
                action.NextStageId,
                CreatedDate = DateTime.UtcNow
            }, transaction: Transaction);

            return id;
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateWorkflowStageActionDto action)
        {
            var sql = @"
                UPDATE workflowmgmt.workflow_stage_actions 
                SET action_name = @ActionName,
                    action_type = @ActionType,
                    next_stage_id = @NextStageId
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                action.ActionName,
                action.ActionType,
                action.NextStageId
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var sql = "DELETE FROM workflowmgmt.workflow_stage_actions WHERE id = @Id";
            var rowsAffected = await Connection.ExecuteAsync(sql, new { Id = id }, transaction: Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var sql = "SELECT COUNT(1) FROM workflowmgmt.workflow_stage_actions WHERE id = @Id";
            var count = await Connection.QuerySingleAsync<int>(sql, new { Id = id }, transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> ExistsByStageAndActionTypeAsync(Guid stageId, string actionType, Guid? excludeId = null)
        {
            var sql = @"
                SELECT COUNT(1) 
                FROM workflowmgmt.workflow_stage_actions 
                WHERE workflow_stage_id = @StageId 
                AND action_type = @ActionType";

            object parameters = new { StageId = stageId, ActionType = actionType };

            if (excludeId.HasValue)
            {
                sql += " AND id != @ExcludeId";
                parameters = new { StageId = stageId, ActionType = actionType, ExcludeId = excludeId.Value };
            }

            var count = await Connection.QuerySingleAsync<int>(sql, parameters, transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> DeleteByStageIdAsync(Guid stageId)
        {
            var sql = "DELETE FROM workflowmgmt.workflow_stage_actions WHERE workflow_stage_id = @StageId";
            var rowsAffected = await Connection.ExecuteAsync(sql, new { StageId = stageId }, transaction: Transaction);
            return rowsAffected >= 0; // Return true even if no rows were deleted (stage had no actions)
        }
    }
}

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
    public class WorkflowStageHistoryRepository : RepositoryTranBase, IWorkflowStageHistoryRepository
    {
        public WorkflowStageHistoryRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<IEnumerable<WorkflowStageHistoryWithDetailsDto>> GetByDocumentWorkflowIdAsync(Guid documentWorkflowId)
        {
            var sql = @"
                SELECT 
                    wsh.id as Id,
                    wsh.document_workflow_id as DocumentWorkflowId,
                    wsh.stage_id as StageId,
                    wsh.action_taken as ActionTaken,
                    wsh.processed_by as ProcessedBy,
                    wsh.processed_date as ProcessedDate,
                    wsh.comments as Comments,
                    wsh.attachments as Attachments,
                    wsh.created_date as CreatedDate,
                    ws.stage_name as StageName,
                    CONCAT(u.first_name, ' ', u.last_name) as ProcessedByName,
                    u.email as ProcessedByEmail,
                    r.name as ProcessedByRole,
                    dw.document_id as DocumentTitle,
                    dw.document_type as DocumentType
                FROM workflowmgmt.workflow_stage_history wsh
                INNER JOIN workflowmgmt.workflow_stages ws ON wsh.stage_id = ws.id
                INNER JOIN workflowmgmt.users u ON wsh.processed_by = u.id
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                INNER JOIN workflowmgmt.document_workflows dw ON wsh.document_workflow_id = dw.id
                WHERE wsh.document_workflow_id = @DocumentWorkflowId
                ORDER BY wsh.processed_date DESC";

            return await Connection.QueryAsync<WorkflowStageHistoryWithDetailsDto>(sql, 
                new { DocumentWorkflowId = documentWorkflowId }, 
                transaction: Transaction);
        }

        public async Task<IEnumerable<WorkflowStageHistoryWithDetailsDto>> GetByStageIdAsync(Guid stageId)
        {
            var sql = @"
                SELECT 
                    wsh.id as Id,
                    wsh.document_workflow_id as DocumentWorkflowId,
                    wsh.stage_id as StageId,
                    wsh.action_taken as ActionTaken,
                    wsh.processed_by as ProcessedBy,
                    wsh.processed_date as ProcessedDate,
                    wsh.comments as Comments,
                    wsh.attachments as Attachments,
                    wsh.created_date as CreatedDate,
                    ws.stage_name as StageName,
                    CONCAT(u.first_name, ' ', u.last_name) as ProcessedByName,
                    u.email as ProcessedByEmail,
                    r.name as ProcessedByRole,
                    dw.document_id as DocumentTitle,
                    dw.document_type as DocumentType
                FROM workflowmgmt.workflow_stage_history wsh
                INNER JOIN workflowmgmt.workflow_stages ws ON wsh.stage_id = ws.id
                INNER JOIN workflowmgmt.users u ON wsh.processed_by = u.id
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                INNER JOIN workflowmgmt.document_workflows dw ON wsh.document_workflow_id = dw.id
                WHERE wsh.stage_id = @StageId
                ORDER BY wsh.processed_date DESC";

            return await Connection.QueryAsync<WorkflowStageHistoryWithDetailsDto>(sql, 
                new { StageId = stageId }, 
                transaction: Transaction);
        }

        public async Task<IEnumerable<WorkflowStageHistoryWithDetailsDto>> GetByProcessedByAsync(Guid processedBy)
        {
            var sql = @"
                SELECT 
                    wsh.id as Id,
                    wsh.document_workflow_id as DocumentWorkflowId,
                    wsh.stage_id as StageId,
                    wsh.action_taken as ActionTaken,
                    wsh.processed_by as ProcessedBy,
                    wsh.processed_date as ProcessedDate,
                    wsh.comments as Comments,
                    wsh.attachments as Attachments,
                    wsh.created_date as CreatedDate,
                    ws.stage_name as StageName,
                    CONCAT(u.first_name, ' ', u.last_name) as ProcessedByName,
                    u.email as ProcessedByEmail,
                    r.name as ProcessedByRole,
                    dw.document_id as DocumentTitle,
                    dw.document_type as DocumentType
                FROM workflowmgmt.workflow_stage_history wsh
                INNER JOIN workflowmgmt.workflow_stages ws ON wsh.stage_id = ws.id
                INNER JOIN workflowmgmt.users u ON wsh.processed_by = u.id
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                INNER JOIN workflowmgmt.document_workflows dw ON wsh.document_workflow_id = dw.id
                WHERE wsh.processed_by = @ProcessedBy
                ORDER BY wsh.processed_date DESC";

            return await Connection.QueryAsync<WorkflowStageHistoryWithDetailsDto>(sql, 
                new { ProcessedBy = processedBy }, 
                transaction: Transaction);
        }

        public async Task<IEnumerable<WorkflowStageHistoryWithDetailsDto>> GetByActionTypeAsync(string actionType)
        {
            var sql = @"
                SELECT 
                    wsh.id as Id,
                    wsh.document_workflow_id as DocumentWorkflowId,
                    wsh.stage_id as StageId,
                    wsh.action_taken as ActionTaken,
                    wsh.processed_by as ProcessedBy,
                    wsh.processed_date as ProcessedDate,
                    wsh.comments as Comments,
                    wsh.attachments as Attachments,
                    wsh.created_date as CreatedDate,
                    ws.stage_name as StageName,
                    CONCAT(u.first_name, ' ', u.last_name) as ProcessedByName,
                    u.email as ProcessedByEmail,
                    r.name as ProcessedByRole,
                    dw.document_id as DocumentTitle,
                    dw.document_type as DocumentType
                FROM workflowmgmt.workflow_stage_history wsh
                INNER JOIN workflowmgmt.workflow_stages ws ON wsh.stage_id = ws.id
                INNER JOIN workflowmgmt.users u ON wsh.processed_by = u.id
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                INNER JOIN workflowmgmt.document_workflows dw ON wsh.document_workflow_id = dw.id
                WHERE wsh.action_taken = @ActionType
                ORDER BY wsh.processed_date DESC";

            return await Connection.QueryAsync<WorkflowStageHistoryWithDetailsDto>(sql, 
                new { ActionType = actionType }, 
                transaction: Transaction);
        }

        public async Task<WorkflowStageHistoryWithDetailsDto?> GetByIdAsync(Guid id)
        {
            var sql = @"
                SELECT 
                    wsh.id as Id,
                    wsh.document_workflow_id as DocumentWorkflowId,
                    wsh.stage_id as StageId,
                    wsh.action_taken as ActionTaken,
                    wsh.processed_by as ProcessedBy,
                    wsh.processed_date as ProcessedDate,
                    wsh.comments as Comments,
                    wsh.attachments as Attachments,
                    wsh.created_date as CreatedDate,
                    ws.stage_name as StageName,
                    CONCAT(u.first_name, ' ', u.last_name) as ProcessedByName,
                    u.email as ProcessedByEmail,
                    r.name as ProcessedByRole,
                    dw.document_id as DocumentTitle,
                    dw.document_type as DocumentType
                FROM workflowmgmt.workflow_stage_history wsh
                INNER JOIN workflowmgmt.workflow_stages ws ON wsh.stage_id = ws.id
                INNER JOIN workflowmgmt.users u ON wsh.processed_by = u.id
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                INNER JOIN workflowmgmt.document_workflows dw ON wsh.document_workflow_id = dw.id
                WHERE wsh.id = @Id";

            return await Connection.QuerySingleOrDefaultAsync<WorkflowStageHistoryWithDetailsDto>(sql, 
                new { Id = id }, 
                transaction: Transaction);
        }

        public async Task<Guid> CreateAsync(CreateWorkflowStageHistoryDto history)
        {
            var id = Guid.NewGuid();
            var sql = @"
                INSERT INTO workflowmgmt.workflow_stage_history
                (id, document_workflow_id, stage_id, action_taken, processed_by, assigned_to, processed_date,
                 comments, attachments, created_date)
                VALUES (@Id, @DocumentWorkflowId, @StageId, @ActionTaken, @ProcessedBy, @AssignedTo, @ProcessedDate,
                        @Comments, @Attachments, @CreatedDate)";

            await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                history.DocumentWorkflowId,
                history.StageId,
                history.ActionTaken,
                history.ProcessedBy,
                history.AssignedTo,
                ProcessedDate = DateTime.UtcNow,
                history.Comments,
                history.Attachments,
                CreatedDate = DateTime.UtcNow
            }, transaction: Transaction);

            return id;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var sql = "SELECT COUNT(1) FROM workflowmgmt.workflow_stage_history WHERE id = @Id";
            var count = await Connection.QuerySingleAsync<int>(sql, new { Id = id }, transaction: Transaction);
            return count > 0;
        }

        public async Task<WorkflowStageHistoryWithDetailsDto?> GetLatestByDocumentWorkflowAsync(Guid documentWorkflowId)
        {
            var sql = @"
                SELECT 
                    wsh.id as Id,
                    wsh.document_workflow_id as DocumentWorkflowId,
                    wsh.stage_id as StageId,
                    wsh.action_taken as ActionTaken,
                    wsh.processed_by as ProcessedBy,
                    wsh.processed_date as ProcessedDate,
                    wsh.comments as Comments,
                    wsh.attachments as Attachments,
                    wsh.created_date as CreatedDate,
                    ws.stage_name as StageName,
                    CONCAT(u.first_name, ' ', u.last_name) as ProcessedByName,
                    u.email as ProcessedByEmail,
                    r.name as ProcessedByRole,
                    dw.document_id as DocumentTitle,
                    dw.document_type as DocumentType
                FROM workflowmgmt.workflow_stage_history wsh
                INNER JOIN workflowmgmt.workflow_stages ws ON wsh.stage_id = ws.id
                INNER JOIN workflowmgmt.users u ON wsh.processed_by = u.id
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                INNER JOIN workflowmgmt.document_workflows dw ON wsh.document_workflow_id = dw.id
                WHERE wsh.document_workflow_id = @DocumentWorkflowId
                ORDER BY wsh.processed_date DESC
                LIMIT 1";

            return await Connection.QuerySingleOrDefaultAsync<WorkflowStageHistoryWithDetailsDto>(sql, 
                new { DocumentWorkflowId = documentWorkflowId }, 
                transaction: Transaction);
        }
    }
}

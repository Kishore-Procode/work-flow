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
    public class DocumentWorkflowRepository : RepositoryTranBase, IDocumentWorkflowRepository
    {
        public DocumentWorkflowRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<IEnumerable<DocumentWorkflowWithDetailsDto>> GetAllAsync()
        {
            var sql = @"
                SELECT 
                    dw.id as Id,
                    dw.document_id as DocumentId,
                    dw.document_type as DocumentType,
                    dw.workflow_template_id as WorkflowTemplateId,
                    dw.current_stage_id as CurrentStageId,
                    dw.status as Status,
                    dw.initiated_by as InitiatedBy,
                    dw.initiated_date as InitiatedDate,
                    dw.completed_date as CompletedDate,
                    dw.is_active as IsActive,
                    dw.created_date as CreatedDate,
                    dw.modified_date as ModifiedDate,
                    wt.name as WorkflowTemplateName,
                    ws.stage_name as CurrentStageName,
                    ws.assigned_role as CurrentStageAssignedRole,
                    ws.stage_order as CurrentStageOrder
                FROM workflowmgmt.document_workflows dw
                INNER JOIN workflowmgmt.workflow_templates wt ON dw.workflow_template_id = wt.id
                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                WHERE dw.is_active = true
                ORDER BY dw.initiated_date DESC";

            return await Connection.QueryAsync<DocumentWorkflowWithDetailsDto>(sql, transaction: Transaction);
        }

        public async Task<IEnumerable<DocumentWorkflowWithDetailsDto>> GetByDocumentTypeAsync(string documentType)
        {
            var sql = @"
                SELECT 
                    dw.id as Id,
                    dw.document_id as DocumentId,
                    dw.document_type as DocumentType,
                    dw.workflow_template_id as WorkflowTemplateId,
                    dw.current_stage_id as CurrentStageId,
                    dw.status as Status,
                    dw.initiated_by as InitiatedBy,
                    dw.initiated_date as InitiatedDate,
                    dw.completed_date as CompletedDate,
                    dw.is_active as IsActive,
                    dw.created_date as CreatedDate,
                    dw.modified_date as ModifiedDate,
                    wt.name as WorkflowTemplateName,
                    ws.stage_name as CurrentStageName,
                    ws.assigned_role as CurrentStageAssignedRole,
                    ws.stage_order as CurrentStageOrder
                FROM workflowmgmt.document_workflows dw
                INNER JOIN workflowmgmt.workflow_templates wt ON dw.workflow_template_id = wt.id
                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                WHERE dw.document_type = @DocumentType AND dw.is_active = true
                ORDER BY dw.initiated_date DESC";

            return await Connection.QueryAsync<DocumentWorkflowWithDetailsDto>(sql, 
                new { DocumentType = documentType }, 
                transaction: Transaction);
        }

        public async Task<IEnumerable<DocumentWorkflowWithDetailsDto>> GetByStatusAsync(string status)
        {
            var sql = @"
                SELECT 
                    dw.id as Id,
                    dw.document_id as DocumentId,
                    dw.document_type as DocumentType,
                    dw.workflow_template_id as WorkflowTemplateId,
                    dw.current_stage_id as CurrentStageId,
                    dw.status as Status,
                    dw.initiated_by as InitiatedBy,
                    dw.initiated_date as InitiatedDate,
                    dw.completed_date as CompletedDate,
                    dw.is_active as IsActive,
                    dw.created_date as CreatedDate,
                    dw.modified_date as ModifiedDate,
                    wt.name as WorkflowTemplateName,
                    ws.stage_name as CurrentStageName,
                    ws.assigned_role as CurrentStageAssignedRole,
                    ws.stage_order as CurrentStageOrder
                FROM workflowmgmt.document_workflows dw
                INNER JOIN workflowmgmt.workflow_templates wt ON dw.workflow_template_id = wt.id
                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                WHERE dw.status = @Status AND dw.is_active = true
                ORDER BY dw.initiated_date DESC";

            return await Connection.QueryAsync<DocumentWorkflowWithDetailsDto>(sql, 
                new { Status = status }, 
                transaction: Transaction);
        }

        public async Task<IEnumerable<DocumentWorkflowWithDetailsDto>> GetByInitiatedByAsync(Guid userId)
        {
            var sql = @"
                SELECT 
                    dw.id as Id,
                    dw.document_id as DocumentId,
                    dw.document_type as DocumentType,
                    dw.workflow_template_id as WorkflowTemplateId,
                    dw.current_stage_id as CurrentStageId,
                    dw.status as Status,
                    dw.initiated_by as InitiatedBy,
                    dw.initiated_date as InitiatedDate,
                    dw.completed_date as CompletedDate,
                    dw.is_active as IsActive,
                    dw.created_date as CreatedDate,
                    dw.modified_date as ModifiedDate,
                    wt.name as WorkflowTemplateName,
                    ws.stage_name as CurrentStageName,
                    ws.assigned_role as CurrentStageAssignedRole,
                    ws.stage_order as CurrentStageOrder
                FROM workflowmgmt.document_workflows dw
                INNER JOIN workflowmgmt.workflow_templates wt ON dw.workflow_template_id = wt.id
                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                WHERE dw.initiated_by = @UserId AND dw.is_active = true
                ORDER BY dw.initiated_date DESC";

            return await Connection.QueryAsync<DocumentWorkflowWithDetailsDto>(sql, 
                new { UserId = userId }, 
                transaction: Transaction);
        }

        public async Task<IEnumerable<DocumentWorkflowWithDetailsDto>> GetByRoleAsync(string role)
        {
            var sql = @"
                SELECT 
                    dw.id as Id,
                    dw.document_id as DocumentId,
                    dw.document_type as DocumentType,
                    dw.workflow_template_id as WorkflowTemplateId,
                    dw.current_stage_id as CurrentStageId,
                    dw.status as Status,
                    dw.initiated_by as InitiatedBy,
                    dw.initiated_date as InitiatedDate,
                    dw.completed_date as CompletedDate,
                    dw.is_active as IsActive,
                    dw.created_date as CreatedDate,
                    dw.modified_date as ModifiedDate,
                    wt.name as WorkflowTemplateName,
                    ws.stage_name as CurrentStageName,
                    ws.assigned_role as CurrentStageAssignedRole,
                    ws.stage_order as CurrentStageOrder
                FROM workflowmgmt.document_workflows dw
                INNER JOIN workflowmgmt.workflow_templates wt ON dw.workflow_template_id = wt.id
                INNER JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                WHERE ws.assigned_role = @Role 
                AND dw.status = 'In Progress' 
                AND dw.is_active = true
                ORDER BY dw.initiated_date DESC";

            return await Connection.QueryAsync<DocumentWorkflowWithDetailsDto>(sql, 
                new { Role = role }, 
                transaction: Transaction);
        }

        public async Task<DocumentWorkflowWithDetailsDto?> GetByIdAsync(Guid id)
        {
            var sql = @"
                SELECT 
                    dw.id as Id,
                    dw.document_id as DocumentId,
                    dw.document_type as DocumentType,
                    dw.workflow_template_id as WorkflowTemplateId,
                    dw.current_stage_id as CurrentStageId,
                    dw.status as Status,
                    dw.initiated_by as InitiatedBy,
                    dw.initiated_date as InitiatedDate,
                    dw.completed_date as CompletedDate,
                    dw.is_active as IsActive,
                    dw.created_date as CreatedDate,
                    dw.modified_date as ModifiedDate,
                    wt.name as WorkflowTemplateName,
                    ws.stage_name as CurrentStageName,
                    ws.assigned_role as CurrentStageAssignedRole,
                    ws.stage_order as CurrentStageOrder
                FROM workflowmgmt.document_workflows dw
                INNER JOIN workflowmgmt.workflow_templates wt ON dw.workflow_template_id = wt.id
                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                WHERE dw.id = @Id";

            return await Connection.QuerySingleOrDefaultAsync<DocumentWorkflowWithDetailsDto>(sql, 
                new { Id = id }, 
                transaction: Transaction);
        }

        public async Task<DocumentWorkflowWithDetailsDto?> GetByDocumentIdAsync(string documentId)
        {
            var sql = @"
                SELECT 
                    dw.id as Id,
                    dw.document_id as DocumentId,
                    dw.document_type as DocumentType,
                    dw.workflow_template_id as WorkflowTemplateId,
                    dw.current_stage_id as CurrentStageId,
                    dw.status as Status,
                    dw.initiated_by as InitiatedBy,
                    dw.assigned_to as AssignedTo,
                    dw.initiated_date as InitiatedDate,
                    dw.completed_date as CompletedDate,
                    dw.is_active as IsActive,
                    dw.created_date as CreatedDate,
                    dw.modified_date as ModifiedDate,
                    wt.name as WorkflowTemplateName,
                    ws.stage_name as CurrentStageName,
                    ws.assigned_role as CurrentStageAssignedRole,
                    ws.stage_order as CurrentStageOrder
                FROM workflowmgmt.document_workflows dw
                INNER JOIN workflowmgmt.workflow_templates wt ON dw.workflow_template_id = wt.id
                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                WHERE dw.document_id = @DocumentId::uuid AND dw.is_active = true
                ORDER BY dw.initiated_date DESC
                LIMIT 1";

            return await Connection.QuerySingleOrDefaultAsync<DocumentWorkflowWithDetailsDto>(sql, 
                new { DocumentId = documentId }, 
                transaction: Transaction);
        }

        public async Task<Guid> CreateAsync(CreateDocumentWorkflowDto workflow)
        {
            var id = Guid.NewGuid();
            
            // Get the first stage of the workflow template
            var firstStageSql = @"
                SELECT id 
                FROM workflowmgmt.workflow_stages 
                WHERE workflow_template_id = @TemplateId 
                AND is_active = true 
                ORDER BY stage_order 
                LIMIT 1";

            var firstStageId = await Connection.QuerySingleOrDefaultAsync<Guid?>(firstStageSql, 
                new { TemplateId = workflow.WorkflowTemplateId }, 
                transaction: Transaction);

            var sql = @"
                INSERT INTO workflowmgmt.document_workflows
                (id, document_id, document_type, workflow_template_id, current_stage_id,
                 status, initiated_by, assigned_to, initiated_date, is_active, created_date)
                VALUES (@Id, @DocumentId, @DocumentType, @WorkflowTemplateId, @CurrentStageId,
                        'In Progress', @InitiatedBy, @AssignedTo, @InitiatedDate, true, @CreatedDate)";

            await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                DocumentId = Guid.Parse(workflow.DocumentId),
                workflow.DocumentType,
                workflow.WorkflowTemplateId,
                CurrentStageId = firstStageId,
                workflow.InitiatedBy,
                AssignedTo = workflow.AssignedTo,
                InitiatedDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            }, transaction: Transaction);

            return id;
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateDocumentWorkflowDto workflow)
        {
            var sql = @"
                UPDATE workflowmgmt.document_workflows 
                SET current_stage_id = COALESCE(@CurrentStageId, current_stage_id),
                    status = COALESCE(@Status, status),
                    completed_date = @CompletedDate,
                    modified_date = @ModifiedDate
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                workflow.CurrentStageId,
                workflow.Status,
                workflow.CompletedDate,
                ModifiedDate = DateTime.UtcNow
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> AdvanceStageAsync(Guid id, Guid? nextStageId, string? comments = null)
        {
            var sql = @"
                UPDATE workflowmgmt.document_workflows 
                SET current_stage_id = @NextStageId,
                    status = CASE 
                        WHEN @NextStageId IS NULL THEN 'Completed'
                        ELSE 'In Progress'
                    END,
                    completed_date = CASE 
                        WHEN @NextStageId IS NULL THEN @CompletedDate
                        ELSE completed_date
                    END,
                    modified_date = @ModifiedDate
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                NextStageId = nextStageId,
                CompletedDate = nextStageId == null ? DateTime.UtcNow : (DateTime?)null,
                ModifiedDate = DateTime.UtcNow
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> CompleteWorkflowAsync(Guid id)
        {
            var sql = @"
                UPDATE workflowmgmt.document_workflows 
                SET current_stage_id = NULL,
                    status = 'Completed',
                    completed_date = @CompletedDate,
                    modified_date = @ModifiedDate
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                CompletedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> CancelWorkflowAsync(Guid id, string? reason = null)
        {
            var sql = @"
                UPDATE workflowmgmt.document_workflows 
                SET status = 'Cancelled',
                    completed_date = @CompletedDate,
                    modified_date = @ModifiedDate
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                CompletedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var sql = "SELECT COUNT(1) FROM workflowmgmt.document_workflows WHERE id = @Id";
            var count = await Connection.QuerySingleAsync<int>(sql, new { Id = id }, transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> ExistsByDocumentIdAsync(string documentId)
        {
            var sql = @"
                SELECT COUNT(1) 
                FROM workflowmgmt.document_workflows 
                WHERE document_id = @DocumentId::uuid AND is_active = true";
            
            var count = await Connection.QuerySingleAsync<int>(sql, new { DocumentId = documentId }, transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> IsWorkflowActiveAsync(Guid id)
        {
            var sql = @"
                SELECT COUNT(1) 
                FROM workflowmgmt.document_workflows 
                WHERE id = @Id AND is_active = true AND status = 'In Progress'";
            
            var count = await Connection.QuerySingleAsync<int>(sql, new { Id = id }, transaction: Transaction);
            return count > 0;
        }

        public async Task<WorkflowStatsDto> GetWorkflowStatsAsync()
        {
            var sql = @"
                SELECT 
                    (SELECT COUNT(*) FROM workflowmgmt.workflow_templates) as TotalTemplates,
                    (SELECT COUNT(*) FROM workflowmgmt.workflow_templates WHERE is_active = true) as ActiveTemplates,
                    (SELECT COUNT(*) FROM workflowmgmt.document_workflows) as TotalWorkflows,
                    (SELECT COUNT(*) FROM workflowmgmt.document_workflows WHERE is_active = true) as ActiveWorkflows,
                    (SELECT COUNT(*) FROM workflowmgmt.document_workflows WHERE status = 'Completed') as CompletedWorkflows,
                    (SELECT COUNT(*) FROM workflowmgmt.document_workflows WHERE status = 'In Progress' AND is_active = true) as PendingWorkflows";

            return await Connection.QuerySingleAsync<WorkflowStatsDto>(sql, transaction: Transaction);
        }
    }
}

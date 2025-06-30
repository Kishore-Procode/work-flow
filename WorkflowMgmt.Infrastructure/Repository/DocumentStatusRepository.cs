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
    public class DocumentStatusRepository : RepositoryTranBase, IDocumentStatusRepository
    {
        public DocumentStatusRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<IEnumerable<DocumentStatusDto>> GetUserDocumentStatusAsync(Guid userId, string? documentType = null)
        {
            // Simple approach - get all document types in one query
            var query = @"
                        SELECT DISTINCT
                            CASE
                                WHEN dw.document_type = 'syllabus' THEN s.id
                                WHEN dw.document_type = 'lesson' THEN lp.id
                                WHEN dw.document_type = 'session' THEN sess.id
                            END as DocumentId,
                            dw.document_type as DocumentType,
                            CASE
                                WHEN dw.document_type = 'syllabus' THEN s.title
                                WHEN dw.document_type = 'lesson' THEN lp.title
                                WHEN dw.document_type = 'session' THEN sess.title
                            END as Title,
                            CASE
                                WHEN dw.document_type = 'syllabus' THEN s.status
                                WHEN dw.document_type = 'lesson' THEN lp.status
                                WHEN dw.document_type = 'session' THEN sess.status
                            END as Status,
                            d.name as DepartmentName,
                            CASE
                                WHEN dw.document_type = 'syllabus' THEN 
                                (select username from workflowmgmt.users where id = s.faculty_id)
                                WHEN dw.document_type = 'lesson' THEN         
                                (select username from workflowmgmt.users where id = lp.faculty_id)
                                WHEN dw.document_type = 'session' THEN         
                                (select username from workflowmgmt.users where id = sess.faculty_id)
                            END as FacultyName,
                            CASE
                                WHEN dw.document_type = 'syllabus' THEN s.created_date
                                WHEN dw.document_type = 'lesson' THEN lp.created_date
                                WHEN dw.document_type = 'session' THEN sess.created_date
                            END as CreatedDate,
                            CASE
                                WHEN dw.document_type = 'syllabus' THEN s.modified_date
                                WHEN dw.document_type = 'lesson' THEN lp.modified_date
                                WHEN dw.document_type = 'session' THEN sess.modified_date
                            END as ModifiedDate,
                            dw.id as WorkflowId,
                            wt.name as WorkflowTemplateName,
                            wt.id as WorkflowTemplateId,
                            dw.current_stage_id as CurrentStageId,
                            ws.stage_name as CurrentStageName,
                            ws.stage_order as CurrentStageOrder,
                            CASE WHEN EXISTS (
                                SELECT 1 FROM workflowmgmt.workflow_stage_history wsh2
                                WHERE wsh2.document_workflow_id = dw.id
                                AND wsh2.assigned_to = @UserId
                            ) THEN true ELSE false END as IsAssignedTo,
                            CASE WHEN EXISTS (
                                SELECT 1 FROM workflowmgmt.workflow_stage_history wsh4
                                WHERE wsh4.document_workflow_id = dw.id
                                AND wsh4.processed_by = @UserId
                            ) THEN true ELSE false END as IsProcessedBy,
                            (SELECT wsh5.processed_date FROM workflowmgmt.workflow_stage_history wsh5
                             WHERE wsh5.document_workflow_id = dw.id
                             AND wsh5.processed_by = @UserId
                             ORDER BY wsh5.processed_date DESC LIMIT 1) as LastActionDate,
                            (SELECT wsh6.action_taken FROM workflowmgmt.workflow_stage_history wsh6
                             WHERE wsh6.document_workflow_id = dw.id
                             AND wsh6.processed_by = @UserId
                             ORDER BY wsh6.processed_date DESC LIMIT 1) as LastActionTaken,
                            CASE
                                WHEN (SELECT COUNT(*) FROM workflowmgmt.workflow_stages ws_count WHERE ws_count.workflow_template_id = dw.workflow_template_id) > 0 THEN
                                    ROUND((COALESCE(ws.stage_order, 0) * 100.0) /
                                          (SELECT COUNT(*) FROM workflowmgmt.workflow_stages ws_count WHERE ws_count.workflow_template_id = dw.workflow_template_id))
                                ELSE 0
                            END::int as Progress,
                            (SELECT COUNT(*) FROM workflowmgmt.workflow_stages ws_count WHERE ws_count.workflow_template_id = dw.workflow_template_id)::int as TotalStages,
                            COALESCE(ws.stage_order, 0)::int as CompletedStages,
                            null as Priority,
                            null as DueDate,
                            null as Version,
                            0 as Comments
                        FROM workflowmgmt.document_workflows dw
                        INNER JOIN workflowmgmt.workflow_stage_history wsh ON dw.id = wsh.document_workflow_id
                        INNER JOIN workflowmgmt.workflow_templates wt ON dw.workflow_template_id = wt.id
                        LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                        LEFT JOIN workflowmgmt.syllabi s ON dw.document_id = s.id AND dw.document_type = 'syllabus'
                        LEFT JOIN workflowmgmt.lesson_plans lp ON dw.document_id = lp.id AND dw.document_type = 'lesson'
                        LEFT JOIN workflowmgmt.sessions sess ON dw.document_id = sess.id AND dw.document_type = 'session'
                        LEFT JOIN workflowmgmt.departments d ON (
                            (dw.document_type = 'syllabus' AND s.department_id = d.id) OR
                            (dw.document_type = 'lesson' AND EXISTS (SELECT 1 FROM workflowmgmt.syllabi s2 WHERE s2.id = lp.syllabus_id AND s2.department_id = d.id)) OR
                            (dw.document_type = 'session' AND EXISTS (SELECT 1 FROM workflowmgmt.lesson_plans lp2 INNER JOIN workflowmgmt.syllabi s3 ON lp2.syllabus_id = s3.id WHERE lp2.id = sess.lesson_plan_id AND s3.department_id = d.id))
                        )
                        WHERE (wsh.processed_by = @UserId OR wsh.assigned_to = @UserId)
                        AND (@DocumentType IS NULL OR dw.document_type = @DocumentType)
                        ORDER BY CreatedDate desc";

            var parameters = new { UserId = userId, DocumentType = documentType };
            var result = await Connection.QueryAsync<DocumentStatusDto>(query, parameters, transaction: Transaction);

            return result;
        }

        public async Task<DocumentStatusDetailDto?> GetDocumentStatusDetailAsync(Guid documentId, string documentType, Guid userId)
        {
            // This method would need to be implemented based on specific requirements
            // For now, returning null as placeholder
            await Task.CompletedTask;
            return null;
        }

        public async Task<IEnumerable<WorkflowRoadmapDto>> GetWorkflowRoadmapAsync(Guid workflowTemplateId)
        {
            var sql = @"
                SELECT
                    ws.id::text as StageId,
                    ws.stage_name as StageName,
                    ws.stage_order as StageOrder,
                    ws.description as Description,
                    ws.assigned_role as AssignedRole,
                    ws.is_required as IsRequired,
                    ws.auto_approve as AutoApprove,
                    ws.timeout_days as TimeoutDays,
                    ws.is_active as IsActive,
                    'pending' as Status,
                    null as CompletedDate,
                    null as AssignedTo,
                    null as AssignedToName
                FROM workflowmgmt.workflow_stages ws
                WHERE ws.workflow_template_id = @WorkflowTemplateId
                AND ws.is_active = true
                ORDER BY ws.stage_order";

            var result = await Connection.QueryAsync<WorkflowRoadmapDto>(sql,
                new { WorkflowTemplateId = workflowTemplateId },
                transaction: Transaction);

            // Set available actions for each stage (placeholder)
            foreach (var stage in result)
            {
                stage.AvailableActions = [];
            }

            return result;
        }

        public async Task<DocumentStatusStatsDto> GetUserDocumentStatsAsync(Guid userId)
        {
            var sql = @"
                WITH user_documents AS (
                    SELECT DISTINCT
                        dw.document_id,
                        dw.document_type,
                        dw.status
                    FROM workflowmgmt.document_workflows dw
                    INNER JOIN workflowmgmt.workflow_stage_history wsh ON dw.id = wsh.document_workflow_id
                    WHERE (wsh.processed_by = @UserId OR wsh.assigned_to = @UserId)
                ),
                document_statuses AS (
                    SELECT
                        ud.document_id,
                        ud.document_type,
                        CASE
                            WHEN s.status IS NOT NULL THEN s.status
                            WHEN lp.status IS NOT NULL THEN lp.status
                            WHEN sess.status IS NOT NULL THEN sess.status
                            ELSE ud.status
                        END as status
                    FROM user_documents ud
                    LEFT JOIN workflowmgmt.syllabi s ON ud.document_id = s.id AND ud.document_type = 'syllabus'
                    LEFT JOIN workflowmgmt.lesson_plans lp ON ud.document_id = lp.id AND ud.document_type = 'lesson'
                    LEFT JOIN workflowmgmt.sessions sess ON ud.document_id = sess.id AND ud.document_type = 'session'
                )
                SELECT
                    COUNT(*) as TotalDocuments,
                    COUNT(CASE WHEN status IN ('under_review', 'review', 'in_progress') THEN 1 END) as UnderReview,
                    COUNT(CASE WHEN status IN ('approved', 'published') THEN 1 END) as Approved,
                    COUNT(CASE WHEN status = 'revision_required' THEN 1 END) as RevisionRequired,
                    COUNT(CASE WHEN status = 'draft' THEN 1 END) as Drafts
                FROM document_statuses";

            var result = await Connection.QuerySingleOrDefaultAsync<DocumentStatusStatsDto>(sql,
                new { UserId = userId },
                transaction: Transaction);

            return result ?? new DocumentStatusStatsDto();
        }

        public async Task<IEnumerable<DocumentUserHistoryDto>> GetUserDocumentHistoryAsync(Guid documentId, string documentType, Guid userId)
        {
            var sql = @"
                SELECT
                    wsh.id::text as Id,
                    wsh.stage_id::text as StageId,
                    ws.stage_name as StageName,
                    ws.stage_order as StageOrder,
                    wsh.action_taken as ActionTaken,
                    wsh.processed_by::text as ProcessedBy,
                    u1.name as ProcessedByName,
                    wsh.assigned_to::text as AssignedTo,
                    u2.name as AssignedToName,
                    wsh.processed_date as ProcessedDate,
                    wsh.comments as Comments,
                    wsh.attachments as Attachments
                FROM workflowmgmt.workflow_stage_history wsh
                INNER JOIN workflowmgmt.document_workflows dw ON wsh.document_workflow_id = dw.id
                INNER JOIN workflowmgmt.workflow_stages ws ON wsh.stage_id = ws.id
                LEFT JOIN workflowmgmt.users u1 ON wsh.processed_by = u1.id
                LEFT JOIN workflowmgmt.users u2 ON wsh.assigned_to = u2.id
                WHERE dw.document_id = @DocumentId::text
                AND dw.document_type = @DocumentType
                AND wsh.processed_by = @UserId
                ORDER BY wsh.processed_date DESC";

            var result = await Connection.QueryAsync<DocumentUserHistoryDto>(sql,
                new { DocumentId = documentId, DocumentType = documentType, UserId = userId },
                transaction: Transaction);

            return result;
        }
    }
}

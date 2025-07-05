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
            // Optimized approach with CTEs and pre-calculated values
            var query = @"
                        WITH last_stages AS (
                            SELECT
                                workflow_template_id,
                                id as last_stage_id,
                                stage_name as last_stage_name,
                                stage_order as last_stage_order
                            FROM workflowmgmt.workflow_stages ws1
                            WHERE ws1.stage_order = (
                                SELECT MAX(ws2.stage_order)
                                FROM workflowmgmt.workflow_stages ws2
                                WHERE ws2.workflow_template_id = ws1.workflow_template_id
                            )
                        ),
                        stage_counts AS (
                            SELECT
                                workflow_template_id,
                                COUNT(*) as total_stages
                            FROM workflowmgmt.workflow_stages
                            GROUP BY workflow_template_id
                        ),
                        user_actions AS (
                            SELECT DISTINCT
                                wsh.document_workflow_id,
                                MAX(CASE WHEN wsh.processed_by = @UserId THEN wsh.processed_date END) as last_action_date,
                                MAX(CASE WHEN wsh.processed_by = @UserId THEN wsh.action_taken END) as last_action_taken,
                                BOOL_OR(wsh.assigned_to = @UserId) as is_assigned_to,
                                BOOL_OR(wsh.processed_by = @UserId) as is_processed_by
                            FROM workflowmgmt.workflow_stage_history wsh
                            WHERE wsh.processed_by = @UserId OR wsh.assigned_to = @UserId
                            GROUP BY wsh.document_workflow_id
                        )
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
                            COALESCE(
                                CASE WHEN dw.document_type = 'syllabus' THEN u_s.username END,
                                CASE WHEN dw.document_type = 'lesson' THEN u_l.username END,
                                CASE WHEN dw.document_type = 'session' THEN u_sess.username END
                            ) as FacultyName,
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
                            CASE
                                WHEN (
                                    (dw.document_type = 'syllabus' AND s.status = 'Published') OR
                                    (dw.document_type = 'lesson' AND lp.status = 'Published') OR
                                    (dw.document_type = 'session' AND sess.status = 'Published')
                                ) THEN ls.last_stage_id
                                ELSE dw.current_stage_id
                            END as CurrentStageId,
                            CASE
                                WHEN (
                                    (dw.document_type = 'syllabus' AND s.status = 'Published') OR
                                    (dw.document_type = 'lesson' AND lp.status = 'Published') OR
                                    (dw.document_type = 'session' AND sess.status = 'Published')
                                ) THEN ls.last_stage_name
                                ELSE ws.stage_name
                            END as CurrentStageName,
                            CASE
                                WHEN (
                                    (dw.document_type = 'syllabus' AND s.status = 'Published') OR
                                    (dw.document_type = 'lesson' AND lp.status = 'Published') OR
                                    (dw.document_type = 'session' AND sess.status = 'Published')
                                ) THEN ls.last_stage_order
                                ELSE ws.stage_order
                            END as CurrentStageOrder,
                            COALESCE(ua.is_assigned_to, false) as IsAssignedTo,
                            COALESCE(ua.is_processed_by, false) as IsProcessedBy,
                            ua.last_action_date as LastActionDate,
                            ua.last_action_taken as LastActionTaken,
                            CASE
                                -- When document status is 'Completed', progress should be 100%
                                WHEN (
                                    (dw.document_type = 'syllabus' AND s.status = 'Published') OR
                                    (dw.document_type = 'lesson' AND lp.status = 'Published') OR
                                    (dw.document_type = 'session' AND sess.status = 'Published')
                                ) THEN 100
                                -- When document is in first stage, progress should be 0%
                                WHEN COALESCE(ws.stage_order, 1) = 1 THEN 0
                                -- For other stages, calculate progress based on completed stages
                                WHEN sc.total_stages > 0 THEN
                                    ROUND(((COALESCE(ws.stage_order, 1) - 1) * 100.0) / sc.total_stages)
                                ELSE 0
                            END::int as Progress,
                            sc.total_stages::int as TotalStages,
                            CASE
                                -- When document status is 'Completed', all stages are completed
                                WHEN (
                                    (dw.document_type = 'syllabus' AND s.status = 'Published') OR
                                    (dw.document_type = 'lesson' AND lp.status = 'Published') OR
                                    (dw.document_type = 'session' AND sess.status = 'Published')
                                ) THEN (SELECT COUNT(*) FROM workflowmgmt.workflow_stages ws_count WHERE ws_count.workflow_template_id = dw.workflow_template_id)
                                -- When in first stage, no stages are completed yet
                                WHEN COALESCE(ws.stage_order, 1) = 1 THEN 0
                                -- For other stages, previous stages are completed
                                ELSE COALESCE(ws.stage_order, 1) - 1
                            END::int as CompletedStages,
                            null as Priority,
                            null as DueDate,
                            null as Version,
                            0 as Comments
                        FROM workflowmgmt.document_workflows dw
                        INNER JOIN user_actions ua ON dw.id = ua.document_workflow_id
                        INNER JOIN workflowmgmt.workflow_templates wt ON dw.workflow_template_id = wt.id
                        LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                        LEFT JOIN last_stages ls ON dw.workflow_template_id = ls.workflow_template_id
                        LEFT JOIN stage_counts sc ON dw.workflow_template_id = sc.workflow_template_id
                        LEFT JOIN workflowmgmt.syllabi s ON dw.document_id = s.id AND dw.document_type = 'syllabus'
                        LEFT JOIN workflowmgmt.lesson_plans lp ON dw.document_id = lp.id AND dw.document_type = 'lesson'
                        LEFT JOIN workflowmgmt.sessions sess ON dw.document_id = sess.id AND dw.document_type = 'session'
                        LEFT JOIN workflowmgmt.users u_s ON s.faculty_id = u_s.id
                        LEFT JOIN workflowmgmt.users u_l ON lp.faculty_id = u_l.id
                        LEFT JOIN workflowmgmt.users u_sess ON sess.faculty_id = u_sess.id
                        LEFT JOIN workflowmgmt.departments d ON (
                            (dw.document_type = 'syllabus' AND s.department_id = d.id) OR
                            (dw.document_type = 'lesson' AND lp.syllabus_id IN (SELECT id FROM workflowmgmt.syllabi WHERE department_id = d.id)) OR
                            (dw.document_type = 'session' AND sess.lesson_plan_id IN (SELECT lp2.id FROM workflowmgmt.lesson_plans lp2 INNER JOIN workflowmgmt.syllabi s3 ON lp2.syllabus_id = s3.id WHERE s3.department_id = d.id))
                        )
                        WHERE (@DocumentType IS NULL OR dw.document_type = @DocumentType)
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

        public async Task<IEnumerable<DocumentWorkflowHistoryDto>> GetDocumentWorkflowHistoryAsync(string documentId, string documentType, Guid userId)
        {
            var sql = @"
                SELECT
                    wsh.id as Id,
                    wsh.document_workflow_id as DocumentWorkflowId,
                    wsh.stage_id as StageId,
                    ws.stage_name as StageName,
                    ws.stage_order as StageOrder,
                    wsh.action_taken as ActionTaken,
                    wsh.processed_by as ProcessedBy,
                    COALESCE(u1.username, 'Unknown User') as ProcessedByName,
                    wsh.assigned_to as AssignedTo,
                    COALESCE(u2.username, 'Unknown User') as AssignedToName,
                    wsh.processed_date as ProcessedDate,
                    wsh.comments as Comments,
                    wsh.attachments as Attachments,
                    wsh.created_date as CreatedDate,
                    dw.document_id as DocumentId,
                    dw.document_type as DocumentType,
                    CASE
                        WHEN dw.document_type = 'syllabus' THEN s.title
                        WHEN dw.document_type = 'lesson' THEN lp.title
                        WHEN dw.document_type = 'session' THEN sess.title
                        ELSE 'Unknown Document'
                    END as DocumentTitle,
                    wt.name as WorkflowTemplateName
                FROM workflowmgmt.workflow_stage_history wsh
                INNER JOIN workflowmgmt.document_workflows dw ON wsh.document_workflow_id = dw.id
                INNER JOIN workflowmgmt.workflow_templates wt ON dw.workflow_template_id = wt.id
                INNER JOIN workflowmgmt.workflow_stages ws ON wsh.stage_id = ws.id
                LEFT JOIN workflowmgmt.users u1 ON wsh.processed_by = u1.id
                LEFT JOIN workflowmgmt.users u2 ON wsh.assigned_to = u2.id
                LEFT JOIN workflowmgmt.syllabi s ON dw.document_id = s.id AND dw.document_type = 'syllabus'
                LEFT JOIN workflowmgmt.lesson_plans lp ON dw.document_id = lp.id AND dw.document_type = 'lesson'
                LEFT JOIN workflowmgmt.sessions sess ON dw.document_id = sess.id AND dw.document_type = 'session'
                WHERE dw.document_id = @DocumentId::uuid
                AND dw.document_type = @DocumentType
                AND (wsh.processed_by = @UserId OR wsh.assigned_to = @UserId OR @UserId IN (
                    SELECT DISTINCT processed_by FROM workflowmgmt.workflow_stage_history wsh2
                    WHERE wsh2.document_workflow_id = dw.id
                ) OR @UserId IN (
                    SELECT DISTINCT assigned_to FROM workflowmgmt.workflow_stage_history wsh3
                    WHERE wsh3.document_workflow_id = dw.id AND assigned_to IS NOT NULL
                ))
                ORDER BY wsh.processed_date DESC, ws.stage_order DESC";

            var result = await Connection.QueryAsync<DocumentWorkflowHistoryDto>(sql,
                new { DocumentId = documentId, DocumentType = documentType, UserId = userId },
                transaction: Transaction);

            return result;
        }
    }
}

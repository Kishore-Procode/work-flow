using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Domain.Models.Workflow;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class DocumentLifecycleRepository : RepositoryTranBase, IDocumentLifecycleRepository
    {
        public DocumentLifecycleRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<DocumentLifecycleDto>> GetDocumentsAssignedToUserAsync(Guid userId, string? documentType = null)
        {
            var sql = @"
                SELECT DISTINCT
                    s.id as DocumentId,
                    'syllabus' as DocumentType,
                    s.title as Title,
                    s.status as Status,
                    d.name as DepartmentName,
                    s.faculty_name as FacultyName,
                    s.created_date as CreatedDate,
                    s.modified_date as ModifiedDate,
                    dw.id as WorkflowId,
                    wt.name as WorkflowTemplateName,
                    dw.current_stage_id as CurrentStageId,
                    ws.stage_name as CurrentStageName,
                    ws.stage_order as CurrentStageOrder,
                    dw.assigned_to as AssignedTo,
                    u.username as AssignedToName
                FROM workflowmgmt.syllabi s
                INNER JOIN workflowmgmt.departments d ON s.department_id = d.id
                INNER JOIN workflowmgmt.document_workflows dw ON s.id = dw.document_id AND dw.document_type = 'syllabus'
                INNER JOIN workflowmgmt.workflow_templates wt ON dw.workflow_template_id = wt.id
                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                LEFT JOIN workflowmgmt.users u ON dw.assigned_to = u.id
                WHERE dw.assigned_to = @UserId 
                AND dw.status IN ('In Progress', 'On Hold')
                AND s.is_active = true";

            sql += " ORDER BY s.modified_date DESC";

            object parameters;
            if (!string.IsNullOrEmpty(documentType))
            {
                sql = sql.Replace("AND s.is_active = true", "AND s.is_active = true AND dw.document_type = @DocumentType");
                parameters = new { UserId = userId, DocumentType = documentType };
            }
            else
            {
                parameters = new { UserId = userId };
            }

            var documents = await Connection.QueryAsync<DocumentLifecycleDto>(sql, parameters, transaction: Transaction);

            // Get available actions and recent feedback for each document
            foreach (var doc in documents)
            {
                doc.AvailableActions = (await GetAvailableActionsAsync(userId, doc.DocumentId, doc.DocumentType)).ToList();
                doc.RecentFeedback = (await GetRecentFeedbackAsync(doc.DocumentId, doc.DocumentType)).ToList();
            }

            return documents.ToList();
        }

        public async Task<DocumentLifecycleDto?> GetDocumentLifecycleAsync(Guid documentId, string documentType, Guid userId)
        {
            var sql = @"
                SELECT 
                    s.id as DocumentId,
                    'syllabus' as DocumentType,
                    s.title as Title,
                    s.status as Status,
                    d.name as DepartmentName,
                    s.faculty_name as FacultyName,
                    s.created_date as CreatedDate,
                    s.modified_date as ModifiedDate,
                    dw.id as WorkflowId,
                    wt.name as WorkflowTemplateName,
                    dw.current_stage_id as CurrentStageId,
                    ws.stage_name as CurrentStageName,
                    ws.stage_order as CurrentStageOrder,
                    dw.assigned_to as AssignedTo,
                    u.name as AssignedToName
                FROM workflowmgmt.syllabi s
                INNER JOIN workflowmgmt.departments d ON s.department_id = d.id
                INNER JOIN workflowmgmt.document_workflows dw ON s.id::text = dw.document_id AND dw.document_type = 'syllabus'
                INNER JOIN workflowmgmt.workflow_templates wt ON dw.workflow_template_id = wt.id
                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                LEFT JOIN workflowmgmt.users u ON dw.assigned_to = u.id
                WHERE s.id = @DocumentId 
                AND dw.document_type = @DocumentType
                AND s.is_active = true";

            var document = await Connection.QuerySingleOrDefaultAsync<DocumentLifecycleDto>(sql, 
                new { DocumentId = documentId, DocumentType = documentType }, 
                transaction: Transaction);

            if (document != null)
            {
                document.AvailableActions = (await GetAvailableActionsAsync(userId, documentId, documentType)).ToList();
                document.RecentFeedback = (await GetRecentFeedbackAsync(documentId, documentType)).ToList();
            }

            return document;
        }

        public async Task<bool> CanUserPerformActionAsync(Guid userId, Guid documentId, string documentType, Guid actionId)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM workflowmgmt.document_workflows dw
                INNER JOIN workflowmgmt.workflow_stage_actions wsa ON dw.current_stage_id = wsa.workflow_stage_id
                INNER JOIN workflowmgmt.workflow_stage_roles wsr ON dw.current_stage_id = wsr.workflow_stage_id
                INNER JOIN workflowmgmt.roles r ON r.code = wsr.role_code
                INNER JOIN workflowmgmt.workflow_role_mapping wrm ON r.id = wrm.role_id
                INNER JOIN workflowmgmt.syllabi s ON dw.document_id = s.id
                WHERE dw.document_id = @DocumentId
                AND dw.document_type = @DocumentType
                AND wsa.id = @ActionId
                AND wrm.user_id = @UserId
                AND wrm.department_id = s.department_id
                AND wsa.is_active = true
                AND dw.status <> 'Completed'";

            var count = await Connection.QuerySingleAsync<int>(sql, 
                new { UserId = userId, DocumentId = documentId, DocumentType = documentType, ActionId = actionId }, 
                transaction: Transaction);

            return count > 0;
        }

        public async Task<List<WorkflowStageActionDto>> GetAvailableActionsAsync(Guid userId, Guid documentId, string documentType)
        {
            var sql = @"
                SELECT DISTINCT
                    wsa.id as Id,
                    wsa.workflow_stage_id as WorkflowStageId,
                    wsa.action_name as ActionName,
                    wsa.action_type as ActionType,
                    wsa.next_stage_id as NextStageId,
                    wsa.is_active as IsActive,
                    wsa.created_date as CreatedDate
                FROM workflowmgmt.document_workflows dw
                INNER JOIN workflowmgmt.workflow_stage_actions wsa ON dw.current_stage_id = wsa.workflow_stage_id
                INNER JOIN workflowmgmt.workflow_stage_roles wsr ON dw.current_stage_id = wsr.workflow_stage_id
                INNER JOIN workflowmgmt.roles r ON r.code = wsr.role_code
                INNER JOIN workflowmgmt.workflow_role_mapping wrm ON r.id = wrm.role_id
                INNER JOIN workflowmgmt.syllabi s ON dw.document_id = s.id
                WHERE dw.document_id = @DocumentId
                AND dw.document_type = @DocumentType
                AND wrm.user_id = @UserId
                AND wrm.department_id = s.department_id
                AND wsa.is_active = true
                AND dw.status <> 'Completed'
                ORDER BY wsa.action_name";

            var actions = await Connection.QueryAsync<WorkflowStageActionDto>(sql,
                new { UserId = userId, DocumentId = documentId, DocumentType = documentType },
                transaction: Transaction);
            return actions.ToList();
        }

        private async Task<List<DocumentFeedbackDto>> GetRecentFeedbackAsync(Guid documentId, string documentType, int limit = 5)
        {
            var sql = @"
                SELECT 
                    df.id as Id,
                    df.document_id as DocumentId,
                    df.document_type as DocumentType,
                    df.workflow_stage_id as WorkflowStageId,
                    df.feedback_provider as FeedbackProvider,
                    df.feedback_text as FeedbackText,
                    df.feedback_type as FeedbackType,
                    df.is_addressed as IsAddressed,
                    df.addressed_by as AddressedBy,
                    df.addressed_date as AddressedDate,
                    df.is_active as IsActive,
                    df.created_date as CreatedDate,
                    df.modified_date as ModifiedDate,
                    fp.username as FeedbackProviderName,
                    ab.username as AddressedByName,
                    ws.stage_name as StageName
                FROM workflowmgmt.document_feedback df
                LEFT JOIN workflowmgmt.users fp ON df.feedback_provider = fp.id
                LEFT JOIN workflowmgmt.users ab ON df.addressed_by = ab.id
                LEFT JOIN workflowmgmt.workflow_stages ws ON df.workflow_stage_id = ws.id
                WHERE df.document_id = @DocumentId 
                AND df.document_type = @DocumentType 
                AND df.is_active = true
                ORDER BY df.created_date DESC
                LIMIT @Limit";

            var feedback = await Connection.QueryAsync<DocumentFeedbackDto>(sql,
                new { DocumentId = documentId, DocumentType = documentType, Limit = limit },
                transaction: Transaction);
            return feedback.ToList();
        }

        public async Task<bool> ProcessDocumentActionAsync(ProcessDocumentActionDto actionDto, Guid processedBy)
        {
            try
            {
                // Validate input parameters
                if (actionDto == null || processedBy == Guid.Empty)
                {
                    throw new ArgumentException("Invalid input parameters");
                }

                // Get the action details with validation
                var actionSql = @"
                    SELECT wsa.id, wsa.workflow_stage_id, wsa.action_name, wsa.action_type, wsa.next_stage_id, wsa.is_active,
                           ws.stage_name, ws.stage_order
                    FROM workflowmgmt.workflow_stage_actions wsa
                    INNER JOIN workflowmgmt.workflow_stages ws ON wsa.workflow_stage_id = ws.id
                    WHERE wsa.id = @ActionId AND wsa.is_active = true";

                var action = await Connection.QuerySingleOrDefaultAsync(actionSql,
                    new { ActionId = actionDto.ActionId },
                    transaction: Transaction);

                if (action == null)
                {
                    throw new InvalidOperationException($"Action with ID {actionDto.ActionId} not found or inactive");
                }

                // Get current document workflow with validation
                var workflowSql = @"
                    SELECT dw.id, dw.document_id, dw.document_type, dw.workflow_template_id,
                           dw.current_stage_id, dw.assigned_to, dw.status, dw.initiated_by,
                           ws.stage_name as current_stage_name, ws.stage_order as current_stage_order
                    FROM workflowmgmt.document_workflows dw
                    LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                    WHERE dw.document_id = @DocumentId AND dw.document_type = @DocumentType
                    AND dw.is_active = true";

                var documentWorkflow = await Connection.QuerySingleOrDefaultAsync(workflowSql,
                    new { DocumentId = actionDto.DocumentId, DocumentType = actionDto.DocumentType },
                    transaction: Transaction);

                if (documentWorkflow == null)
                {
                    throw new InvalidOperationException($"Document workflow not found for document {actionDto.DocumentId}");
                }

                // Validate that the action belongs to the current stage
                if (action.workflow_stage_id != documentWorkflow.current_stage_id)
                {
                    throw new InvalidOperationException("Action does not belong to the current workflow stage");
                }

                // Validate workflow status
                if (documentWorkflow.status == "Completed" || documentWorkflow.status == "Cancelled")
                {
                    throw new InvalidOperationException($"Cannot perform actions on a {documentWorkflow.status.ToLower()} workflow");
                }

                // Create feedback if comments provided
                Guid? feedbackId = null;
                if (!string.IsNullOrEmpty(actionDto.Comments))
                {
                    feedbackId = Guid.NewGuid();
                    var feedbackSql = @"
                        INSERT INTO workflowmgmt.document_feedback
                        (id, document_id, document_type, workflow_stage_id, feedback_provider,
                         feedback_text, feedback_type, is_addressed, is_active, created_date)
                        VALUES (@Id, @DocumentId, @DocumentType, @WorkflowStageId, @FeedbackProvider,
                                @FeedbackText, @FeedbackType, false, true, @CreatedDate)";

                    await Connection.ExecuteAsync(feedbackSql, new
                    {
                        Id = feedbackId,
                        DocumentId = actionDto.DocumentId,
                        DocumentType = actionDto.DocumentType,
                        WorkflowStageId = documentWorkflow.current_stage_id,
                        FeedbackProvider = processedBy,
                        FeedbackText = actionDto.Comments,
                        FeedbackType = actionDto.FeedbackType ?? "general",
                        CreatedDate = DateTime.UtcNow
                    }, transaction: Transaction);
                }
                var isDraftStatus = await CheckDraftStatus(action.next_stage_id);
                bool isDocumentRejected = action.action_type.ToLower() == "reject";
                // Determine document status and workflow status based on action
                var newDocumentStatus = isDraftStatus ? "Draft" : DetermineDocumentStatus(action.action_type, action.action_name, action.next_stage_id != null);
                var newWorkflowStatus = DetermineWorkflowStatus(action.action_type, action.next_stage_id != null);

                if(isDocumentRejected)
                {
                    newDocumentStatus = "Rejected";
                    newWorkflowStatus = "Cancelled";
                }

                // Determine next stage assignment
                Guid? nextAssignedTo = null;
                if (action.next_stage_id != null)
                {
                    nextAssignedTo = await GetNextStageAssigneeAsync(action.next_stage_id, actionDto.DocumentId);
                }

                // Update document workflow with new stage and assignment
                var updateWorkflowSql = @"
                    UPDATE workflowmgmt.document_workflows
                    SET current_stage_id = @CurrentStageId,
                        status = @Status,
                        assigned_to = @AssignedTo,
                        completed_date = @CompletedDate,
                        modified_date = @ModifiedDate
                    WHERE id = @Id";

                await Connection.ExecuteAsync(updateWorkflowSql, new
                {
                    Id = documentWorkflow.id,
                    CurrentStageId = isDocumentRejected ? documentWorkflow.current_stage_id : action.next_stage_id,
                    Status = newWorkflowStatus,
                    AssignedTo = nextAssignedTo,
                    CompletedDate = newWorkflowStatus == "Completed" ? DateTime.UtcNow : (DateTime?)null,
                    ModifiedDate = DateTime.UtcNow
                }, transaction: Transaction);

                // Update document status in syllabus table
                await UpdateDocumentStatusAsync(actionDto.DocumentId, actionDto.DocumentType, newDocumentStatus);

                // Create workflow stage history with enhanced details
                var historyId = Guid.NewGuid();
                var historySql = @"
                    INSERT INTO workflowmgmt.workflow_stage_history
                    (id, document_workflow_id, stage_id, action_taken, processed_by, assigned_to,
                     processed_date, comments, created_date)
                    VALUES (@Id, @DocumentWorkflowId, @StageId, @ActionTaken, @ProcessedBy, @AssignedTo,
                            @ProcessedDate, @Comments, @CreatedDate)";

                await Connection.ExecuteAsync(historySql, new
                {
                    Id = historyId,
                    DocumentWorkflowId = documentWorkflow.id,
                    StageId = documentWorkflow.current_stage_id,
                    ActionTaken = action.action_name,
                    ProcessedBy = processedBy,
                    AssignedTo = nextAssignedTo,
                    ProcessedDate = DateTime.UtcNow,
                    Comments = actionDto.Comments,
                    CreatedDate = DateTime.UtcNow
                }, transaction: Transaction);

                // Log the action for audit purposes
                await LogWorkflowActionAsync(documentWorkflow.id, action.action_name, processedBy,
                    actionDto.Comments, feedbackId, historyId);

                return true;
            }
            catch (ArgumentException ex)
            {
                // Log validation errors
                throw new InvalidOperationException($"Validation error: {ex.Message}", ex);
            }
            catch (InvalidOperationException)
            {
                // Re-throw business logic errors
                throw;
            }
            catch (Exception ex)
            {
                // Log unexpected errors and wrap them
                throw new InvalidOperationException($"Failed to process document action: {ex.Message}", ex);
            }
        }

        private string DetermineDocumentStatus(string actionType, string actionName, bool hasNextStage)
        {
            // Status logic as per requirements
            if (actionType.ToLower() == "approve" && !hasNextStage)
            {
                return "Published";
            }
            else if (actionName.ToLower().Contains("review"))
            {
                return "Under Review";
            }
            else if (actionType.ToLower() == "reject")
            {
                return "Rejected";
            }
            else
            {
                return "In Progress";
            }
        }

        private string DetermineWorkflowStatus(string actionType, bool hasNextStage)
        {
            // Determine workflow status based on action and next stage
            if (!hasNextStage && actionType.ToLower() == "approve")
            {
                return "Completed";
            }
            else if (actionType.ToLower() == "reject")
            {
                return "Cancelled";
            }
            else
            {
                return "In Progress";
            }
        }

        private async Task<bool> CheckDraftStatus(Guid? nextStageId)
        {
            if (!nextStageId.HasValue) return false;

            var stageInfoSql = @"
                SELECT stage_order
                FROM workflowmgmt.workflow_stages
                WHERE id = @StageId";

            var stageOrder = await Connection.QuerySingleOrDefaultAsync<int?>(stageInfoSql,
                new { StageId = nextStageId },
                transaction: Transaction);

            return stageOrder == 1 ? true : false;
        }
        private async Task<Guid?> GetNextStageAssigneeAsync(Guid? nextStageId, Guid documentId)
        {
            if (!nextStageId.HasValue) return null;

            // First, check if this stage is marked as a faculty stage (stage_order = 1)
            var stageInfoSql = @"
                SELECT stage_order
                FROM workflowmgmt.workflow_stages
                WHERE id = @StageId";

            var stageOrder = await Connection.QuerySingleOrDefaultAsync<int?>(stageInfoSql,
                new { StageId = nextStageId },
                transaction: Transaction);

            // If stage_order = 1, assign to faculty_id from syllabi table
            if (stageOrder == 1)
            {
                var facultyAssignmentSql = @"
                    SELECT faculty_id
                    FROM workflowmgmt.syllabi
                    WHERE id = @DocumentId
                    AND faculty_id IS NOT NULL";

                var facultyId = await Connection.QuerySingleOrDefaultAsync<Guid?>(facultyAssignmentSql,
                    new { DocumentId = documentId },
                    transaction: Transaction);

                if (facultyId.HasValue)
                {
                    return facultyId;
                }
            }

            // Continue with current functionality for other stages
            // Get the primary user assigned to the next stage roles for this document's department
            var assignmentSql = @"
                SELECT DISTINCT wrm.user_id
                FROM workflowmgmt.workflow_stage_roles wsr
                INNER JOIN workflowmgmt.roles r ON r.code = wsr.role_code
                INNER JOIN workflowmgmt.workflow_role_mapping wrm ON r.id = wrm.role_id
                INNER JOIN workflowmgmt.syllabi s ON wrm.department_id = s.department_id
                WHERE wsr.workflow_stage_id = @StageId
                AND s.id = @DocumentId
                AND wrm.isprimary = true
                AND wsr.is_required = true
                LIMIT 1";

            var assignedUserId = await Connection.QuerySingleOrDefaultAsync<Guid?>(assignmentSql,
                new { StageId = nextStageId, DocumentId = documentId },
                transaction: Transaction);

            // If no primary user found, get any user with the required role
            if (!assignedUserId.HasValue)
            {
                var fallbackSql = @"
                    SELECT DISTINCT wrm.user_id
                    FROM workflowmgmt.workflow_stage_roles wsr
                    INNER JOIN workflowmgmt.workflow_role_mapping wrm ON wsr.role_code = wrm.role_code
                    INNER JOIN workflowmgmt.syllabi s ON wrm.department_id = s.department_id
                    WHERE wsr.workflow_stage_id = @StageId
                    AND s.id = @DocumentId
                    ORDER BY wrm.created_date ASC
                    LIMIT 1";

                assignedUserId = await Connection.QuerySingleOrDefaultAsync<Guid?>(fallbackSql,
                    new { StageId = nextStageId, DocumentId = documentId },
                    transaction: Transaction);
            }

            return assignedUserId;
        }

        private async Task LogWorkflowActionAsync(Guid documentWorkflowId, string actionName, Guid processedBy,
            string? comments, Guid? feedbackId, Guid historyId)
        {
            // This method can be used for additional logging, notifications, or audit trails
            // For now, we'll just ensure the action is properly recorded
            // In a full implementation, you might:
            // 1. Send notifications to assigned users
            // 2. Log to an audit table
            // 3. Trigger external integrations
            // 4. Update metrics/analytics

            // Example: Log to a separate audit table (if it exists)
            try
            {
                var auditSql = @"
                    INSERT INTO workflowmgmt.workflow_audit_log
                    (id, document_workflow_id, action_name, processed_by, comments,
                     feedback_id, history_id, created_date)
                    VALUES (@Id, @DocumentWorkflowId, @ActionName, @ProcessedBy, @Comments,
                            @FeedbackId, @HistoryId, @CreatedDate)";

                await Connection.ExecuteAsync(auditSql, new
                {
                    Id = Guid.NewGuid(),
                    DocumentWorkflowId = documentWorkflowId,
                    ActionName = actionName,
                    ProcessedBy = processedBy,
                    Comments = comments,
                    FeedbackId = feedbackId,
                    HistoryId = historyId,
                    CreatedDate = DateTime.UtcNow
                }, transaction: Transaction);
            }
            catch
            {
                // Ignore audit logging errors - don't fail the main operation
                // In production, you might want to log this to a separate error log
            }
        }

        public async Task<bool> UpdateDocumentStatusAsync(Guid documentId, string documentType, string status)
        {
            var tableName = GetTableName(documentType);

            var sql = $@"
                    UPDATE workflowmgmt.{tableName} 
                    SET status = @Status, modified_date = @ModifiedDate
                    WHERE id = @DocumentId";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                DocumentId = documentId,
                Status = status,
                ModifiedDate = DateTime.UtcNow
            }, transaction: Transaction);

            return rowsAffected > 0 ? true : false;
        }

        public static string GetTableName(string documentType)
        {
            switch (documentType.ToLower())
            {
                case "syllabus":
                    return "syllabi";
                case "lesson":
                    return "lesson";
                case "session":
                    return "session";
                default:
                    throw new ArgumentException($"Unknown document type: {documentType}");
            }
        }
    }
}

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
    public class DocumentFeedbackRepository : RepositoryTranBase, IDocumentFeedbackRepository
    {
        public DocumentFeedbackRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<DocumentFeedbackDto>> GetByDocumentIdAsync(Guid documentId, string documentType)
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
                    fp.name as FeedbackProviderName,
                    ab.name as AddressedByName,
                    ws.stage_name as StageName
                FROM workflowmgmt.document_feedback df
                LEFT JOIN workflowmgmt.users fp ON df.feedback_provider = fp.id
                LEFT JOIN workflowmgmt.users ab ON df.addressed_by = ab.id
                LEFT JOIN workflowmgmt.workflow_stages ws ON df.workflow_stage_id = ws.id
                WHERE df.document_id = @DocumentId 
                AND df.document_type = @DocumentType 
                AND df.is_active = true
                ORDER BY df.created_date DESC";

            var feedback = await Connection.QueryAsync<DocumentFeedbackDto>(sql,
                new { DocumentId = documentId, DocumentType = documentType },
                transaction: Transaction);
            return feedback.ToList();
        }

        public async Task<DocumentFeedbackDto?> GetByIdAsync(Guid id)
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
                    fp.name as FeedbackProviderName,
                    ab.name as AddressedByName,
                    ws.stage_name as StageName
                FROM workflowmgmt.document_feedback df
                LEFT JOIN workflowmgmt.users fp ON df.feedback_provider = fp.id
                LEFT JOIN workflowmgmt.users ab ON df.addressed_by = ab.id
                LEFT JOIN workflowmgmt.workflow_stages ws ON df.workflow_stage_id = ws.id
                WHERE df.id = @Id AND df.is_active = true";

            return await Connection.QuerySingleOrDefaultAsync<DocumentFeedbackDto>(sql, 
                new { Id = id }, 
                transaction: Transaction);
        }

        public async Task<Guid> CreateAsync(CreateDocumentFeedbackDto feedback, Guid feedbackProvider)
        {
            var id = Guid.NewGuid();
            var sql = @"
                INSERT INTO workflowmgmt.document_feedback 
                (id, document_id, document_type, workflow_stage_id, feedback_provider, 
                 feedback_text, feedback_type, is_addressed, is_active, created_date)
                VALUES (@Id, @DocumentId, @DocumentType, @WorkflowStageId, @FeedbackProvider, 
                        @FeedbackText, @FeedbackType, false, true, @CreatedDate)";

            await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                feedback.DocumentId,
                feedback.DocumentType,
                feedback.WorkflowStageId,
                FeedbackProvider = feedbackProvider,
                feedback.FeedbackText,
                feedback.FeedbackType,
                CreatedDate = DateTime.UtcNow
            }, transaction: Transaction);

            return id;
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateDocumentFeedbackDto feedback)
        {
            var sql = @"
                UPDATE workflowmgmt.document_feedback 
                SET feedback_text = COALESCE(@FeedbackText, feedback_text),
                    feedback_type = COALESCE(@FeedbackType, feedback_type),
                    is_addressed = COALESCE(@IsAddressed, is_addressed),
                    modified_date = @ModifiedDate
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                feedback.FeedbackText,
                feedback.FeedbackType,
                feedback.IsAddressed,
                ModifiedDate = DateTime.UtcNow
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var sql = @"
                UPDATE workflowmgmt.document_feedback 
                SET is_active = false, modified_date = @ModifiedDate
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                ModifiedDate = DateTime.UtcNow
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var sql = "SELECT COUNT(1) FROM workflowmgmt.document_feedback WHERE id = @Id AND is_active = true";
            var count = await Connection.QuerySingleAsync<int>(sql, new { Id = id }, transaction: Transaction);
            return count > 0;
        }

        public async Task<List<DocumentFeedbackDto>> GetByStageIdAsync(Guid stageId)
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
                    fp.name as FeedbackProviderName,
                    ab.name as AddressedByName,
                    ws.stage_name as StageName
                FROM workflowmgmt.document_feedback df
                LEFT JOIN workflowmgmt.users fp ON df.feedback_provider = fp.id
                LEFT JOIN workflowmgmt.users ab ON df.addressed_by = ab.id
                LEFT JOIN workflowmgmt.workflow_stages ws ON df.workflow_stage_id = ws.id
                WHERE df.workflow_stage_id = @StageId AND df.is_active = true
                ORDER BY df.created_date DESC";

            var feedback = await Connection.QueryAsync<DocumentFeedbackDto>(sql,
                new { StageId = stageId },
                transaction: Transaction);
            return feedback.ToList();
        }

        public async Task<List<DocumentFeedbackDto>> GetByProviderAsync(Guid providerId)
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
                    fp.name as FeedbackProviderName,
                    ab.name as AddressedByName,
                    ws.stage_name as StageName
                FROM workflowmgmt.document_feedback df
                LEFT JOIN workflowmgmt.users fp ON df.feedback_provider = fp.id
                LEFT JOIN workflowmgmt.users ab ON df.addressed_by = ab.id
                LEFT JOIN workflowmgmt.workflow_stages ws ON df.workflow_stage_id = ws.id
                WHERE df.feedback_provider = @ProviderId AND df.is_active = true
                ORDER BY df.created_date DESC";

            var feedback = await Connection.QueryAsync<DocumentFeedbackDto>(sql,
                new { ProviderId = providerId },
                transaction: Transaction);
            return feedback.ToList();
        }

        public async Task<List<DocumentFeedbackDto>> GetUnaddressedByDocumentAsync(Guid documentId, string documentType)
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
                    fp.name as FeedbackProviderName,
                    ab.name as AddressedByName,
                    ws.stage_name as StageName
                FROM workflowmgmt.document_feedback df
                LEFT JOIN workflowmgmt.users fp ON df.feedback_provider = fp.id
                LEFT JOIN workflowmgmt.users ab ON df.addressed_by = ab.id
                LEFT JOIN workflowmgmt.workflow_stages ws ON df.workflow_stage_id = ws.id
                WHERE df.document_id = @DocumentId 
                AND df.document_type = @DocumentType 
                AND df.is_addressed = false 
                AND df.is_active = true
                ORDER BY df.created_date DESC";

            var feedback = await Connection.QueryAsync<DocumentFeedbackDto>(sql,
                new { DocumentId = documentId, DocumentType = documentType },
                transaction: Transaction);
            return feedback.ToList();
        }

        public async Task<List<DocumentFeedbackDto>> GetRecentByDocumentAsync(Guid documentId, string documentType, int limit = 5)
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
                    fp.name as FeedbackProviderName,
                    ab.name as AddressedByName,
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

        public async Task<bool> MarkAsAddressedAsync(Guid id, Guid addressedBy)
        {
            var sql = @"
                UPDATE workflowmgmt.document_feedback 
                SET is_addressed = true,
                    addressed_by = @AddressedBy,
                    addressed_date = @AddressedDate,
                    modified_date = @ModifiedDate
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                AddressedBy = addressedBy,
                AddressedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> MarkAsUnaddressedAsync(Guid id)
        {
            var sql = @"
                UPDATE workflowmgmt.document_feedback 
                SET is_addressed = false,
                    addressed_by = NULL,
                    addressed_date = NULL,
                    modified_date = @ModifiedDate
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                ModifiedDate = DateTime.UtcNow
            }, transaction: Transaction);

            return rowsAffected > 0;
        }
    }
}

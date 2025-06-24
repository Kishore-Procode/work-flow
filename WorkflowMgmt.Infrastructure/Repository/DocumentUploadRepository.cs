using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Domain.Models.DocumentUpload;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class DocumentUploadRepository : RepositoryTranBase, IDocumentUploadRepository
    {
        public DocumentUploadRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<IEnumerable<DocumentUploadDto>> GetAllAsync()
        {
            var sql = "SELECT * FROM workflowmgmt.document_uploads WHERE is_active = true ORDER BY upload_date DESC";
            return await Connection.QueryAsync<DocumentUploadDto>(sql, transaction: Transaction);
        }

        public async Task<IEnumerable<DocumentUploadDto>> GetByDocumentIdAsync(Guid documentId)
        {
            var sql = "SELECT * FROM workflowmgmt.document_uploads WHERE document_id = @DocumentId AND is_active = true ORDER BY upload_date DESC";
            return await Connection.QueryAsync<DocumentUploadDto>(sql, new { DocumentId = documentId }, Transaction);
        }

        public async Task<IEnumerable<DocumentUploadDto>> GetByDocumentTypeAsync(string documentType)
        {
            var sql = "SELECT * FROM workflowmgmt.document_uploads WHERE document_type = @DocumentType AND is_active = true ORDER BY upload_date DESC";
            return await Connection.QueryAsync<DocumentUploadDto>(sql, new { DocumentType = documentType }, Transaction);
        }

        public async Task<IEnumerable<DocumentUploadDto>> GetByStatusAsync(string status)
        {
            var sql = "SELECT * FROM workflowmgmt.document_uploads WHERE upload_status = @Status AND is_active = true ORDER BY upload_date DESC";
            return await Connection.QueryAsync<DocumentUploadDto>(sql, new { Status = status }, Transaction);
        }

        public async Task<IEnumerable<DocumentUploadDto>> GetByUploadedByAsync(Guid userId)
        {
            var sql = "SELECT * FROM workflowmgmt.document_uploads WHERE uploaded_by = @UserId AND is_active = true ORDER BY upload_date DESC";
            return await Connection.QueryAsync<DocumentUploadDto>(sql, new { UserId = userId }, Transaction);
        }

        public async Task<DocumentUploadDto?> GetByIdAsync(Guid id)
        {
            var sql = "SELECT * FROM workflowmgmt.document_uploads WHERE id = @Id";
            return await Connection.QueryFirstOrDefaultAsync<DocumentUploadDto>(sql, new { Id = id }, Transaction);
        }

        public async Task<Guid> CreateAsync(CreateDocumentUploadDto documentUpload)
        {
            var id = Guid.NewGuid();
            var sql = @"
                INSERT INTO workflowmgmt.document_uploads 
                (id, document_id, document_type, original_filename, file_size, file_type, file_url, 
                 upload_status, uploaded_by, upload_date, is_active, created_date)
                VALUES (@Id, @DocumentId, @DocumentType, @OriginalFilename, @FileSize, @FileType, @FileUrl,
                        @UploadStatus, @UploadedBy, NOW(), true, NOW())";

            await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                documentUpload.DocumentId,
                documentUpload.DocumentType,
                documentUpload.OriginalFilename,
                documentUpload.FileSize,
                documentUpload.FileType,
                documentUpload.FileUrl,
                documentUpload.UploadStatus,
                documentUpload.UploadedBy
            }, Transaction);

            return id;
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateDocumentUploadDto documentUpload)
        {
            var sql = @"
                UPDATE workflowmgmt.document_uploads 
                SET upload_status = COALESCE(@UploadStatus, upload_status),
                    processed_date = COALESCE(@ProcessedDate, processed_date),
                    extracted_content = COALESCE(@ExtractedContent, extracted_content),
                    processing_notes = COALESCE(@ProcessingNotes, processing_notes),
                    modified_date = NOW()
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                documentUpload.UploadStatus,
                documentUpload.ProcessedDate,
                documentUpload.ExtractedContent,
                documentUpload.ProcessingNotes
            }, Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var sql = @"
                UPDATE workflowmgmt.document_uploads 
                SET is_active = false, modified_date = NOW()
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new { Id = id }, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var sql = "SELECT COUNT(1) FROM workflowmgmt.document_uploads WHERE id = @Id";
            var count = await Connection.ExecuteScalarAsync<int>(sql, new { Id = id }, Transaction);
            return count > 0;
        }

        public async Task<bool> ExistsByDocumentIdAsync(Guid documentId)
        {
            var sql = "SELECT COUNT(1) FROM workflowmgmt.document_uploads WHERE document_id = @DocumentId AND is_active = true";
            var count = await Connection.ExecuteScalarAsync<int>(sql, new { DocumentId = documentId }, Transaction);
            return count > 0;
        }
    }
}

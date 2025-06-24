using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models.DocumentUpload;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IDocumentUploadRepository
    {
        Task<IEnumerable<DocumentUploadDto>> GetAllAsync();
        Task<IEnumerable<DocumentUploadDto>> GetByDocumentIdAsync(Guid documentId);
        Task<IEnumerable<DocumentUploadDto>> GetByDocumentTypeAsync(string documentType);
        Task<IEnumerable<DocumentUploadDto>> GetByStatusAsync(string status);
        Task<IEnumerable<DocumentUploadDto>> GetByUploadedByAsync(Guid userId);
        Task<DocumentUploadDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(CreateDocumentUploadDto documentUpload);
        Task<bool> UpdateAsync(Guid id, UpdateDocumentUploadDto documentUpload);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByDocumentIdAsync(Guid documentId);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IDocumentFeedbackRepository
    {
        // Basic CRUD operations
        Task<List<DocumentFeedbackDto>> GetByDocumentIdAsync(Guid documentId, string documentType);
        Task<DocumentFeedbackDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(CreateDocumentFeedbackDto feedback, Guid feedbackProvider);
        Task<bool> UpdateAsync(Guid id, UpdateDocumentFeedbackDto feedback);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);

        // Query operations
        Task<List<DocumentFeedbackDto>> GetByStageIdAsync(Guid stageId);
        Task<List<DocumentFeedbackDto>> GetByProviderAsync(Guid providerId);
        Task<List<DocumentFeedbackDto>> GetUnaddressedByDocumentAsync(Guid documentId, string documentType);
        Task<List<DocumentFeedbackDto>> GetRecentByDocumentAsync(Guid documentId, string documentType, int limit = 5);

        // Address feedback
        Task<bool> MarkAsAddressedAsync(Guid id, Guid addressedBy);
        Task<bool> MarkAsUnaddressedAsync(Guid id);
    }
}

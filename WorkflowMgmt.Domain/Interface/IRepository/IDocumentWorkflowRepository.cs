using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities.Workflow;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IDocumentWorkflowRepository
    {
        Task<IEnumerable<DocumentWorkflowWithDetailsDto>> GetAllAsync();
        Task<IEnumerable<DocumentWorkflowWithDetailsDto>> GetByDocumentTypeAsync(string documentType);
        Task<IEnumerable<DocumentWorkflowWithDetailsDto>> GetByStatusAsync(string status);
        Task<IEnumerable<DocumentWorkflowWithDetailsDto>> GetByInitiatedByAsync(Guid userId);
        Task<IEnumerable<DocumentWorkflowWithDetailsDto>> GetByRoleAsync(string role);
        Task<DocumentWorkflowWithDetailsDto?> GetByIdAsync(Guid id);
        Task<DocumentWorkflowWithDetailsDto?> GetByDocumentIdAsync(string documentId);
        Task<Guid> CreateAsync(CreateDocumentWorkflowDto workflow);
        Task<bool> UpdateAsync(Guid id, UpdateDocumentWorkflowDto workflow);
        Task<bool> AdvanceStageAsync(Guid id, Guid? nextStageId, string? comments = null);
        Task<bool> CompleteWorkflowAsync(Guid id);
        Task<bool> CancelWorkflowAsync(Guid id, string? reason = null);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByDocumentIdAsync(string documentId);
        Task<bool> IsWorkflowActiveAsync(Guid id);
        Task<WorkflowStatsDto> GetWorkflowStatsAsync();
    }
}

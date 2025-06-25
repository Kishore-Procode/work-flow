using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IDocumentLifecycleRepository
    {
        // Get documents assigned to user
        Task<List<DocumentLifecycleDto>> GetDocumentsAssignedToUserAsync(Guid userId, string? documentType = null);
        
        // Get document lifecycle details
        Task<DocumentLifecycleDto?> GetDocumentLifecycleAsync(Guid documentId, string documentType, Guid userId);
        
        // Check if user can perform action on document
        Task<bool> CanUserPerformActionAsync(Guid userId, Guid documentId, string documentType, Guid actionId);
        
        // Get available actions for user on document
        Task<List<WorkflowStageActionDto>> GetAvailableActionsAsync(Guid userId, Guid documentId, string documentType);
        
        // Process document action
        Task<bool> ProcessDocumentActionAsync(ProcessDocumentActionDto actionDto, Guid processedBy);
        
        // Update document status
        Task<bool> UpdateDocumentStatusAsync(Guid documentId, string documentType, string status);
    }
}

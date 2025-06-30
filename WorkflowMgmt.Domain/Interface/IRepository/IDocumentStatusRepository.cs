using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IDocumentStatusRepository
    {
        /// <summary>
        /// Get documents based on user's history in workflow_stage_history table
        /// This includes documents where the user is either assigned_to or processed_by
        /// </summary>
        /// <param name="userId">Current user ID</param>
        /// <param name="documentType">Optional filter by document type</param>
        /// <returns>List of documents with user involvement</returns>
        Task<IEnumerable<DocumentStatusDto>> GetUserDocumentStatusAsync(Guid userId, string? documentType = null);

        /// <summary>
        /// Get detailed document information with workflow roadmap and user history
        /// </summary>
        /// <param name="documentId">Document ID</param>
        /// <param name="documentType">Document type (syllabus, lesson, session)</param>
        /// <param name="userId">Current user ID</param>
        /// <returns>Detailed document information</returns>
        Task<DocumentStatusDetailDto?> GetDocumentStatusDetailAsync(Guid documentId, string documentType, Guid userId);

        /// <summary>
        /// Get workflow roadmap for a specific workflow template
        /// </summary>
        /// <param name="workflowTemplateId">Workflow template ID</param>
        /// <returns>List of workflow stages with their details</returns>
        Task<IEnumerable<WorkflowRoadmapDto>> GetWorkflowRoadmapAsync(Guid workflowTemplateId);

        /// <summary>
        /// Get document statistics for the current user
        /// </summary>
        /// <param name="userId">Current user ID</param>
        /// <returns>Document statistics</returns>
        Task<DocumentStatusStatsDto> GetUserDocumentStatsAsync(Guid userId);

        /// <summary>
        /// Get user's involvement history for a specific document
        /// </summary>
        /// <param name="documentId">Document ID</param>
        /// <param name="documentType">Document type</param>
        /// <param name="userId">User ID</param>
        /// <returns>List of user's actions on the document</returns>
        Task<IEnumerable<DocumentUserHistoryDto>> GetUserDocumentHistoryAsync(Guid documentId, string documentType, Guid userId);
    }
}

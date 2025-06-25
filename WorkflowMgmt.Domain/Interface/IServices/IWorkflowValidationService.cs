using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Domain.Interface.IServices
{
    public interface IWorkflowValidationService
    {
        /// <summary>
        /// Validate if a user can perform a specific action on a document
        /// </summary>
        Task<WorkflowValidationResult> ValidateUserActionAsync(Guid userId, Guid documentId, 
            string documentType, Guid actionId);

        /// <summary>
        /// Validate if a workflow stage transition is valid
        /// </summary>
        Task<WorkflowValidationResult> ValidateStageTransitionAsync(Guid currentStageId, 
            Guid? nextStageId, string actionType);

        /// <summary>
        /// Validate if a document is in a valid state for workflow actions
        /// </summary>
        Task<WorkflowValidationResult> ValidateDocumentStateAsync(Guid documentId, string documentType);

        /// <summary>
        /// Validate if required approvals are met for a stage
        /// </summary>
        Task<WorkflowValidationResult> ValidateRequiredApprovalsAsync(Guid stageId, Guid documentId);

        /// <summary>
        /// Validate if a user has the required role for a workflow stage
        /// </summary>
        Task<WorkflowValidationResult> ValidateUserRoleAsync(Guid userId, Guid stageId, Guid documentId);

        /// <summary>
        /// Validate workflow template configuration
        /// </summary>
        Task<WorkflowValidationResult> ValidateWorkflowTemplateAsync(Guid templateId);

        /// <summary>
        /// Get validation rules for a specific document type
        /// </summary>
        Task<List<WorkflowValidationRule>> GetValidationRulesAsync(string documentType);
    }

    public class WorkflowValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorCode { get; set; }
        public List<string> Warnings { get; set; } = new();
        public Dictionary<string, object> AdditionalData { get; set; } = new();

        public static WorkflowValidationResult Success() => new() { IsValid = true };
        
        public static WorkflowValidationResult Failure(string errorMessage, string? errorCode = null) => 
            new() { IsValid = false, ErrorMessage = errorMessage, ErrorCode = errorCode };
        
        public static WorkflowValidationResult Warning(string warningMessage) => 
            new() { IsValid = true, Warnings = new List<string> { warningMessage } };
    }

    public class WorkflowValidationRule
    {
        public string RuleId { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RuleType { get; set; } = string.Empty; // "required", "conditional", "warning"
        public Dictionary<string, object> Parameters { get; set; } = new();
        public bool IsActive { get; set; } = true;
    }
}

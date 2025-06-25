using System;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IServices;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Services
{
    public interface IWorkflowActionProcessorService
    {
        Task<WorkflowActionResult> ProcessActionAsync(ProcessDocumentActionDto actionDto, Guid processedBy);
        Task<WorkflowActionResult> ValidateActionAsync(ProcessDocumentActionDto actionDto, Guid processedBy);
        Task<bool> CanUserPerformActionAsync(Guid userId, Guid documentId, string documentType, Guid actionId);
    }

    public class WorkflowActionProcessorService : IWorkflowActionProcessorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWorkflowValidationService _validationService;
        private readonly IWorkflowNotificationService _notificationService;

        public WorkflowActionProcessorService(
            IUnitOfWork unitOfWork,
            IWorkflowValidationService validationService,
            IWorkflowNotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _validationService = validationService;
            _notificationService = notificationService;
        }

        public async Task<WorkflowActionResult> ProcessActionAsync(ProcessDocumentActionDto actionDto, Guid processedBy)
        {
            try
            {
                // Step 1: Validate the action
                var validationResult = await ValidateActionAsync(actionDto, processedBy);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }

                // Step 2: Get action and document details for notifications
                var action = await _unitOfWork.WorkflowStageActionRepository.GetByIdAsync(actionDto.ActionId);
                var documentWorkflow = await _unitOfWork.DocumentWorkflowRepository.GetByDocumentIdAsync(actionDto.DocumentId.ToString());
                
                if (action == null || documentWorkflow == null)
                {
                    return WorkflowActionResult.Failure("Action or document workflow not found");
                }

                // Step 3: Process the action using the repository
                var success = await _unitOfWork.DocumentLifecycleRepository.ProcessDocumentActionAsync(actionDto, processedBy);
                
                if (!success)
                {
                    return WorkflowActionResult.Failure("Failed to process document action");
                }

                // Step 4: Commit the transaction
                _unitOfWork.Commit();

                // Step 5: Send notifications (after successful commit)
                await SendNotificationsAsync(actionDto, processedBy, action, documentWorkflow);

                return WorkflowActionResult.Success("Document action processed successfully");
            }
            catch (Exception ex)
            {
                return WorkflowActionResult.Failure($"Failed to process document action: {ex.Message}");
            }
        }

        public async Task<WorkflowActionResult> ValidateActionAsync(ProcessDocumentActionDto actionDto, Guid processedBy)
        {
            try
            {
                // Validate user permissions
                var userValidation = await _validationService.ValidateUserActionAsync(
                    processedBy, actionDto.DocumentId, actionDto.DocumentType, actionDto.ActionId);
                
                if (!userValidation.IsValid)
                {
                    return WorkflowActionResult.Failure(userValidation.ErrorMessage ?? "User not authorized");
                }

                // Validate document state
                var documentValidation = await _validationService.ValidateDocumentStateAsync(
                    actionDto.DocumentId, actionDto.DocumentType);
                
                if (!documentValidation.IsValid)
                {
                    return WorkflowActionResult.Failure(documentValidation.ErrorMessage ?? "Document in invalid state");
                }

                // Get action details for stage transition validation
                var action = await _unitOfWork.WorkflowStageActionRepository.GetByIdAsync(actionDto.ActionId);
                if (action == null)
                {
                    return WorkflowActionResult.Failure("Action not found");
                }

                // Validate stage transition
                var transitionValidation = await _validationService.ValidateStageTransitionAsync(
                    action.WorkflowStageId, action.NextStageId, action.ActionType);
                
                if (!transitionValidation.IsValid)
                {
                    return WorkflowActionResult.Failure(transitionValidation.ErrorMessage ?? "Invalid stage transition");
                }

                return WorkflowActionResult.Success("Action validation passed");
            }
            catch (Exception ex)
            {
                return WorkflowActionResult.Failure($"Validation failed: {ex.Message}");
            }
        }

        public async Task<bool> CanUserPerformActionAsync(Guid userId, Guid documentId, string documentType, Guid actionId)
        {
            try
            {
                var validation = await _validationService.ValidateUserActionAsync(userId, documentId, documentType, actionId);
                return validation.IsValid;
            }
            catch
            {
                return false;
            }
        }

        private async Task SendNotificationsAsync(ProcessDocumentActionDto actionDto, Guid processedBy, 
            WorkflowStageActionDto action, DocumentWorkflowWithDetailsDto documentWorkflow)
        {
            try
            {
                // Notify action completion
                await _notificationService.NotifyActionCompletedAsync(
                    actionDto.DocumentId, actionDto.DocumentType, processedBy, action.ActionName, actionDto.Comments);

                // If document is assigned to next stage, notify the assignee
                if (action.NextStageId.HasValue)
                {
                    // Get updated workflow to see new assignment
                    var updatedWorkflow = await _unitOfWork.DocumentWorkflowRepository.GetByDocumentIdAsync(actionDto.DocumentId.ToString());
                    if (updatedWorkflow?.AssignedTo.HasValue == true)
                    {
                        await _notificationService.NotifyDocumentAssignedAsync(
                            actionDto.DocumentId, actionDto.DocumentType, updatedWorkflow.AssignedTo.Value,
                            updatedWorkflow.CurrentStageName ?? "Unknown Stage", "Review Required");
                    }
                }

                // Handle specific action types
                switch (action.ActionType.ToLower())
                {
                    case "reject":
                        await _notificationService.NotifyDocumentRejectedAsync(
                            actionDto.DocumentId, actionDto.DocumentType, processedBy,
                            documentWorkflow.InitiatedBy, actionDto.Comments ?? "No reason provided");
                        break;

                    case "approve" when !action.NextStageId.HasValue:
                        // Final approval - workflow completed
                        await _notificationService.NotifyWorkflowCompletedAsync(
                            actionDto.DocumentId, actionDto.DocumentType, documentWorkflow.InitiatedBy, "published");
                        break;

                    case "escalate":
                        // Handle escalation notifications
                        if (action.NextStageId.HasValue)
                        {
                            var updatedWorkflow = await _unitOfWork.DocumentWorkflowRepository.GetByDocumentIdAsync(actionDto.DocumentId.ToString());
                            if (updatedWorkflow?.AssignedTo.HasValue == true)
                            {
                                await _notificationService.NotifyDocumentEscalatedAsync(
                                    actionDto.DocumentId, actionDto.DocumentType, processedBy,
                                    updatedWorkflow.AssignedTo.Value, actionDto.Comments ?? "Escalated for review");
                            }
                        }
                        break;
                }

                // If feedback was provided, notify relevant parties
                if (!string.IsNullOrEmpty(actionDto.Comments))
                {
                    await _notificationService.NotifyFeedbackAddedAsync(
                        actionDto.DocumentId, actionDto.DocumentType, processedBy,
                        documentWorkflow.InitiatedBy, actionDto.Comments);
                }
            }
            catch (Exception ex)
            {
                // Log notification errors but don't fail the main operation
                // In production, you'd want to log this properly
                Console.WriteLine($"Failed to send notifications: {ex.Message}");
            }
        }
    }

    public class WorkflowActionResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorCode { get; set; }
        public object? Data { get; set; }

        public static WorkflowActionResult Success(string message, object? data = null) =>
            new() { IsSuccess = true, Message = message, Data = data };

        public static WorkflowActionResult Failure(string message, string? errorCode = null) =>
            new() { IsSuccess = false, Message = message, ErrorCode = errorCode };
    }
}

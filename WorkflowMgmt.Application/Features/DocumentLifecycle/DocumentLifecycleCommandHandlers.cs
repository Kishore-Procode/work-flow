using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Workflow;
using WorkflowMgmt.Domain.Models;
using WorkflowMgmt.Application.Services;

namespace WorkflowMgmt.Application.Features.DocumentLifecycle
{
    // Query Handlers
    public class GetDocumentsAssignedToUserCommandHandler : IRequestHandler<GetDocumentsAssignedToUserCommand, ApiResponse<List<DocumentLifecycleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDocumentsAssignedToUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<DocumentLifecycleDto>>> Handle(GetDocumentsAssignedToUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var documents = await _unitOfWork.DocumentLifecycleRepository.GetDocumentsAssignedToUserAsync(request.UserId, request.DocumentType);
                return ApiResponse<List<DocumentLifecycleDto>>.SuccessResponse(documents, "Documents retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<DocumentLifecycleDto>>.ErrorResponse($"Failed to retrieve documents: {ex.Message}");
            }
        }
    }

    public class GetDocumentLifecycleByIdCommandHandler : IRequestHandler<GetDocumentLifecycleByIdCommand, ApiResponse<DocumentLifecycleDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDocumentLifecycleByIdCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<DocumentLifecycleDto?>> Handle(GetDocumentLifecycleByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var document = await _unitOfWork.DocumentLifecycleRepository.GetDocumentLifecycleAsync(request.DocumentId, request.DocumentType, request.UserId);
                return ApiResponse<DocumentLifecycleDto?>.SuccessResponse(document, "Document lifecycle retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<DocumentLifecycleDto?>.ErrorResponse($"Failed to retrieve document lifecycle: {ex.Message}");
            }
        }
    }

    public class GetAvailableActionsCommandHandler : IRequestHandler<GetAvailableActionsCommand, ApiResponse<List<WorkflowStageActionDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAvailableActionsCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<WorkflowStageActionDto>>> Handle(GetAvailableActionsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var actions = await _unitOfWork.DocumentLifecycleRepository.GetAvailableActionsAsync(request.UserId, request.DocumentId, request.DocumentType);
                return ApiResponse<List<WorkflowStageActionDto>>.SuccessResponse(actions, "Available actions retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<WorkflowStageActionDto>>.ErrorResponse($"Failed to retrieve available actions: {ex.Message}");
            }
        }
    }

    public class CanUserPerformActionCommandHandler : IRequestHandler<CanUserPerformActionCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CanUserPerformActionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(CanUserPerformActionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var canPerform = await _unitOfWork.DocumentLifecycleRepository.CanUserPerformActionAsync(request.UserId, request.DocumentId, request.DocumentType, request.ActionId);
                return ApiResponse<bool>.SuccessResponse(canPerform, "Permission check completed successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Failed to check user permissions: {ex.Message}");
            }
        }
    }

    // Command Handlers
    public class ProcessDocumentActionCommandHandler : IRequestHandler<ProcessDocumentActionCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public ProcessDocumentActionCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<bool>> Handle(ProcessDocumentActionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Validate the action
                var validationResult = await ValidateUserActionAsync(request.ProcessedBy, request.DocumentId, request.DocumentType, request.ActionId);
                if (!validationResult.IsValid)
                {
                    _unitOfWork.Rollback();
                    return ApiResponse<bool>.ErrorResponse(validationResult.ErrorMessage ?? "Validation failed");
                }

                // Step 2: Get action and document details
                var action = await _unitOfWork.WorkflowStageActionRepository.GetByIdAsync(request.ActionId);
                var documentWorkflow = await _unitOfWork.DocumentWorkflowRepository.GetByDocumentIdAsync(request.DocumentId.ToString());

                if (action == null || documentWorkflow == null)
                {
                    _unitOfWork.Rollback();
                    return ApiResponse<bool>.ErrorResponse("Action or document workflow not found");
                }

                // Step 3: Create the action DTO
                var actionDto = new ProcessDocumentActionDto
                {
                    DocumentId = request.DocumentId,
                    DocumentType = request.DocumentType,
                    ActionId = request.ActionId,
                    Comments = request.Comments,
                    FeedbackType = request.FeedbackType
                };
                // Step 4: Process the action using the repository
                var success = await _unitOfWork.DocumentLifecycleRepository.ProcessDocumentActionAsync(actionDto, request.ProcessedBy);

                if (!success)
                {
                    _unitOfWork.Rollback();
                    return ApiResponse<bool>.ErrorResponse("Failed to process document action");
                }

                // Step 5: Send notifications after successful commit
                await SendWorkflowNotificationsAsync(request, action, documentWorkflow);

                // Step 6: Commit the transaction
                _unitOfWork.Commit();

                return ApiResponse<bool>.SuccessResponse(true, "Document action processed successfully");
            }
            catch (Exception ex)
            {
                try
                {
                    _unitOfWork.Rollback();
                }
                catch
                {
                    // Ignore rollback errors
                }
                return ApiResponse<bool>.ErrorResponse($"Failed to process document action: {ex.Message}");
            }
        }

        private async Task SendWorkflowNotificationsAsync(ProcessDocumentActionCommand request, dynamic action, dynamic documentWorkflow)
        {
            try
            {
                // Get document details for notification
                var documentTitle = await GetDocumentTitleAsync(request.DocumentId, request.DocumentType);

                // Get current workflow details after action processing
                var updatedWorkflow = await _unitOfWork.DocumentWorkflowRepository.GetByDocumentIdAsync(request.DocumentId.ToString());
                if (updatedWorkflow == null) return;

                var recipientUserIds = new List<Guid>();

                // Add assigned user (new assignee after action)
                if (updatedWorkflow.AssignedTo != null && updatedWorkflow.AssignedTo != request.ProcessedBy)
                {
                    recipientUserIds.Add(updatedWorkflow.AssignedTo.Value);

                    // Send assignment notification to new assignee
                    var currentStage = await GetCurrentStageNameAsync(updatedWorkflow.CurrentStageId);
                    await _notificationService.SendDocumentAssignedNotificationAsync(
                        request.DocumentId,
                        request.DocumentType,
                        documentTitle,
                        currentStage,
                        request.ProcessedBy,
                        updatedWorkflow.AssignedTo.Value
                    );
                }

                // Add document initiator (creator)
                if (updatedWorkflow.InitiatedBy != request.ProcessedBy)
                {
                    recipientUserIds.Add(updatedWorkflow.InitiatedBy);
                }

                // Remove duplicates
                recipientUserIds = recipientUserIds.Distinct().ToList();

                // Send action completion notification to relevant users
                if (recipientUserIds.Any())
                {
                    var actionName = action?.action_name ?? "Unknown Action";

                    // Determine notification type based on action
                    if (actionName.ToLower().Contains("approve"))
                    {
                        foreach (var userId in recipientUserIds)
                        {
                            await _notificationService.SendDocumentApprovedNotificationAsync(
                                request.DocumentId,
                                request.DocumentType,
                                documentTitle,
                                request.ProcessedBy,
                                userId
                            );
                        }
                    }
                    else if (actionName.ToLower().Contains("reject"))
                    {
                        foreach (var userId in recipientUserIds)
                        {
                            await _notificationService.SendDocumentRejectedNotificationAsync(
                                request.DocumentId,
                                request.DocumentType,
                                documentTitle,
                                request.Comments ?? "No reason provided",
                                request.ProcessedBy,
                                userId
                            );
                        }
                    }
                    else
                    {
                        // General status update notification
                        var previousStatus = documentWorkflow?.Status ?? "Unknown";
                        var newStatus = updatedWorkflow.Status ?? "Unknown";

                        await _notificationService.SendDocumentStatusUpdatedNotificationAsync(
                            request.DocumentId,
                            request.DocumentType,
                            documentTitle,
                            previousStatus,
                            newStatus,
                            request.ProcessedBy,
                            recipientUserIds
                        );
                    }
                }

                // Send feedback notification if comments were provided
                if (!string.IsNullOrEmpty(request.Comments) && updatedWorkflow.InitiatedBy != request.ProcessedBy)
                {
                    await _notificationService.SendFeedbackReceivedNotificationAsync(
                        request.DocumentId,
                        request.DocumentType,
                        documentTitle,
                        request.FeedbackType ?? "general",
                        request.ProcessedBy,
                        updatedWorkflow.InitiatedBy
                    );
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the main operation
                System.Diagnostics.Debug.WriteLine($"Error sending workflow notifications: {ex.Message}");
            }
        }

        private async Task<string> GetDocumentTitleAsync(Guid documentId, string documentType)
        {
            try
            {
                switch (documentType.ToLower())
                {
                    case "syllabus":
                        var syllabus = await _unitOfWork.SyllabusRepository.GetByIdAsync(documentId);
                        return syllabus?.Title ?? "Untitled Syllabus";
                    case "lesson":
                        var lesson = await _unitOfWork.LessonPlanRepository.GetByIdAsync(documentId);
                        return lesson?.Title ?? "Untitled Lesson Plan";
                    case "session":
                        var session = await _unitOfWork.SessionRepository.GetByIdAsync(documentId);
                        return session?.Title ?? "Untitled Session";
                    default:
                        return "Untitled Document";
                }
            }
            catch
            {
                return "Untitled Document";
            }
        }

        private async Task<string> GetCurrentStageNameAsync(Guid? stageId)
        {
            try
            {
                if (!stageId.HasValue) return "Unknown Stage";

                var stage = await _unitOfWork.WorkflowStageRepository.GetByIdAsync(stageId.Value);
                return stage?.StageName ?? "Unknown Stage";
            }
            catch
            {
                return "Unknown Stage";
            }
        }

        private async Task<ValidationResult> ValidateUserActionAsync(Guid userId, Guid documentId, string documentType, Guid actionId)
        {
            try
            {
                // Check if user can perform this action using existing repository method
                var canPerform = await _unitOfWork.DocumentLifecycleRepository.CanUserPerformActionAsync(
                    userId, documentId, documentType, actionId);

                if (!canPerform)
                {
                    return ValidationResult.Failure("User is not authorized to perform this action");
                }

                // Validate document state
                var workflow = await _unitOfWork.DocumentWorkflowRepository.GetByDocumentIdAsync(documentId.ToString());
                if (workflow == null)
                {
                    return ValidationResult.Failure("Document workflow not found");
                }

                // Check if workflow is in a valid state for actions
                if (workflow.Status == "Completed")
                {
                    return ValidationResult.Failure("Cannot perform actions on completed workflow");
                }

                if (workflow.Status == "Cancelled")
                {
                    return ValidationResult.Failure("Cannot perform actions on cancelled workflow");
                }

                // Get action details for stage transition validation
                var action = await _unitOfWork.WorkflowStageActionRepository.GetByIdAsync(actionId);
                if (action == null)
                {
                    return ValidationResult.Failure("Action not found");
                }

                // Validate stage transition
                if (action.WorkflowStageId != workflow.CurrentStageId)
                {
                    return ValidationResult.Failure("Action does not belong to the current workflow stage");
                }

                return ValidationResult.Success();
            }
            catch (Exception ex)
            {
                return ValidationResult.Failure($"Validation error: {ex.Message}");
            }
        }


    }

    // Simple validation result class
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }

        public static ValidationResult Success() => new() { IsValid = true };
        public static ValidationResult Failure(string errorMessage) => new() { IsValid = false, ErrorMessage = errorMessage };
    }

    public class CreateDocumentFeedbackCommandHandler : IRequestHandler<CreateDocumentFeedbackCommand, ApiResponse<DocumentFeedbackDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public CreateDocumentFeedbackCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<DocumentFeedbackDto>> Handle(CreateDocumentFeedbackCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var createDto = new CreateDocumentFeedbackDto
                {
                    DocumentId = request.DocumentId,
                    DocumentType = request.DocumentType,
                    WorkflowStageId = request.WorkflowStageId,
                    FeedbackText = request.FeedbackText,
                    FeedbackType = request.FeedbackType
                };

                var feedbackId = await _unitOfWork.DocumentFeedbackRepository.CreateAsync(createDto, request.FeedbackProvider);

                var result = await _unitOfWork.DocumentFeedbackRepository.GetByIdAsync(feedbackId);

                // Send notifications to all users related to the document from history table except current user
                await SendFeedbackNotificationsAsync(request.DocumentId, request.DocumentType, request.FeedbackProvider, request.FeedbackType);
                _unitOfWork.Commit();

                return ApiResponse<DocumentFeedbackDto>.SuccessResponse(result!, "Feedback created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<DocumentFeedbackDto>.ErrorResponse($"Failed to create feedback: {ex.Message}");
            }
        }

        private async Task SendFeedbackNotificationsAsync(Guid documentId, string documentType, Guid feedbackProvider, string feedbackType)
        {
            try
            {
                // Get document title
                var documentTitle = await GetDocumentTitleAsync(documentId, documentType);

                // Get all users related to the document from workflow history except current user
                var relatedUserIds = await GetDocumentRelatedUsersAsync(documentId, documentType, feedbackProvider);

                if (relatedUserIds.Any())
                {
                    // Send feedback notification to each related user
                    foreach (var userId in relatedUserIds)
                    {
                        await _notificationService.SendFeedbackReceivedNotificationAsync(
                            documentId,
                            documentType,
                            documentTitle,
                            feedbackType,
                            feedbackProvider,
                            userId
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception but don't fail the feedback creation
                // You can add logging here if needed
                Console.WriteLine($"Failed to send feedback notifications: {ex.Message}");
            }
        }

        private async Task<string> GetDocumentTitleAsync(Guid documentId, string documentType)
        {
            try
            {
                return documentType.ToLower() switch
                {
                    "syllabus" => (await _unitOfWork.SyllabusRepository.GetByIdAsync(documentId))?.Title ?? "Unknown Document",
                    "lesson" => (await _unitOfWork.LessonPlanRepository.GetByIdAsync(documentId))?.Title ?? "Unknown Document",
                    "session" => (await _unitOfWork.SessionRepository.GetByIdAsync(documentId))?.Title ?? "Unknown Document",
                    _ => "Unknown Document"
                };
            }
            catch
            {
                return "Unknown Document";
            }
        }

        private async Task<List<Guid>> GetDocumentRelatedUsersAsync(Guid documentId, string documentType, Guid excludeUserId)
        {
            try
            {
                // Get document workflow using DocumentWorkflowRepository
                var documentWorkflow = await _unitOfWork.DocumentWorkflowRepository.GetByDocumentIdAsync(documentId.ToString());
                if (documentWorkflow == null) return [];

                // Get all users from workflow stage history who have interacted with this document
                var workflowHistory = await _unitOfWork.WorkflowStageHistoryRepository.GetByDocumentWorkflowIdAsync(documentWorkflow.Id);

                var relatedUsers = new List<Guid>();

                // Add users who processed or were assigned to stages (excluding current user)
                foreach (var history in workflowHistory)
                {
                    if (history.ProcessedBy != excludeUserId)
                    {
                        relatedUsers.Add(history.ProcessedBy);
                    }
                }

                // Add document initiator if not already included and not the current user
                if (documentWorkflow.InitiatedBy != excludeUserId && !relatedUsers.Contains(documentWorkflow.InitiatedBy))
                {
                    relatedUsers.Add(documentWorkflow.InitiatedBy);
                }

                // Add currently assigned user if different from current user
                if (documentWorkflow.AssignedTo.HasValue && documentWorkflow.AssignedTo.Value != excludeUserId && !relatedUsers.Contains(documentWorkflow.AssignedTo.Value))
                {
                    relatedUsers.Add(documentWorkflow.AssignedTo.Value);
                }

                return relatedUsers.Distinct().ToList();
            }
            catch(Exception ex)
            {
                return [];
            }
        }
    }

    public class UpdateDocumentFeedbackCommandHandler : IRequestHandler<UpdateDocumentFeedbackCommand, ApiResponse<DocumentFeedbackDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDocumentFeedbackCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<DocumentFeedbackDto?>> Handle(UpdateDocumentFeedbackCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var updateDto = new UpdateDocumentFeedbackDto
                {
                    FeedbackText = request.FeedbackText,
                    FeedbackType = request.FeedbackType,
                    IsAddressed = request.IsAddressed
                };

                var success = await _unitOfWork.DocumentFeedbackRepository.UpdateAsync(request.Id, updateDto);
                if (!success)
                {
                    return ApiResponse<DocumentFeedbackDto?>.ErrorResponse("Feedback not found or update failed");
                }

                _unitOfWork.Commit();

                var result = await _unitOfWork.DocumentFeedbackRepository.GetByIdAsync(request.Id);
                return ApiResponse<DocumentFeedbackDto?>.SuccessResponse(result, "Feedback updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<DocumentFeedbackDto?>.ErrorResponse($"Failed to update feedback: {ex.Message}");
            }
        }
    }

    public class GetDocumentFeedbackCommandHandler : IRequestHandler<GetDocumentFeedbackCommand, ApiResponse<List<DocumentFeedbackDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDocumentFeedbackCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<DocumentFeedbackDto>>> Handle(GetDocumentFeedbackCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var feedback = await _unitOfWork.DocumentFeedbackRepository.GetByDocumentIdAsync(request.DocumentId, request.DocumentType);
                return ApiResponse<List<DocumentFeedbackDto>>.SuccessResponse(feedback, "Feedback retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<DocumentFeedbackDto>>.ErrorResponse($"Failed to retrieve feedback: {ex.Message}");
            }
        }
    }

    public class MarkFeedbackAsAddressedCommandHandler : IRequestHandler<MarkFeedbackAsAddressedCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public MarkFeedbackAsAddressedCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(MarkFeedbackAsAddressedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _unitOfWork.DocumentFeedbackRepository.MarkAsAddressedAsync(request.FeedbackId, request.AddressedBy);
                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Feedback not found or update failed");
                }

                _unitOfWork.Commit();
                return ApiResponse<bool>.SuccessResponse(true, "Feedback marked as addressed successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Failed to mark feedback as addressed: {ex.Message}");
            }
        }
    }
}

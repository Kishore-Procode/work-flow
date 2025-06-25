using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Workflow;
using WorkflowMgmt.Application.Services;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.DocumentLifecycle
{
    // Query Handlers
    public class GetDocumentsAssignedToUserQueryHandler : IRequestHandler<GetDocumentsAssignedToUserQuery, ApiResponse<List<DocumentLifecycleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDocumentsAssignedToUserQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<DocumentLifecycleDto>>> Handle(GetDocumentsAssignedToUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var documents = await _unitOfWork.DocumentLifecycleRepository.GetDocumentsAssignedToUserAsync(request.UserId, request.DocumentType);
                return ApiResponse<List<DocumentLifecycleDto>>.SuccessResponse(documents.ToList(), "Documents retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<DocumentLifecycleDto>>.ErrorResponse($"Failed to retrieve documents: {ex.Message}");
            }
        }
    }

    public class GetDocumentLifecycleQueryHandler : IRequestHandler<GetDocumentLifecycleQuery, ApiResponse<DocumentLifecycleDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDocumentLifecycleQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<DocumentLifecycleDto?>> Handle(GetDocumentLifecycleQuery request, CancellationToken cancellationToken)
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

    public class GetAvailableActionsQueryHandler : IRequestHandler<GetAvailableActionsQuery, ApiResponse<List<WorkflowStageActionDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAvailableActionsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<WorkflowStageActionDto>>> Handle(GetAvailableActionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var actions = await _unitOfWork.DocumentLifecycleRepository.GetAvailableActionsAsync(request.UserId, request.DocumentId, request.DocumentType);
                return ApiResponse<List<WorkflowStageActionDto>>.SuccessResponse(actions.ToList(), "Available actions retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<WorkflowStageActionDto>>.ErrorResponse($"Failed to retrieve available actions: {ex.Message}");
            }
        }
    }

    public class CanUserPerformActionQueryHandler : IRequestHandler<CanUserPerformActionQuery, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CanUserPerformActionQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(CanUserPerformActionQuery request, CancellationToken cancellationToken)
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
        private readonly IWorkflowActionProcessorService _actionProcessorService;

        public ProcessDocumentActionCommandHandler(IWorkflowActionProcessorService actionProcessorService)
        {
            _actionProcessorService = actionProcessorService;
        }

        public async Task<ApiResponse<bool>> Handle(ProcessDocumentActionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Create the action DTO
                var actionDto = new ProcessDocumentActionDto
                {
                    DocumentId = request.DocumentId,
                    DocumentType = request.DocumentType,
                    ActionId = request.ActionId,
                    Comments = request.Comments,
                    FeedbackType = request.FeedbackType
                };

                // Process the action using the enhanced service
                var result = await _actionProcessorService.ProcessActionAsync(actionDto, request.ProcessedBy);

                if (!result.IsSuccess)
                {
                    return ApiResponse<bool>.ErrorResponse(result.Message);
                }

                return ApiResponse<bool>.SuccessResponse(true, result.Message);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Failed to process document action: {ex.Message}");
            }
        }


    }

    public class CreateDocumentFeedbackCommandHandler : IRequestHandler<CreateDocumentFeedbackCommand, ApiResponse<DocumentFeedbackDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDocumentFeedbackCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                _unitOfWork.Commit();

                var result = await _unitOfWork.DocumentFeedbackRepository.GetByIdAsync(feedbackId);
                return ApiResponse<DocumentFeedbackDto>.SuccessResponse(result!, "Feedback created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<DocumentFeedbackDto>.ErrorResponse($"Failed to create feedback: {ex.Message}");
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

    public class GetDocumentFeedbackQueryHandler : IRequestHandler<GetDocumentFeedbackQuery, ApiResponse<List<DocumentFeedbackDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDocumentFeedbackQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<DocumentFeedbackDto>>> Handle(GetDocumentFeedbackQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var feedback = await _unitOfWork.DocumentFeedbackRepository.GetByDocumentIdAsync(request.DocumentId, request.DocumentType);
                return ApiResponse<List<DocumentFeedbackDto>>.SuccessResponse(feedback.ToList(), "Feedback retrieved successfully");
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

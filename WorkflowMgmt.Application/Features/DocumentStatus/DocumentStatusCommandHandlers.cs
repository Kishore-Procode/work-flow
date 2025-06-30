using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Workflow;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.DocumentStatus
{
    // Query Handlers
    public class GetUserDocumentStatusCommandHandler : IRequestHandler<GetUserDocumentStatusCommand, ApiResponse<List<DocumentStatusDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserDocumentStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<DocumentStatusDto>>> Handle(GetUserDocumentStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var documents = await _unitOfWork.DocumentStatusRepository.GetUserDocumentStatusAsync(request.UserId, request.DocumentType);
                return ApiResponse<List<DocumentStatusDto>>.SuccessResponse(documents.ToList(), "Documents retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<DocumentStatusDto>>.ErrorResponse($"Error retrieving documents: {ex.Message}");
            }
        }
    }

    public class GetDocumentStatusDetailCommandHandler : IRequestHandler<GetDocumentStatusDetailCommand, ApiResponse<DocumentStatusDetailDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDocumentStatusDetailCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<DocumentStatusDetailDto?>> Handle(GetDocumentStatusDetailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var document = await _unitOfWork.DocumentStatusRepository.GetDocumentStatusDetailAsync(request.DocumentId, request.DocumentType, request.UserId);
                return ApiResponse<DocumentStatusDetailDto?>.SuccessResponse(document, "Document detail retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<DocumentStatusDetailDto?>.ErrorResponse($"Error retrieving document detail: {ex.Message}");
            }
        }
    }

    public class GetWorkflowRoadmapCommandHandler : IRequestHandler<GetWorkflowRoadmapCommand, ApiResponse<List<WorkflowRoadmapDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowRoadmapCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<WorkflowRoadmapDto>>> Handle(GetWorkflowRoadmapCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var roadmap = await _unitOfWork.DocumentStatusRepository.GetWorkflowRoadmapAsync(request.WorkflowTemplateId);
                return ApiResponse<List<WorkflowRoadmapDto>>.SuccessResponse(roadmap.ToList(), "Workflow roadmap retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<WorkflowRoadmapDto>>.ErrorResponse($"Error retrieving workflow roadmap: {ex.Message}");
            }
        }
    }

    public class GetUserDocumentStatsCommandHandler : IRequestHandler<GetUserDocumentStatsCommand, ApiResponse<DocumentStatusStatsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserDocumentStatsCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<DocumentStatusStatsDto>> Handle(GetUserDocumentStatsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var stats = await _unitOfWork.DocumentStatusRepository.GetUserDocumentStatsAsync(request.UserId);
                return ApiResponse<DocumentStatusStatsDto>.SuccessResponse(stats, "Document statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<DocumentStatusStatsDto>.ErrorResponse($"Error retrieving document statistics: {ex.Message}");
            }
        }
    }
}

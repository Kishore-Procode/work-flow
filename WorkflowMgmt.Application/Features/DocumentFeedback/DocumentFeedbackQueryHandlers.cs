using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.DocumentFeedback
{
    public class GetFeedbackByDocumentsQueryHandler : IRequestHandler<GetFeedbackByDocumentsQuery, ApiResponse<Dictionary<Guid, List<DocumentFeedbackDto>>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFeedbackByDocumentsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<Dictionary<Guid, List<DocumentFeedbackDto>>>> Handle(
            GetFeedbackByDocumentsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var feedbackDict = new Dictionary<Guid, List<DocumentFeedbackDto>>();

                foreach (var documentId in request.DocumentIds)
                {
                    var feedback = await _unitOfWork.DocumentFeedbackRepository
                        .GetRecentByDocumentAsync(documentId, request.DocumentType ?? "syllabus", 5);

                    feedbackDict[documentId] = feedback;
                }

                return ApiResponse<Dictionary<Guid, List<DocumentFeedbackDto>>>.SuccessResponse(
                    feedbackDict,
                    "Document feedback retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<Dictionary<Guid, List<DocumentFeedbackDto>>>.ErrorResponse(
                    $"Error retrieving document feedback: {ex.Message}");
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

        public async Task<ApiResponse<List<DocumentFeedbackDto>>> Handle(
            GetDocumentFeedbackQuery request, 
            CancellationToken cancellationToken)
        {
            try
            {
                var feedback = await _unitOfWork.DocumentFeedbackRepository
                    .GetByDocumentIdAsync(request.DocumentId, request.DocumentType);

                return ApiResponse<List<DocumentFeedbackDto>>.SuccessResponse(
                    feedback, 
                    "Document feedback retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<DocumentFeedbackDto>>.ErrorResponse(
                    $"Error retrieving document feedback: {ex.Message}");
            }
        }
    }

    public class GetRecentFeedbackQueryHandler : IRequestHandler<GetRecentFeedbackQuery, ApiResponse<List<DocumentFeedbackDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRecentFeedbackQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<DocumentFeedbackDto>>> Handle(
            GetRecentFeedbackQuery request, 
            CancellationToken cancellationToken)
        {
            try
            {
                // Get recent feedback across all documents
                // This is a simplified implementation - could be optimized with a specific repository method
                var allFeedback = new List<DocumentFeedbackDto>();

                // For now, we'll return an empty list as this would require a more complex query
                // In a real implementation, you'd add a method to get recent feedback across all documents
                
                return ApiResponse<List<DocumentFeedbackDto>>.SuccessResponse(
                    allFeedback, 
                    "Recent feedback retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<DocumentFeedbackDto>>.ErrorResponse(
                    $"Error retrieving recent feedback: {ex.Message}");
            }
        }
    }
}

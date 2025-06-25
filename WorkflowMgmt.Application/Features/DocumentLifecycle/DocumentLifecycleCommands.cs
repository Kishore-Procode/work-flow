using System;
using System.Collections.Generic;
using MediatR;
using WorkflowMgmt.Domain.Models;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.DocumentLifecycle
{
    // Queries
    public record GetDocumentsAssignedToUserCommand(Guid UserId, string? DocumentType = null) : IRequest<ApiResponse<List<DocumentLifecycleDto>>>;

    public record GetDocumentLifecycleByIdCommand(Guid DocumentId, string DocumentType, Guid UserId) : IRequest<ApiResponse<DocumentLifecycleDto?>>;

    public record GetAvailableActionsCommand(Guid UserId, Guid DocumentId, string DocumentType) : IRequest<ApiResponse<List<WorkflowStageActionDto>>>;

    public record CanUserPerformActionCommand(Guid UserId, Guid DocumentId, string DocumentType, Guid ActionId) : IRequest<ApiResponse<bool>>;

    // Commands
    public record ProcessDocumentActionCommand(Guid DocumentId, string DocumentType, Guid ActionId, Guid ProcessedBy, string? Comments = null, string FeedbackType = "general") : IRequest<ApiResponse<bool>>;

    public record CreateDocumentFeedbackCommand(Guid DocumentId, string DocumentType, Guid FeedbackProvider, string FeedbackText, Guid? WorkflowStageId = null, string FeedbackType = "general") : IRequest<ApiResponse<DocumentFeedbackDto>>;

    public record UpdateDocumentFeedbackCommand(Guid Id, string? FeedbackText = null, string? FeedbackType = null, bool? IsAddressed = null) : IRequest<ApiResponse<DocumentFeedbackDto?>>;

    public record GetDocumentFeedbackCommand(Guid DocumentId, string DocumentType) : IRequest<ApiResponse<List<DocumentFeedbackDto>>>;

    public record MarkFeedbackAsAddressedCommand(Guid FeedbackId, Guid AddressedBy) : IRequest<ApiResponse<bool>>;
}

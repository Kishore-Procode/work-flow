using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using WorkflowMgmt.Domain.Models;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.DocumentLifecycle
{
    // Queries
    public class GetDocumentsAssignedToUserQuery : IRequest<ApiResponse<List<DocumentLifecycleDto>>>
    {
        [Required]
        public Guid UserId { get; set; }
        
        public string? DocumentType { get; set; }
    }

    public class GetDocumentLifecycleQuery : IRequest<ApiResponse<DocumentLifecycleDto?>>
    {
        [Required]
        public Guid DocumentId { get; set; }
        
        [Required]
        public string DocumentType { get; set; } = string.Empty;
        
        [Required]
        public Guid UserId { get; set; }
    }

    public class GetAvailableActionsQuery : IRequest<ApiResponse<List<WorkflowStageActionDto>>>
    {
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public Guid DocumentId { get; set; }
        
        [Required]
        public string DocumentType { get; set; } = string.Empty;
    }

    public class CanUserPerformActionQuery : IRequest<ApiResponse<bool>>
    {
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public Guid DocumentId { get; set; }
        
        [Required]
        public string DocumentType { get; set; } = string.Empty;
        
        [Required]
        public Guid ActionId { get; set; }
    }

    // Commands
    public class ProcessDocumentActionCommand : IRequest<ApiResponse<bool>>
    {
        [Required]
        public Guid DocumentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        public Guid ActionId { get; set; }

        [Required]
        public Guid ProcessedBy { get; set; }

        public string? Comments { get; set; }

        public string? FeedbackType { get; set; } = "general";
    }

    public class CreateDocumentFeedbackCommand : IRequest<ApiResponse<DocumentFeedbackDto>>
    {
        [Required]
        public Guid DocumentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        public Guid? WorkflowStageId { get; set; }

        [Required]
        public Guid FeedbackProvider { get; set; }

        [Required]
        public string FeedbackText { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FeedbackType { get; set; } = "general";
    }

    public class UpdateDocumentFeedbackCommand : IRequest<ApiResponse<DocumentFeedbackDto?>>
    {
        [Required]
        public Guid Id { get; set; }

        public string? FeedbackText { get; set; }
        public string? FeedbackType { get; set; }
        public bool? IsAddressed { get; set; }
    }

    public class GetDocumentFeedbackQuery : IRequest<ApiResponse<List<DocumentFeedbackDto>>>
    {
        [Required]
        public Guid DocumentId { get; set; }
        
        [Required]
        public string DocumentType { get; set; } = string.Empty;
    }

    public class MarkFeedbackAsAddressedCommand : IRequest<ApiResponse<bool>>
    {
        [Required]
        public Guid FeedbackId { get; set; }
        
        [Required]
        public Guid AddressedBy { get; set; }
    }
}

using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.DocumentWorkflow
{
    // Queries
    public class GetAllDocumentWorkflowsQuery : IRequest<IEnumerable<DocumentWorkflowWithDetailsDto>>
    {
    }

    public class GetDocumentWorkflowsByDocumentTypeQuery : IRequest<IEnumerable<DocumentWorkflowWithDetailsDto>>
    {
        public string DocumentType { get; set; } = string.Empty;
    }

    public class GetDocumentWorkflowsByStatusQuery : IRequest<IEnumerable<DocumentWorkflowWithDetailsDto>>
    {
        public string Status { get; set; } = string.Empty;
    }

    public class GetDocumentWorkflowsByInitiatedByQuery : IRequest<IEnumerable<DocumentWorkflowWithDetailsDto>>
    {
        public Guid UserId { get; set; }
    }

    public class GetDocumentWorkflowsByRoleQuery : IRequest<IEnumerable<DocumentWorkflowWithDetailsDto>>
    {
        public string Role { get; set; } = string.Empty;
    }

    public class GetDocumentWorkflowByIdQuery : IRequest<DocumentWorkflowWithDetailsDto?>
    {
        public Guid Id { get; set; }
    }

    public class GetDocumentWorkflowByDocumentIdQuery : IRequest<DocumentWorkflowWithDetailsDto?>
    {
        public string DocumentId { get; set; } = string.Empty;
    }

    public class GetWorkflowStatsQuery : IRequest<WorkflowStatsDto>
    {
    }

    // Commands
    public class CreateDocumentWorkflowCommand : IRequest<DocumentWorkflowDto>
    {
        [Required]
        [MaxLength(255)]
        public string DocumentId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        public Guid WorkflowTemplateId { get; set; }

        [Required]
        public Guid InitiatedBy { get; set; }

        public Guid? AssignedTo { get; set; }
    }

    public class UpdateDocumentWorkflowCommand : IRequest<DocumentWorkflowDto?>
    {
        public Guid Id { get; set; }

        public Guid? CurrentStageId { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        public DateTime? CompletedDate { get; set; }
    }

    public class AdvanceDocumentWorkflowCommand : IRequest<DocumentWorkflowDto?>
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string ActionType { get; set; } = string.Empty;

        public string? Comments { get; set; }
    }

    public class CompleteDocumentWorkflowCommand : IRequest<DocumentWorkflowDto?>
    {
        public Guid Id { get; set; }
    }

    public class CancelDocumentWorkflowCommand : IRequest<DocumentWorkflowDto?>
    {
        public Guid Id { get; set; }

        public string? Reason { get; set; }
    }
}

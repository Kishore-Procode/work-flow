using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.WorkflowAction
{
    public class ProcessWorkflowActionCommand : IRequest<DocumentWorkflowDto?>
    {
        [Required]
        public Guid DocumentWorkflowId { get; set; }

        [Required]
        public Guid ActionId { get; set; }

        [Required]
        public Guid ProcessedBy { get; set; }

        public string? Comments { get; set; }

        public string[]? Attachments { get; set; }
    }

    public class GetWorkflowActionsQuery : IRequest<WorkflowStageActionDto[]>
    {
        [Required]
        public Guid DocumentWorkflowId { get; set; }
    }
}

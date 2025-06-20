using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.WorkflowStageAction
{
    // Queries
    public class GetWorkflowStageActionsByStageIdQuery : IRequest<IEnumerable<WorkflowStageActionDto>>
    {
        public Guid StageId { get; set; }
    }

    public class GetWorkflowStageActionByIdQuery : IRequest<WorkflowStageActionDto?>
    {
        public Guid Id { get; set; }
    }

    // Commands
    public class CreateWorkflowStageActionCommand : IRequest<WorkflowStageActionDto>
    {
        public Guid StageId { get; set; }

        [Required]
        [MaxLength(255)]
        public string ActionName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ActionType { get; set; } = string.Empty;

        public Guid? NextStageId { get; set; }
    }

    public class UpdateWorkflowStageActionCommand : IRequest<WorkflowStageActionDto?>
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string ActionName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ActionType { get; set; } = string.Empty;

        public Guid? NextStageId { get; set; }
    }

    public class DeleteWorkflowStageActionCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}

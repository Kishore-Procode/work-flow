using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.WorkflowStage
{
    // Queries
    public class GetWorkflowStagesByTemplateIdQuery : IRequest<IEnumerable<WorkflowStageDto>>
    {
        public Guid TemplateId { get; set; }
    }

    public class GetWorkflowStageByIdQuery : IRequest<WorkflowStageDto?>
    {
        public Guid Id { get; set; }
    }

    public class GetWorkflowStagesByRoleQuery : IRequest<IEnumerable<WorkflowStageDto>>
    {
        public string Role { get; set; } = string.Empty;
    }

    // Commands
    public class CreateWorkflowStageCommand : IRequest<WorkflowStageDto>
    {
        public Guid TemplateId { get; set; }

        [Required]
        [MaxLength(255)]
        public string StageName { get; set; } = string.Empty;

        [Required]
        public int StageOrder { get; set; }

        [Required]
        [MaxLength(100)]
        public string AssignedRole { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsRequired { get; set; } = true;

        public bool AutoApprove { get; set; } = false;

        public int? TimeoutDays { get; set; }

        public List<CreateWorkflowStageActionDto> Actions { get; set; } = new();
    }

    public class UpdateWorkflowStageCommand : IRequest<WorkflowStageDto?>
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string StageName { get; set; } = string.Empty;

        [Required]
        public int StageOrder { get; set; }

        [Required]
        [MaxLength(100)]
        public string AssignedRole { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsRequired { get; set; } = true;

        public bool AutoApprove { get; set; } = false;

        public int? TimeoutDays { get; set; }
    }

    public class DeleteWorkflowStageCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class ReorderWorkflowStagesCommand : IRequest<bool>
    {
        public Guid TemplateId { get; set; }
        public List<StageOrderDto> StageOrders { get; set; } = new();
    }

    public class StageOrderDto
    {
        public Guid StageId { get; set; }
        public int NewOrder { get; set; }
    }
}

using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.WorkflowTemplate
{
    // Queries
    public class GetAllWorkflowTemplatesQuery : IRequest<IEnumerable<WorkflowTemplateDto>>
    {
    }

    public class GetWorkflowTemplatesByDocumentTypeQuery : IRequest<IEnumerable<WorkflowTemplateDto>>
    {
        public string DocumentType { get; set; } = string.Empty;
    }

    public class GetActiveWorkflowTemplatesQuery : IRequest<IEnumerable<WorkflowTemplateDto>>
    {
    }

    public class GetWorkflowTemplateByIdQuery : IRequest<WorkflowTemplateWithStagesDto?>
    {
        public Guid Id { get; set; }
    }

    // Commands
    public class CreateWorkflowTemplateCommand : IRequest<WorkflowTemplateDto>
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        public List<CreateWorkflowStageDto> Stages { get; set; } = new();
    }

    public class UpdateWorkflowTemplateCommand : IRequest<WorkflowTemplateDto?>
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;
    }

    public class DeleteWorkflowTemplateCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class ToggleWorkflowTemplateActiveCommand : IRequest<WorkflowTemplateDto?>
    {
        public Guid Id { get; set; }
    }
}

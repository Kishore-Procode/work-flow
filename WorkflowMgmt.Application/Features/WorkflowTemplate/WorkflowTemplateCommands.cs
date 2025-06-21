using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkflowMgmt.Domain.Models.Workflow;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.WorkflowTemplate
{
    // Queries
    public class GetAllWorkflowTemplatesQuery : IRequest<ApiResponse<List<WorkflowTemplateWithStagesDto>>>
    {
    }

    public class GetWorkflowTemplatesByDocumentTypeQuery : IRequest<ApiResponse<List<WorkflowTemplateDto>>>
    {
        public string DocumentType { get; set; } = string.Empty;
    }

    public class GetActiveWorkflowTemplatesQuery : IRequest<ApiResponse<List<WorkflowTemplateDto>>>
    {
    }

    public class GetWorkflowTemplateByIdQuery : IRequest<ApiResponse<WorkflowTemplateWithStagesDto?>>
    {
        public Guid Id { get; set; }
    }

    // Commands
    public class CreateWorkflowTemplateCommand : IRequest<ApiResponse<WorkflowTemplateDto>>
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

    public class UpdateWorkflowTemplateCommand : IRequest<ApiResponse<WorkflowTemplateDto?>>
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        public List<CreateWorkflowStageDto> Stages { get; set; } = new();
    }

    public class DeleteWorkflowTemplateCommand : IRequest<ApiResponse<bool>>
    {
        public Guid Id { get; set; }
    }

    public class ToggleWorkflowTemplateActiveCommand : IRequest<ApiResponse<WorkflowTemplateDto?>>
    {
        public Guid Id { get; set; }
    }
}

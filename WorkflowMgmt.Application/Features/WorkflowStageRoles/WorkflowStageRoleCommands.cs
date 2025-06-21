using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkflowMgmt.Domain.Models.Workflow;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.WorkflowStageRoles
{
    // Queries
    public class GetWorkflowStageRolesQuery : IRequest<ApiResponse<List<WorkflowStageRoleDto>>>
    {
        [Required]
        public Guid WorkflowStageId { get; set; }
    }

    public class GetActiveRolesQuery : IRequest<ApiResponse<List<RoleOptionDto>>>
    {
    }

    public class GetWorkflowStageDetailsQuery : IRequest<ApiResponse<List<WorkflowStageDetailsDto>>>
    {
        [Required]
        public Guid WorkflowTemplateId { get; set; }
    }

    public class GetWorkflowStageDetailsByIdQuery : IRequest<ApiResponse<WorkflowStageDetailsDto?>>
    {
        [Required]
        public Guid StageId { get; set; }
    }

    // Commands
    public class UpdateWorkflowStageRolesCommand : IRequest<ApiResponse<bool>>
    {
        [Required]
        public Guid StageId { get; set; }

        [Required]
        public List<UpdateRoleDto> Roles { get; set; } = new List<UpdateRoleDto>();
    }

    public class AddWorkflowStageRoleCommand : IRequest<ApiResponse<bool>>
    {
        [Required]
        public Guid StageId { get; set; }

        [Required]
        [MaxLength(20)]
        public string RoleCode { get; set; } = string.Empty;

        public bool IsRequired { get; set; } = true;
    }

    public class RemoveWorkflowStageRoleCommand : IRequest<ApiResponse<bool>>
    {
        [Required]
        public Guid StageId { get; set; }

        [Required]
        [MaxLength(20)]
        public string RoleCode { get; set; } = string.Empty;
    }
}
